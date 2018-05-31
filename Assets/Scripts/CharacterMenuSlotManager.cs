using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMenuSlotManager : MonoBehaviour
{
    public GameObject[] playerslots;

    public void UpdateSlot(int player, bool selected)
    {
        playerslots[player].SetActive(selected);
    }
}