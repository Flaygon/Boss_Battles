using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public Image bossHealth;

    public Transform[] playerSpawns;

    public Boss boss;
    [HideInInspector]
    public GameObject[] players = { null, null, null, null };

    public Level[] levels;

    private int previousLevel;
    private int nextLevel;

    private bool newLevel;
    public float transitionTime;
    private float currentTransitionTime;

    private Vector3 preLevelChangeCameraPosition;
    public float cameraFollowThreshold;
    public float cameraMinSize;
    public float cameraMaxSize;

    private void Start()
    {
        boss.healthBar = bossHealth;
        boss.levelManager = this;

        // spawn player on random spawn point
        int numPlayers = PlayerPrefs.GetInt("NumPlayers", 0);
        Object[] characters = Resources.LoadAll("Characters");
        for(int iPlayer = 0; iPlayer < numPlayers + 1; ++iPlayer)
        {
            int playerCharacter = PlayerPrefs.GetInt("Player_" + iPlayer + "_Character", 0);
            players[iPlayer] = Instantiate(characters[playerCharacter], playerSpawns[iPlayer].position, Quaternion.identity) as GameObject;
            players[iPlayer].GetComponent<Player>().playerNum = iPlayer;
            players[iPlayer].GetComponent<Player>().levelManager = this;
        }
    }

    private void Update()
    {
        if(newLevel)
        {
            currentTransitionTime += Time.deltaTime;
            if(currentTransitionTime >= transitionTime)
            {
                currentTransitionTime = 0.0f;
                newLevel = false;

                Vector3 newPos = levels[nextLevel].centerPoint.position;
                newPos.z = Camera.main.transform.position.z;
                Camera.main.transform.position = newPos;
            }
            else
            {
                Vector3 slerpedPosition = Vector3.Lerp(levels[previousLevel].centerPoint.position, levels[nextLevel].centerPoint.position, currentTransitionTime / transitionTime);
                slerpedPosition.z = Camera.main.transform.position.z;
                Camera.main.transform.position = slerpedPosition;
            }
        }

        if (boss == null || (players[0] == null && players[1] == null && players[2] == null && players[3] == null))
            SceneManager.LoadScene("charactermenu");
    }

    private void LateUpdate()
    {
        UpdateCamera();
    }

    public void TransitionToLevel(int level)
    {
        preLevelChangeCameraPosition = Camera.main.transform.position;

        previousLevel = nextLevel;
        nextLevel = level;

        newLevel = true;
    }

    public Level GetCurrentLevel()
    {
        return levels[nextLevel];
    }

    private void UpdateCamera()
    {
        Level currentLevel = GetCurrentLevel();

        // Find out middle of all the players and put camera there and encapsulate them
        Bounds playersBounds = new Bounds();
        playersBounds.center = Camera.main.transform.position;
        Vector3 playersMiddle = Vector3.zero;
        int numPlayers = 0;
        for(int iPlayer = 0; iPlayer < players.Length; ++iPlayer)
        {
            if(players[iPlayer] != null)
            {
                ++numPlayers;
                playersMiddle += players[iPlayer].transform.position;
                playersBounds.Encapsulate(players[iPlayer].transform.position);
            }
        }
        playersMiddle /= numPlayers;

        playersMiddle.z = Camera.main.transform.position.z;

        Camera.main.transform.position = playersMiddle;
        Camera.main.orthographicSize = Mathf.Clamp(Mathf.Max(playersBounds.extents.x, playersBounds.extents.y), cameraMinSize, cameraMaxSize);

        // Restrict camera to within the bounding box of the current level
        Vector3 restrictedCameraPosition = Camera.main.transform.position;
        restrictedCameraPosition.x = Mathf.Clamp(Camera.main.transform.position.x, -currentLevel.cameraBounds.extents.x, currentLevel.cameraBounds.extents.x);
        restrictedCameraPosition.y = Mathf.Clamp(Camera.main.transform.position.y, -currentLevel.cameraBounds.extents.y, currentLevel.cameraBounds.extents.y);
        Camera.main.transform.position = restrictedCameraPosition;
    }
}