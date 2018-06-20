using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase_Idle : PhaseNode
{
    public GameObject[] armorPlates;

    public override void UpdateNode()
    {
        manager.body.velocity = Vector3.zero;
        manager.body.gravityScale = 0.0f;
    }

    public override void OnHit(int damage)
    {
        foreach(GameObject iArmorPlate in armorPlates)
        {
            iArmorPlate.transform.parent = null;
            iArmorPlate.GetComponent<Movement>().enabled = true;
        }

        triggered1 = true;
    }
}