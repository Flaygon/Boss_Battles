using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase_1 : PhaseNode
{
    public GameObject projectileAsset;
    public GameObject beamWarningAsset;
    public GameObject beamAsset;

    private GameObject beamObject;

    public Transform projectilePosition;
    public Transform beamPosition;

    public int numProjectileAttacks;
    private int currentNumProjectilesFired;
    public float timeBetweenProjectiles;
    private float currentTimeBetweenProjectiles;

    public float beamWarningTime;
    private float currentBeamWarningTime;
    public float beamTime;
    private float currentBeamTime;

    public float fallingBeginTime;
    private float currentFallingBeginTime;
    public float fallingEndTime;
    private float currentFallingEndTime;

    private enum States
    {
        IDLE,

        PROJECTILES,
        BEAM_WARNING,
        BEAM_FIRING,

        FALLING_BEGIN,
        FALLING,
        FALLING_END,

        NONE,
    }
    private States mainState = States.IDLE;
    private States secondaryState = States.NONE;

    public override void UpdateNode()
    {
        currentPlayerAttachTime += Time.deltaTime;

        Vector3 position = transform.position;

        Level currentLevel = manager.levelManager.GetCurrentLevel();
        if (position.y <= currentLevel.transform.position.y + currentLevel.playerBounds.min.y)
        {
            position.y = currentLevel.transform.position.y + currentLevel.playerBounds.min.y;

            manager.body.velocity = Vector2.zero;
            manager.body.gravityScale = 0.0f;
        }

        if (mainState == States.IDLE && position.y > currentLevel.transform.position.y + currentLevel.playerBounds.min.y)
        {
            SetMainState(States.FALLING_BEGIN);

            manager.body.velocity = Vector2.zero;
            manager.body.gravityScale = 1.0f;
        }

        switch (mainState)
        {
            case States.IDLE:
                /*if (secondaryState == States.STALK)
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
                }*/

                currentIdleTimer += Time.deltaTime;
                if (currentIdleTimer >= idleTimer)
                {
                    currentIdleTimer = 0.0f;
                    ChooseAttack();

                    /*if (secondaryState == States.STALK)
                        secondaryState = States.NONE;*/

                    if (currentPlayerAttachTime >= playerAttachTime)
                    {
                        currentPlayerAttachTime = 0.0f;

                        AttackNewPlayer();
                    }
                }

                break;
            case States.PROJECTILES:
                currentTimeBetweenProjectiles += Time.deltaTime;
                if (currentTimeBetweenProjectiles >= timeBetweenProjectiles)
                {
                    currentTimeBetweenProjectiles = 0.0f;
                    ++currentNumProjectilesFired;

                    GameObject newAttack = Instantiate(projectileAsset, projectilePosition.position, Quaternion.identity);
                    newAttack.GetComponent<TurnTowardsTarget>().target = manager.levelManager.players[currentAttackedPlayer].transform;

                    if (currentNumProjectilesFired >= numProjectileAttacks)
                    {
                        currentNumProjectilesFired = 0;
                        SetMainState(States.IDLE);
                    }
                }
                break;
            case States.BEAM_WARNING:
                if (beamObject == null)
                {
                    beamObject = Instantiate(beamWarningAsset, beamPosition.position, Quaternion.identity);
                    beamObject.GetComponent<TurnTowardsTarget>().target = manager.levelManager.players[currentAttackedPlayer].transform;
                }

                currentBeamWarningTime += Time.deltaTime;
                if (currentBeamWarningTime >= beamWarningTime)
                {
                    currentBeamWarningTime = 0.0f;

                    Quaternion warningRotation = beamObject.transform.rotation;
                    Destroy(beamObject);
                    beamObject = Instantiate(beamAsset, beamPosition.position, warningRotation);
                    beamObject.GetComponent<TurnTowardsTarget>().target = manager.levelManager.players[currentAttackedPlayer].transform;

                    SetMainState(States.BEAM_FIRING);
                }
                break;
            case States.BEAM_FIRING:
                currentBeamTime += Time.deltaTime;
                if (currentBeamTime >= beamTime)
                {
                    currentBeamTime = 0.0f;

                    Destroy(beamObject);

                    SetMainState(States.IDLE);
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
                    SetMainState(States.FALLING_END);
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
        }

        transform.position = position;
    }

    private void ChooseAttack()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                SetMainState(States.PROJECTILES);
                break;
            case 1:
                SetMainState(States.BEAM_WARNING);
                break;
        }
    }

    private void SetMainState(States toSet)
    {
        mainState = toSet;
        manager.animator.SetTrigger(mainState.ToString());
    }

    private void SetSecondaryState(States toSet)
    {
        secondaryState = toSet;
    }

    private void AttackNewPlayer()
    {
        int numPlayers = 0;
        for (int iPlayers = 0; iPlayers < manager.levelManager.players.Length; ++iPlayers)
        {
            if (manager.levelManager.players[iPlayers] != null)
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

        manager.boss.currentHealth -= damage;

        manager.boss.SetHealth(manager.boss.currentHealth);

        if (manager.boss.currentHealth <= 0)
        {
            triggered1 = true;
        }
    }

    public override void OnEnd()
    {
        switch(mainState)
        {
            case States.BEAM_WARNING:
                Destroy(beamObject);
                break;
            case States.BEAM_FIRING:
                Destroy(beamObject);
                break;
        }
    }
}