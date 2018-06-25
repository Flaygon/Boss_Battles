using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja : Character
{
    private bool doubleJumped = false;

    private int lastMovementDirection = 1;

    public float attackSpeed;
    private float currentAttackSpeed;

    public float attackDistance;
    public int attackDamage;

    private bool attacking;

    public GameObject attackAsset;

    public GameObject dashAsset;
    public float dashTime;
    private float currentDashTime;
    public float dashDistance;

    protected override void Update()
    {
        if (incapacitated)
            return;

        base.Update();
    }

    protected override void UpdateJump()
    {
        if (!doubleJumped)
        {
            if (player.GetPositiveAxis("Jump", 0.5f))
            {
                if (!jumped)
                {
                    if (airborn)
                    {
                        doubleJumped = true;
                    }
                    airborn = true;

                    velocity.y = jumpSpeed;
                }

                jumped = true;
            }
            else
            {
                jumped = false;
            }
        }
    }

    protected override void UpdateMovement()
    {
        currentDashTime += Time.deltaTime;

        if (attacking)
            return;

        // Horizontal movement
        float horizontalAxis = player.GetAxis("Horizontal");
        if (horizontalAxis > 0.2f || horizontalAxis < -0.2f)
        {
            velocity.x = horizontalAxis * movementSpeed;

            lastMovementDirection = horizontalAxis > 0.0f ? 1 : -1;
        }
        else
        {
            velocity.x = 0.0f;
        }

        // Final position calculation
        Vector3 newPosition = transform.position;

        // Dash
        if (currentDashTime >= dashTime && player.GetPositiveAxis("B", 0.1f))
        {
            currentDashTime = 0.0f;

            velocity.y = 0.0f;

            Vector3 previousPosition = newPosition;
            newPosition.x += lastMovementDirection * dashDistance;

            GameObject dashLine = Instantiate(dashAsset, Vector3.Lerp(previousPosition, newPosition, 0.5f), Quaternion.identity);
            Vector3 stretched = dashLine.transform.localScale;
            stretched.x = Vector3.Distance(previousPosition, newPosition) * 3;
            dashLine.transform.localScale = stretched;
        }

        newPosition += velocity * Time.deltaTime;

        // Restraining to ground
        Level currentLevel = player.levelManager.GetCurrentLevel();
        if (newPosition.y < currentLevel.transform.position.y + currentLevel.playerBounds.min.y - bottom.localPosition.y)
        {
            newPosition.y = currentLevel.transform.position.y + currentLevel.playerBounds.min.y - bottom.localPosition.y;

            airborn = false;
            jumped = false;
            doubleJumped = false;
        }

        // Restraining within view
        if(newPosition.x < currentLevel.transform.position.x + currentLevel.playerBounds.min.x)
        {
            newPosition.x = currentLevel.transform.position.x + currentLevel.playerBounds.min.x;
        }
        else if (newPosition.x > currentLevel.transform.position.x + currentLevel.playerBounds.max.x)
        {
            newPosition.x = currentLevel.transform.position.x + currentLevel.playerBounds.max.x;
        }

        transform.position = newPosition;
    }

    protected override void UpdateAttack()
    {
        currentAttackSpeed += Time.deltaTime;

        if(player.GetPositiveAxis("Right Bumper", 0.5f))
        {
            if(currentAttackSpeed >= attackSpeed)
            {
                currentAttackSpeed = 0.0f;
                attacking = false;

                GameObject attackEffect = Instantiate(attackAsset, new Vector3(transform.position.x + lastMovementDirection * attackDistance, transform.position.y + Random.Range(-attackDistance, attackDistance), 0.0f), Quaternion.identity) as GameObject;
                attackEffect.transform.Rotate(0.0f, 0.0f, Random.Range(-60.0f, 60.0f));

                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackDistance, attackLayer);
                if (colliders.Length > 0)
                {
                    foreach(Collider2D iCollider in colliders)
                    {
                        if((lastMovementDirection > 0 && Vector3.Dot((iCollider.transform.position - transform.position).normalized, Vector3.right) > 0.0f)
                            || (lastMovementDirection < 0 && Vector3.Dot((iCollider.transform.position - transform.position).normalized, Vector3.left) > 0.0f))
                        {
                            attacking = true;

                            Boss boss = iCollider.gameObject.GetComponent<Boss>();
                            if (boss)
                            {
                                boss.OnHit(attackDamage);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            attacking = false;
        }
    }
}