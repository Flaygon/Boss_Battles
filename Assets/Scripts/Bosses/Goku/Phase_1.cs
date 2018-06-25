using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase_1 : PhaseNode
{
    public GameObject projectileAsset;
    public GameObject beamWarningAsset;
    public GameObject beamAsset;
    public GameObject dashHitAsset;

    public AudioSource projectileAudioAsset;
    public AudioSource beamWarningAudioAsset;
    public AudioSource beamAudioAsset;
    public AudioSource dashAudioAsset;
    public AudioSource dashHitAudioAsset;

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

    public int dashHits;
    private int currentDashHits = 0;
    public float dashHitSpeed;
    private float currentDashHitSpeed;

    public float dashMovementSpeedMultiplier;

    public ContactFilter2D dashHitBackgroundContactFilter;
    public float dashHitBackgroundInterractRadius;

    private enum States
    {
        IDLE,

        PROJECTILES,

        BEAM_MOVING,
        BEAM_WARNING,
        BEAM_FIRING,

        DASH,
        DASH_HITTING,

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

        Vector3 position = manager.boss.transform.position;

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
                currentIdleTimer += Time.deltaTime;
                if (currentIdleTimer >= idleTimer)
                {
                    currentIdleTimer = 0.0f;
                    ChooseAttack();

                    if (currentPlayerAttachTime >= playerAttachTime)
                    {
                        currentPlayerAttachTime = 0.0f;

                        AttackNewPlayer();
                    }
                }

                break;
            case States.PROJECTILES:
                manager.boss.transform.rotation = Quaternion.identity;
                Vector3 distanceToRecordedPlayerPositionNoHeight4 = manager.levelManager.players[currentAttackedPlayer].transform.position - manager.boss.transform.position;
                distanceToRecordedPlayerPositionNoHeight4.y = 0.0f;
                Vector3 directionToPlayerNoHeight4 = distanceToRecordedPlayerPositionNoHeight4.normalized;

                if (Vector3.Dot(directionToPlayerNoHeight4, Vector3.right) <= 0.0f)
                {
                    manager.boss.transform.Rotate(0.0f, 180.0f, 0.0f);
                }

                currentTimeBetweenProjectiles += Time.deltaTime;
                if (currentTimeBetweenProjectiles >= timeBetweenProjectiles)
                {
                    currentTimeBetweenProjectiles = 0.0f;
                    ++currentNumProjectilesFired;

                    GameObject newAttack = Instantiate(projectileAsset, projectilePosition.position, projectilePosition.rotation);
                    newAttack.GetComponent<TurnTowardsTarget>().target = manager.levelManager.players[currentAttackedPlayer].transform;

                    projectileAudioAsset.Play();

                    if (currentNumProjectilesFired >= numProjectileAttacks)
                    {
                        currentNumProjectilesFired = 0;
                        SetMainState(States.IDLE);
                    }
                }
                break;
            case States.BEAM_MOVING:
                manager.boss.transform.rotation = Quaternion.identity;
                Vector3 distanceToRecordedPlayerPositionNoHeight1 = manager.levelManager.GetCurrentLevel().transform.position - manager.boss.transform.position;
                distanceToRecordedPlayerPositionNoHeight1.y = 0.0f;
                Vector3 directionToPlayerNoHeight1 = distanceToRecordedPlayerPositionNoHeight1.normalized;

                if (distanceToRecordedPlayerPositionNoHeight1.magnitude > 1.0f)
                {
                    if (Vector3.Dot(directionToPlayerNoHeight1, Vector3.right) <= 0.0f)
                    {
                        manager.boss.transform.Rotate(0.0f, 180.0f, 0.0f);

                        position.x -= maxMovementSpeed * Time.deltaTime;
                    }
                    else
                    {
                        position.x += maxMovementSpeed * Time.deltaTime;
                    }
                }
                else
                {
                    position.x = manager.levelManager.GetCurrentLevel().transform.position.x;
                    SetMainState(States.BEAM_WARNING);
                }
                break;
            case States.BEAM_WARNING:
                if (beamObject == null)
                {
                    beamObject = Instantiate(beamWarningAsset, beamPosition.position, Quaternion.identity);
                    beamObject.GetComponent<TurnTowardsTarget>().target = manager.levelManager.players[currentAttackedPlayer].transform;

                    beamWarningAudioAsset.Play();
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

                if(!beamAudioAsset.isPlaying)
                    beamAudioAsset.Play();

                if (currentBeamTime >= beamTime)
                {
                    currentBeamTime = 0.0f;

                    Destroy(beamObject);

                    beamAudioAsset.Stop();

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
            case States.DASH:
                manager.boss.transform.rotation = Quaternion.identity;
                Vector3 distanceToRecordedPlayerPositionNoHeight3 = manager.levelManager.players[currentAttackedPlayer].transform.position - manager.boss.transform.position;
                distanceToRecordedPlayerPositionNoHeight3.y = 0.0f;
                Vector3 directionToPlayerNoHeight3 = distanceToRecordedPlayerPositionNoHeight3.normalized;

                if (distanceToRecordedPlayerPositionNoHeight3.magnitude > manager.boss.playerRange)
                {
                    if (Vector3.Dot(directionToPlayerNoHeight3, Vector3.right) <= 0.0f)
                    {
                        manager.boss.transform.Rotate(0.0f, 180.0f, 0.0f);

                        position.x -= maxMovementSpeed * dashMovementSpeedMultiplier * Time.deltaTime;
                    }
                    else
                    {
                        position.x += maxMovementSpeed * dashMovementSpeedMultiplier  * Time.deltaTime;
                    }
                }
                else
                {
                    SetMainState(States.DASH_HITTING);
                }
                break;
            case States.DASH_HITTING:
                manager.boss.transform.rotation = Quaternion.identity;
                Vector3 distanceToRecordedPlayerPositionNoHeight2 = manager.levelManager.players[currentAttackedPlayer].transform.position - manager.boss.transform.position;
                distanceToRecordedPlayerPositionNoHeight2.y = 0.0f;
                Vector3 directionToPlayerNoHeight2 = distanceToRecordedPlayerPositionNoHeight2.normalized;

                if (Vector3.Dot(directionToPlayerNoHeight2, Vector3.right) <= 0.0f)
                {
                    manager.boss.transform.Rotate(0.0f, 180.0f, 0.0f);
                }

                currentDashHitSpeed += Time.deltaTime;
                if(currentDashHitSpeed >= dashHitSpeed)
                {
                    currentDashHitSpeed = 0.0f;

                    ++currentDashHits;

                    Instantiate(dashHitAsset, projectilePosition.position, manager.boss.transform.rotation);

                    dashHitAudioAsset.Play();

                    Collider2D[] hits = new Collider2D[32];
                    int numHits = Physics2D.OverlapCircle(projectilePosition.position, dashHitBackgroundInterractRadius, dashHitBackgroundContactFilter, hits);
                    for (int iHit = 0; iHit < numHits; ++iHit)
                    {
                        hits[iHit].GetComponent<Rigidbody2D>().gravityScale = 1.0f;
                        hits[iHit].GetComponent<Rigidbody2D>().velocity = (new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(0.5f, 1.0f), 0.0f)).normalized * Random.Range(5.0f, 10.0f);
                    }

                    if (currentDashHits >= dashHits)
                    {
                        currentDashHits = 0;

                        SetMainState(States.IDLE);
                    }
                }
                break;
        }

        manager.boss.transform.position = position;
    }

    private void ChooseAttack()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                SetMainState(States.PROJECTILES);
                break;
            case 1:
                SetMainState(States.BEAM_MOVING);
                break;
            case 2:
                SetMainState(States.DASH);
                dashAudioAsset.Play();
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

        manager.boss.healthManager.UpdateHealth(manager.boss.currentHealth, health, 0);

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
                beamWarningAudioAsset.Stop();
                break;
            case States.BEAM_FIRING:
                Destroy(beamObject);
                beamAudioAsset.Stop();
                break;
        }
    }
}