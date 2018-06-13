using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class PhaseNode : MonoBehaviour
{
    public PhaseManager manager;

    public int health;

    public bool invincible;

    public float maxMovementSpeed;
    public float jumpSpeed;

    public float playerAttachTime;
    protected float currentPlayerAttachTime;
    protected int currentAttackedPlayer;

    public float idleTimer;
    protected float currentIdleTimer;

    public virtual void OnBegin() { }
    public virtual void OnEnd() { }

    public abstract void UpdateNode();

    public PhaseNode node1;
    public PhaseNode node2;
    public PhaseNode node3;
    public PhaseNode node4;
    public PhaseNode node5;

    [HideInInspector] public bool triggered1;
    [HideInInspector] public bool triggered2;
    [HideInInspector] public bool triggered3;
    [HideInInspector] public bool triggered4;
    [HideInInspector] public bool triggered5;

    public virtual void OnHit(int damage) { }
}