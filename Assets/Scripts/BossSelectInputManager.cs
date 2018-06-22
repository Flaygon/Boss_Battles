using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossSelectInputManager : MonoBehaviour
{
    public Image bossImage;

    public Button[] bossPanels;

    public int selectedBoss;
    public Color selectedBossColor;

    Object[] bossesAvailable;

    private bool selected;

    public int numColumns;
    public int numRows;

    public AudioClip music;

    private void Awake()
    {
        ApplicationManager.Get().PlayMusic(music);
    }

    private void Start()
    {
        bossesAvailable = Resources.LoadAll("Bosses/Information");

        for (int iPanel = 0; iPanel < bossPanels.Length; ++iPanel)
        {
            bossPanels[iPanel].interactable = iPanel < bossesAvailable.Length;

            if(bossPanels[iPanel].interactable)
                bossPanels[iPanel].GetComponent<Image>().sprite = (bossesAvailable[iPanel] as GameObject).GetComponent<CharacterInformation>().icon;
        }

        selectedBoss = PlayerPrefs.GetInt("LastBoss", 0);
        SelectBoss(selectedBoss, selectedBoss);
    }

    private void Update()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal 0"), Input.GetAxis("Vertical 0"), 0.0f);

        if (!selected && direction.magnitude > 0.5)
        {
            selected = true;

            if (Vector3.Dot(direction.normalized, Vector3.right) > 0.75f)
            {
                int previousBoss = selectedBoss;

                selectedBoss = (selectedBoss + 1) % bossesAvailable.Length;

                SelectBoss(selectedBoss, previousBoss);
            }
            if (Vector3.Dot(direction.normalized, Vector3.left) > 0.75f)
            {
                int previousBoss = selectedBoss;

                selectedBoss = selectedBoss - 1;
                if (selectedBoss < 0)
                    selectedBoss = bossesAvailable.Length - 1;

                SelectBoss(selectedBoss, previousBoss);
            }
            if (Vector3.Dot(direction.normalized, Vector3.up) > 0.75f)
            {
                int previousBoss = selectedBoss;

                selectedBoss = selectedBoss - numColumns;
                if (selectedBoss < 0)
                    selectedBoss = 0;

                SelectBoss(selectedBoss, previousBoss);
            }
            if (Vector3.Dot(direction.normalized, Vector3.down) > 0.75f)
            {
                int previousBoss = selectedBoss;

                selectedBoss = selectedBoss + numColumns;
                if (selectedBoss >= bossesAvailable.Length)
                    selectedBoss = bossesAvailable.Length - 1;

                SelectBoss(selectedBoss, previousBoss);
            }
        }
        else if (direction.magnitude < 0.5)
        {
            selected = false;
        }

        if (Input.GetAxis("Jump 0") > 0.5f)
        {
            SceneManager.LoadScene((bossesAvailable[selectedBoss] as GameObject).GetComponent<BossInformation>().level);
        }

        if (Input.GetAxis("Cancel") > 0.5f)
        {
            SceneManager.LoadScene("charactermenu");
        }
    }

    private void SelectBoss(int boss, int previousBoss)
    {
        PlayerPrefs.SetInt("LastBoss", boss);

        // boss in center of screen

        ColorBlock colorBlockPrevious = bossPanels[previousBoss].colors;
        colorBlockPrevious.normalColor = Color.white;
        bossPanels[previousBoss].colors = colorBlockPrevious;

        ColorBlock colorBlock = bossPanels[boss].colors;
        colorBlock.normalColor = selectedBossColor;
        bossPanels[boss].colors = colorBlock;
    }
}