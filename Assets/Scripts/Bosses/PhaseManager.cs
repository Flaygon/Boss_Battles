using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D body;
    public Boss boss;
    public LevelManager levelManager;

    public PhaseNode currentPhase;

    private void Start()
    {
        boss.SetHealth(currentPhase.health);
    }

    private void Update()
    {
        if (currentPhase.triggered1)
        {
            SwitchPhase(currentPhase.node1);
        }
        if (currentPhase.triggered2)
        {
            SwitchPhase(currentPhase.node2);
        }
        if (currentPhase.triggered3)
        {
            SwitchPhase(currentPhase.node3);
        }
        if (currentPhase.triggered4)
        {
            SwitchPhase(currentPhase.node4);
        }
        if (currentPhase.triggered5)
        {
            SwitchPhase(currentPhase.node5);
        }

        currentPhase.UpdateNode();
    }

    private void SwitchPhase(PhaseNode nextNode)
    {
        currentPhase.OnEnd();
        currentPhase = nextNode;
        boss.SetHealth(currentPhase.health);
        currentPhase.OnBegin();
    }

    public void OnHit(int damage)
    {
        currentPhase.OnHit(damage);
    }
}