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
    private float transitionTime;
    private float currentTransitionTime;

    private Vector3 preLevelChangeCameraPosition;
    public float cameraFollowThreshold;
    public float cameraMinSize;
    public float cameraMaxSize;

    private Vector3[] playerTransitionPositions = { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
    private Vector3 bossTransitionPosition = Vector3.zero;

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

    private void LateUpdate()
    {
        if(!newLevel)
        {
            UpdateCamera();

            if (boss == null || (players[0] == null && players[1] == null && players[2] == null && players[3] == null))
                SceneManager.LoadScene("charactermenu");
        }
        else
        {
            if (newLevel)
            {
                currentTransitionTime += Time.deltaTime;
                if (currentTransitionTime >= transitionTime)
                {
                    currentTransitionTime = 0.0f;
                    newLevel = false;

                    Vector3 newPos = levels[nextLevel].centerPoint.position;
                    newPos.z = Camera.main.transform.position.z;
                    Camera.main.transform.position = newPos;
                }
                else
                {
                    float transitionDelta = currentTransitionTime / transitionTime;

                    Vector3 slerpedPosition = Vector3.Lerp(levels[previousLevel].centerPoint.position, levels[nextLevel].centerPoint.position, transitionDelta);
                    slerpedPosition.z = Camera.main.transform.position.z;
                    Camera.main.transform.position = slerpedPosition;

                    Vector3 positionDistance = bossTransitionPosition - levels[previousLevel].centerPoint.position;
                    boss.transform.position = Vector3.Lerp(levels[previousLevel].centerPoint.position + positionDistance, levels[nextLevel].centerPoint.position + positionDistance, transitionDelta);
                    for (int iPlayer = 0; iPlayer < PlayerPrefs.GetInt("NumPlayers", 0) + 1; ++iPlayer)
                    {
                        positionDistance = players[iPlayer].transform.position - levels[previousLevel].centerPoint.position;
                        playerTransitionPositions[iPlayer] = Vector3.Lerp(levels[previousLevel].centerPoint.position + positionDistance, levels[nextLevel].centerPoint.position + positionDistance, transitionDelta);
                    }
                }
            }
        }
    }

    public void TransitionToLevel(int level, float transitionTime)
    {
        preLevelChangeCameraPosition = Camera.main.transform.position;

        previousLevel = nextLevel;
        nextLevel = level;

        this.transitionTime = transitionTime;

        bossTransitionPosition = boss.transform.position;
        for (int iPlayer = 0; iPlayer < PlayerPrefs.GetInt("NumPlayers", 0) + 1; ++iPlayer)
        {
            playerTransitionPositions[iPlayer] = players[iPlayer].transform.position;
        }

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
        restrictedCameraPosition.x = Mathf.Clamp(Camera.main.transform.position.x, currentLevel.centerPoint.position.x + currentLevel.cameraBounds.min.x, currentLevel.centerPoint.position.x + currentLevel.cameraBounds.max.x);
        restrictedCameraPosition.y = Mathf.Clamp(Camera.main.transform.position.y, currentLevel.centerPoint.position.y + currentLevel.cameraBounds.min.y, currentLevel.centerPoint.position.y + currentLevel.cameraBounds.max.y);
        Camera.main.transform.position = restrictedCameraPosition;
    }
}