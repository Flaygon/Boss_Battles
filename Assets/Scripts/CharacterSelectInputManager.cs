using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelectInputManager : MonoBehaviour
{
    public CharacterSelectPanel[] playerPanels;
    private CharacterSelectPanel activePanel;

    public CharacterMenuSlotManager[] characterSelectionPanels;

    private int[] playerCharacterSelects = { 0, 0, 0, 0, 0 };
    private bool[] playerSelected = { false, false, false, false, false };

    private Color[] playerSelectionColors = { Color.black, Color.red, Color.green, Color.blue, Color.yellow };

    Object[] charactersAvailable;

    public int numColumns;
    public int numRows;

    private int lastJoystickNum = 0;

    private float joystickCheckTime = 0.5f;
    private float currentJoystickCheckTime;

    private void Start()
    {
        charactersAvailable = Resources.LoadAll("Characters");

        CheckJoysticksConnected();

        foreach (CharacterMenuSlotManager iSlotManager in characterSelectionPanels)
        {
            iSlotManager.UpdateSlot(0, false);
            iSlotManager.UpdateSlot(1, false);
            iSlotManager.UpdateSlot(2, false);
            iSlotManager.UpdateSlot(3, false);
        }

        for (int iCharacter = 0; iCharacter < characterSelectionPanels.Length; ++iCharacter)
        {
            characterSelectionPanels[iCharacter].transform.GetChild(0).GetComponent<Button>().interactable = iCharacter < charactersAvailable.Length;
        }

        for(int iCharacter = 0; iCharacter < charactersAvailable.Length; ++iCharacter)
        {
            CharacterInformation info = (charactersAvailable[iCharacter] as GameObject).GetComponent<CharacterInformation>();
            characterSelectionPanels[iCharacter].transform.GetChild(0).GetComponent<Image>().sprite = info.icon;
        }
    }

    private void Update()
    {
        CheckJoysticksConnected();

        for (int iPlayer = 0; iPlayer < lastJoystickNum; ++iPlayer)
        {
            UpdatePlayerControls(iPlayer);
        }

        if(Input.GetAxis("Submit") > 0.5f)
        {
            SceneManager.LoadScene("bossmenu");
        }
    }

    private void CheckJoysticksConnected()
    {
        currentJoystickCheckTime += Time.deltaTime;
        if (currentJoystickCheckTime >= joystickCheckTime)
        {
            currentJoystickCheckTime = 0.0f;

            string[] claimedJoysticks = Input.GetJoystickNames();
            int trueJoysticks = 0;
            for (int iJoystick = 0; iJoystick < claimedJoysticks.Length; ++iJoystick)
            {
                if (claimedJoysticks[iJoystick] != "")
                {
                    ++trueJoysticks;
                }
            }

            if (trueJoysticks != lastJoystickNum)
                UpdateNumberOfPlayersConnected(trueJoysticks);
            lastJoystickNum = trueJoysticks;
        }
    }

    private void UpdatePlayerControls(int player)
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal " + player), Input.GetAxis("Vertical " + player), 0.0f);

        if (!playerSelected[player] && direction.magnitude > 0.5)
        {
            playerSelected[player] = true;

            if (Vector3.Dot(direction.normalized, Vector3.right) > 0.75f)
            {
                int previousCharacter = playerCharacterSelects[player];

                playerCharacterSelects[player] = (playerCharacterSelects[player] + 1) % charactersAvailable.Length;

                SelectCharacterForPlayer(player, playerCharacterSelects[player], previousCharacter);
            }
            if (Vector3.Dot(direction.normalized, Vector3.left) > 0.75f)
            {
                int previousCharacter = playerCharacterSelects[player];

                playerCharacterSelects[player] = playerCharacterSelects[player] - 1;
                if (playerCharacterSelects[player] < 0)
                    playerCharacterSelects[player] = charactersAvailable.Length - 1;

                SelectCharacterForPlayer(player, playerCharacterSelects[player], previousCharacter);
            }
            if (Vector3.Dot(direction.normalized, Vector3.up) > 0.75f)
            {
                int previousCharacter = playerCharacterSelects[player];

                playerCharacterSelects[player] = playerCharacterSelects[player] - numColumns;
                if (playerCharacterSelects[player] < 0)
                    playerCharacterSelects[player] = 0;

                SelectCharacterForPlayer(player, playerCharacterSelects[player], previousCharacter);
            }
            if (Vector3.Dot(direction.normalized, Vector3.down) > 0.75f)
            {
                int previousCharacter = playerCharacterSelects[player];

                playerCharacterSelects[player] = playerCharacterSelects[player] + numColumns;
                if (playerCharacterSelects[player] >= charactersAvailable.Length)
                    playerCharacterSelects[player] = charactersAvailable.Length - 1;

                SelectCharacterForPlayer(player, playerCharacterSelects[player], previousCharacter);
            }
        }
        else if (direction.magnitude < 0.5)
        {
            playerSelected[player] = false;
        }
    }

    private void SelectCharacterForPlayer(int player, int character, int previousCharacter)
    {
        PlayerPrefs.SetInt("Player_" + player + "_Character", character);

        characterSelectionPanels[previousCharacter].UpdateSlot(player, false);
        characterSelectionPanels[character].UpdateSlot(player, true);

        activePanel.transform.GetChild(player).GetChild(2).GetComponent<Text>().text = charactersAvailable[character].name;
        activePanel.transform.GetChild(player).GetChild(0).GetChild(0).GetComponent<Image>().sprite = (charactersAvailable[character] as GameObject).GetComponent<CharacterInformation>().icon;
    }

    private void UpdateNumberOfPlayersConnected(int numPlayers)
    {
        for (int iPanel = 0; iPanel < playerPanels.Length; ++iPanel)
        {
            playerPanels[iPanel].gameObject.SetActive(iPanel == numPlayers);
        }

        activePanel = playerPanels[numPlayers];

        for (int iPlayer = 0; iPlayer < numPlayers; ++iPlayer)
        {
            playerCharacterSelects[iPlayer] = PlayerPrefs.GetInt("Player_" + iPlayer + "_Character", 0);
            if (playerCharacterSelects[iPlayer] >= charactersAvailable.Length)
                playerCharacterSelects[iPlayer] = charactersAvailable.Length - 1;
            SelectCharacterForPlayer(iPlayer, playerCharacterSelects[iPlayer], playerCharacterSelects[iPlayer]);
        }
    }
}