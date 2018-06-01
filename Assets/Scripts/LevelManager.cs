using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public Image bossHealth;

    public Transform[] playerSpawns;

    private GameObject spawnedBoss;
    [HideInInspector]
    public GameObject[] players = { null, null, null, null };

    private void Start()
    {
        int boss = PlayerPrefs.GetInt("LastBoss", 0);
        Object[] bosses = Resources.LoadAll("Bosses");
        spawnedBoss = Instantiate(bosses[boss]) as GameObject;
        spawnedBoss.GetComponent<Boss_Goku>().healthBar = bossHealth;
        spawnedBoss.GetComponent<Boss_Goku>().levelManager = this;

        // spawn player on random spawn point
        int numPlayers = PlayerPrefs.GetInt("NumPlayers", 0);
        Object[] characters = Resources.LoadAll("Characters");
        for(int iPlayer = 0; iPlayer < numPlayers + 1; ++iPlayer)
        {
            int playerCharacter = PlayerPrefs.GetInt("Player_" + iPlayer + "_Character", 0);
            players[iPlayer] = Instantiate(characters[playerCharacter], playerSpawns[iPlayer].position, Quaternion.identity) as GameObject;
            players[iPlayer].GetComponent<Player>().playerNum = iPlayer;
        }
    }

    private void Update()
    {
        if (spawnedBoss == null || (players[0] == null && players[1] == null && players[2] == null && players[3] == null))
            SceneManager.LoadScene("charactermenu");
    }
}