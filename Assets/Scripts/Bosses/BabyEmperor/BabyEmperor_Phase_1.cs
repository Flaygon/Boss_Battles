using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyEmperor_Phase_1 : PhaseNode
{
    public GameObject[] armorPlates;

    public override void UpdateNode()
    {
        
    }

    public override void OnHit(int damage)
    {
        manager.boss.currentHealth -= damage;

        manager.boss.SetHealth(manager.boss.currentHealth);

        if (manager.boss.currentHealth <= 0)
        {
            foreach (GameObject iArmorPlate in armorPlates)
            {
                iArmorPlate.transform.parent = null;
                iArmorPlate.GetComponent<Movement>().enabled = true;
            }

            triggered1 = true;
        }
    }
}