using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss_Goku : MonoBehaviour
{
    public int health;
    private int currentHealth;

    public bool invincible;

    public Image healthBar;

    public Animator animator;

    public float firstAttackTimer;
    private float currentFirstAttackTimer;
    public int firstAttacks;
    private int currentFirstAttacks;
    public float betweenFirstAttacks;
    private float currentBetweenFirstAttacks;

    public float fallingBeginTime;
    private float currentFallingBeginTime;
    public float fallingEndTime;
    private float currentFallingEndTime;

    public float gravity;
    private Vector3 velocity;

    public float maxfallSpeed;

    private enum States
    {
        IDLE_FIRST,
        ATTACK_FIRST,

        FALLING_BEGIN,
        FALLING,
        FALLING_END,
    }
    private States currentState = States.IDLE_FIRST;

    private void Start()
    {
        currentHealth = health;
    }

    private void Update()
    {
        velocity.y = Mathf.Clamp(velocity.y - gravity * Time.deltaTime, -maxfallSpeed, maxfallSpeed);
        Vector3 position = transform.position;
        position += velocity * Time.deltaTime;

        if (position.y <= -Camera.main.orthographicSize)
        {
            position.y = -Camera.main.orthographicSize;
        }

        transform.position = position;

        if (currentState == States.IDLE_FIRST && position.y > -Camera.main.orthographicSize)
        {
            currentState = States.FALLING_BEGIN;
            animator.SetTrigger(currentState.ToString());
        }

        switch (currentState)
        {
            case States.IDLE_FIRST:
                {
                    currentFirstAttackTimer += Time.deltaTime;
                    if (currentFirstAttackTimer >= firstAttackTimer)
                    {
                        currentFirstAttackTimer = 0.0f;
                        currentState = States.ATTACK_FIRST;
                        animator.SetTrigger(currentState.ToString());
                    }

                    break;
                }
            case States.ATTACK_FIRST:
                {
                    currentBetweenFirstAttacks += Time.deltaTime;
                    if(currentBetweenFirstAttacks >= betweenFirstAttacks)
                    {
                        currentBetweenFirstAttacks = 0.0f;
                        ++currentFirstAttacks;

                        // make attack

                        if(currentFirstAttacks >= firstAttacks)
                        {
                            currentFirstAttacks = 0;
                            currentState = States.IDLE_FIRST;
                            animator.SetTrigger(currentState.ToString());
                        }
                    }
                    break;
                }
            case States.FALLING_BEGIN:
                {
                    currentFallingBeginTime += Time.deltaTime;
                    if (currentFallingBeginTime >= fallingBeginTime)
                    {
                        currentFallingBeginTime = 0.0f;
                        ++currentFirstAttacks;

                        currentState = States.FALLING;
                        animator.SetTrigger(currentState.ToString());
                    }
                    break;
                }
            case States.FALLING:
                {
                    if (position.y <= -Camera.main.orthographicSize)
                    {
                        currentState = States.FALLING_END;
                        animator.SetTrigger(currentState.ToString());
                    }
                    break;
                }

            case States.FALLING_END:
                {
                    currentFallingEndTime += Time.deltaTime;
                    if (currentFallingEndTime >= fallingEndTime)
                    {
                        currentFallingEndTime = 0.0f;

                        currentState = States.IDLE_FIRST;
                        animator.SetTrigger(currentState.ToString());
                    }
                    break;
                }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Damage damage = collision.gameObject.GetComponent<Damage>();
        if(damage)
        {
            OnHit(damage.damage);
        }
    }

    public void OnHit(int damage)
    {
        if (invincible)
            return;

        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            Destroy(gameObject);
        }

        healthBar.fillAmount = currentHealth / (float)health;
    }
}