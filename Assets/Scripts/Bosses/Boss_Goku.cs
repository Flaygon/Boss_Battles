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

    public GameObject projectileAsset;

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

    public float maxMovementSpeed;
    public float jumpSpeed;
    public float maxfallSpeed;

    public float playerRange;

    public float playerAttachTime;
    private float currentPlayerAttachTime;
    private int currentAttackedPlayer;

    public Transform attackPosition;

    [HideInInspector]
    public LevelManager levelManager;

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

        AttackNewPlayer();
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

        if (currentState == States.IDLE_FIRST && position.y > -Camera.main.orthographicSize)
        {
            SetState(States.FALLING_BEGIN);
        }

        currentPlayerAttachTime += Time.deltaTime;

        switch (currentState)
        {
            case States.IDLE_FIRST:
                {
                    transform.rotation = Quaternion.identity;
                    Vector3 distanceToPlayer = levelManager.players[currentAttackedPlayer].transform.position - transform.position;
                    Vector3 directionToPlayer = distanceToPlayer.normalized;
                    if (Vector3.Dot(directionToPlayer, Vector3.right) < 0.0f)
                    {
                        transform.Rotate(0.0f, 180.0f, 0.0f);

                        if (distanceToPlayer.magnitude > playerRange)
                            position.x -= maxMovementSpeed * Time.deltaTime;
                    }
                    else
                    {
                        if (distanceToPlayer.magnitude > playerRange)
                            position.x += maxMovementSpeed * Time.deltaTime;
                    }

                    if (Vector3.Dot(directionToPlayer, Vector3.up) > 0.75f)
                    {
                        SetState(States.FALLING_BEGIN);

                        velocity.y = jumpSpeed;
                    }

                    currentFirstAttackTimer += Time.deltaTime;
                    if (currentFirstAttackTimer >= firstAttackTimer)
                    {
                        currentFirstAttackTimer = 0.0f;
                        SetState(States.ATTACK_FIRST);

                        if(currentPlayerAttachTime >= playerAttachTime)
                        {
                            currentPlayerAttachTime = 0.0f;

                            AttackNewPlayer();
                        }
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

                        GameObject newAttack = Instantiate(projectileAsset, attackPosition.position, Quaternion.identity);
                        newAttack.GetComponent<TurnTowardsTarget>().target = levelManager.players[currentAttackedPlayer].transform;

                        if(currentFirstAttacks >= firstAttacks)
                        {
                            currentFirstAttacks = 0;
                            SetState(States.IDLE_FIRST);
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

                        SetState(States.FALLING);
                    }
                    break;
                }
            case States.FALLING:
                {
                    if (position.y <= -Camera.main.orthographicSize)
                    {
                        SetState(States.FALLING_END);
                    }
                    break;
                }

            case States.FALLING_END:
                {
                    currentFallingEndTime += Time.deltaTime;
                    if (currentFallingEndTime >= fallingEndTime)
                    {
                        currentFallingEndTime = 0.0f;

                        SetState(States.IDLE_FIRST);
                    }
                    break;
                }
        }

        transform.position = position;
    }

    private void SetState(States toSet)
    {
        currentState = toSet;
        animator.SetTrigger(currentState.ToString());
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

    private void AttackNewPlayer()
    {
        int numPlayers = 0;
        for(int iPlayers = 0; iPlayers < levelManager.players.Length; ++iPlayers)
        {
            if(levelManager.players[iPlayers] != null)
            {
                ++numPlayers;
            }
        }

        currentAttackedPlayer = Random.Range(0, numPlayers);
    }
}