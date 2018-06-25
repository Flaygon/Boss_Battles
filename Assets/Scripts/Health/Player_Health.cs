using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Health : HealthManager
{
    public LevelManager manager;

    private Image[] playerHealthObjects = { null, null, null, null };
    public GameObject[] playerHealthAssets;

    public float paddingX;
    public float paddingY;

    public override void Init(int startHealth)
    {
        int numPlayers = 0;
        for (int iPlayer = 0; iPlayer < manager.players.Length; ++iPlayer)
        {
            if (manager.players[iPlayer] != null)
                ++numPlayers;
        }

        for (int iPlayer = 0; iPlayer < numPlayers; ++iPlayer)
        {
            playerHealthObjects[iPlayer] = Instantiate(playerHealthAssets[iPlayer], transform).GetComponent<Image>();
            RectTransform rect = playerHealthObjects[iPlayer].GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(paddingX + (1.0f / numPlayers) * iPlayer, paddingY);
            rect.anchorMax = new Vector2((1.0f / numPlayers) * (iPlayer + 1) - paddingX, 1 - paddingY);
        }
    }

    public override void UpdateHealth(int currentHealth, int health, int player)
    {
        playerHealthObjects[player].fillAmount = (float)currentHealth / health;
    }
}