using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D body;
    public Boss boss;

    public int health;

    public bool invincible;

    public float maxMovementSpeed;
    public float jumpSpeed;

    public float playerAttachTime;
    protected float currentPlayerAttachTime;
    protected int currentAttackedPlayer;

    public float idleTimer;
    protected float currentIdleTimer;

    [HideInInspector]
    public bool running = false;

    public virtual void OnHit(int damage) { }
}