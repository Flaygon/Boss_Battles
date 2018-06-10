using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase_2 : Phase
{
    public GameObject projectileAsset;
    public GameObject fallExplosionAsset;
    public GameObject moonlightAsset;

    private GameObject moonlightObject;

    public Transform projectilePosition;

    public float flyingHeight;

    public int numProjectileAttacks;
    private int currentNumProjectilesFired;
    public float timeBetweenProjectiles;
    private float currentTimeBetweenProjectiles;

    public float moonlightGazeTime;
    private float currentMoonlightGazeTime;

    public float groundStompStalkTime;
    private float currentGroundStompStalkTime;
    public int numGroundStompsBeforeStageSwitch;
    private int currentNumGroundStomps;

    public float fallingBeginTime;
    private float currentFallingBeginTime;
    public float fallingEndTime;
    private float currentFallingEndTime;

    private enum States
    {
        IDLE,

        PROJECTILES,
        GROUND_SLAM,

        MOONLIGHT,

        FALLING_BEGIN,
        FALLING,
        FALLING_END,

        STALK,

        FLYING,

        NONE,
    }
    private States mainState = States.IDLE;
    private States secondaryState = States.NONE;

    private void Update()
    {
        if (!running)
            return;

        currentPlayerAttachTime += Time.deltaTime;

        /*if (secondaryState != States.FLYING)
        {
            velocity.y = Mathf.Clamp(velocity.y - gravity * Time.deltaTime, -maxfallSpeed, maxfallSpeed);
        }*/

        Vector3 position = transform.position;

        Level currentLevel = boss.levelManager.GetCurrentLevel();
        if (mainState == States.IDLE && position.y > currentLevel.transform.position.y + currentLevel.cameraBounds.min.y)
        {
            SetMainState(States.FALLING_BEGIN);

            body.velocity = Vector2.zero;
            body.gravityScale = 1.0f;
        }

        switch (mainState)
        {
            case States.IDLE:
                currentIdleTimer += Time.deltaTime;
                if (currentIdleTimer >= idleTimer)
                {
                    currentIdleTimer = 0.0f;

                    if (Random.Range(0, 2) == 0)
                    {
                        SetMainState(States.FLYING);
                        SetSecondaryState(States.FLYING);

                        body.velocity = Vector2.zero;
                        body.gravityScale = -1.0f;
                    }
                    else
                    {
                        SetMainState(States.MOONLIGHT);
                        SetSecondaryState(States.NONE);
                    }

                    if (currentPlayerAttachTime >= playerAttachTime)
                    {
                        currentPlayerAttachTime = 0.0f;

                        AttackNewPlayer();
                    }
                }

                break;
            case States.FLYING:
                if (position.y >= currentLevel.transform.position.y + currentLevel.playerBounds.min.y + flyingHeight)
                {
                    position.y = currentLevel.transform.position.y + currentLevel.playerBounds.min.y + flyingHeight;

                    body.velocity = Vector2.zero;
                    body.gravityScale = 0.0f;

                    ChooseAttack();
                }
                break;
            case States.PROJECTILES:
                currentTimeBetweenProjectiles += Time.deltaTime;
                if (currentTimeBetweenProjectiles >= timeBetweenProjectiles)
                {
                    currentTimeBetweenProjectiles = 0.0f;
                    ++currentNumProjectilesFired;

                    GameObject newAttack = Instantiate(projectileAsset, projectilePosition.position, Quaternion.identity);
                    newAttack.GetComponent<TurnTowardsTarget>().target = boss.levelManager.players[currentAttackedPlayer].transform;

                    if (currentNumProjectilesFired >= numProjectileAttacks)
                    {
                        currentNumProjectilesFired = 0;

                        SetMainState(States.FLYING);
                    }
                }
                break;
            case States.GROUND_SLAM:

                transform.rotation = Quaternion.identity;
                Vector3 distanceToPlayer = boss.levelManager.players[currentAttackedPlayer].transform.position - transform.position;
                Vector3 directionToPlayer = distanceToPlayer.normalized;
                if (Vector3.Dot(directionToPlayer, Vector3.right) < 0.0f)
                {
                    transform.Rotate(0.0f, 180.0f, 0.0f);

                    if (distanceToPlayer.magnitude > boss.playerRange)
                        position.x -= maxMovementSpeed * Time.deltaTime;
                }
                else
                {
                    if (distanceToPlayer.magnitude > boss.playerRange)
                        position.x += maxMovementSpeed * Time.deltaTime;
                }

                currentGroundStompStalkTime += Time.deltaTime;
                if (currentGroundStompStalkTime >= groundStompStalkTime)
                {
                    currentGroundStompStalkTime = 0.0f;

                    SetMainState(States.FALLING_BEGIN);
                    SetSecondaryState(States.NONE);

                    body.velocity = Vector2.zero;
                    body.gravityScale = 1.0f;
                }
                break;
            case States.FALLING_BEGIN:
                currentFallingBeginTime += Time.deltaTime;
                if (currentFallingBeginTime >= fallingBeginTime)
                {
                    currentFallingBeginTime = 0.0f;

                    SetMainState(States.FALLING);
                }
                break;
            case States.FALLING:
                if (position.y <= currentLevel.transform.position.y + currentLevel.playerBounds.min.y)
                {
                    body.velocity = Vector2.zero;
                    body.gravityScale = 0.0f;

                    Instantiate(fallExplosionAsset, transform.position, Quaternion.identity);

                    ++currentNumGroundStomps;

                    if (currentNumGroundStomps >= numGroundStompsBeforeStageSwitch)
                    {
                        currentNumGroundStomps = 0;

                        // TRIGGER SPECIAL PHASE SWAP: BELOW

                        boss.SetPhase(4);
                        (boss.GetCurrentPhase() as Phase_Transform).phase = 2;
                        animator.SetTrigger("TRANSFORM");
                        boss.levelManager.TransitionToLevel(1);
                    }
                    else
                    {
                        SetMainState(States.FALLING_END);
                    }
                }
                break;
            case States.FALLING_END:
                currentFallingEndTime += Time.deltaTime;
                if (currentFallingEndTime >= fallingEndTime)
                {
                    currentFallingEndTime = 0.0f;

                    SetMainState(States.IDLE);
                }
                break;
            case States.MOONLIGHT:
                if (moonlightObject == null)
                {
                    moonlightObject = Instantiate(moonlightAsset, transform.position, Quaternion.identity);
                }

                currentMoonlightGazeTime += Time.deltaTime;
                if (currentMoonlightGazeTime >= moonlightGazeTime)
                {
                    currentMoonlightGazeTime = 0.0f;

                    Destroy(moonlightObject);

                    // TRIGGER SPECIAL PHASE SWAP: ABOVE

                    boss.SetPhase(4);
                    (boss.GetCurrentPhase() as Phase_Transform).phase = 3;
                    animator.SetTrigger("TRANSFORM");
                    boss.levelManager.TransitionToLevel(2);
                }
                break;
        }

        if (position.y <= currentLevel.transform.position.y + currentLevel.playerBounds.min.y)
        {
            position.y = currentLevel.transform.position.y + currentLevel.playerBounds.min.y;
        }

        transform.position = position;
    }

    private void ChooseAttack()
    {
        switch (Random.Range(0, 2))
        {
            case 0:
                SetMainState(States.PROJECTILES);
                break;
            case 1:
                SetMainState(States.GROUND_SLAM);
                break;
        }
    }

    private void SetMainState(States toSet)
    {
        mainState = toSet;
        animator.SetTrigger(mainState.ToString());
    }

    private void SetSecondaryState(States toSet)
    {
        secondaryState = toSet;
    }

    private void AttackNewPlayer()
    {
        int numPlayers = 0;
        for (int iPlayers = 0; iPlayers < boss.levelManager.players.Length; ++iPlayers)
        {
            if (boss.levelManager.players[iPlayers] != null)
            {
                ++numPlayers;
            }
        }

        currentAttackedPlayer = Random.Range(0, numPlayers);
    }

    public override void OnHit(int damage)
    {
        if (invincible)
            return;

        boss.currentHealth -= damage;

        if (mainState == States.MOONLIGHT)
        {
            currentMoonlightGazeTime = 0.0f;
            SetMainState(States.IDLE);

            if (moonlightObject != null)
                Destroy(moonlightObject);
        }

        if (boss.currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}