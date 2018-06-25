using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : Entity
{
    public Sprite icon;

    public Player player;
    public Transform bottom;

    public float jumpSpeed;
    protected bool jumped = false;
    protected bool airborn = false;

    public float movementSpeed;
    protected Vector3 velocity;
    protected static float maxFallSpeed = 10.0f;
    protected static float gravity = 9.87f;

    protected bool gravityApplied = true;

    public LayerMask attackLayer;

    protected virtual void Update()
    {
        if (incapacitated)
            return;

        if(gravityApplied)
        {
            velocity.y = Mathf.Clamp(velocity.y - gravity * Time.deltaTime, -maxFallSpeed, 9999.0f);
        }

        UpdateJump();
        UpdateMovement();
        UpdateAttack();
    }

    protected virtual void UpdateJump()
    {
        if (!airborn && player.GetPositiveAxis("Jump", 0.5f))
        {
            if (!jumped)
            {
                velocity.y = jumpSpeed;
            }

            airborn = true;
            jumped = true;
        }
        else
        {
            jumped = false;
        }
    }

    protected virtual void UpdateMovement()
    {
        // Horizontal movement
        float horizontalAxis = player.GetAxis("Horizontal");
        if (horizontalAxis > 0.2f || horizontalAxis < -0.2f)
        {
            velocity.x = horizontalAxis * movementSpeed;
        }
        else
        {
            velocity.x = 0.0f;
        }

        // Final position calculation
        Vector3 newPosition = transform.position;

        newPosition += velocity * Time.deltaTime;

        // Restraining to ground
        Level currentLevel = player.levelManager.GetCurrentLevel();
        if (newPosition.y < currentLevel.transform.position.y + currentLevel.playerBounds.min.y - bottom.localPosition.y)
        {
            newPosition.y = currentLevel.transform.position.y + currentLevel.playerBounds.min.y - bottom.localPosition.y;

            velocity.y = 0.0f;

            airborn = false;
            jumped = false;
        }

        // Restraining within view
        if (newPosition.x < currentLevel.transform.position.x + currentLevel.playerBounds.min.x)
        {
            newPosition.x = currentLevel.transform.position.x + currentLevel.playerBounds.min.x;
            velocity.x = 0.0f;
        }
        else if (newPosition.x > currentLevel.transform.position.x + currentLevel.playerBounds.max.x)
        {
            newPosition.x = currentLevel.transform.position.x + currentLevel.playerBounds.max.x;
            velocity.x = 0.0f;
        }

        transform.position = newPosition;
    }

    protected abstract void UpdateAttack();

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        Damage damage = collider.gameObject.GetComponent<Damage>();
        if (damage != null)
        {
            OnHit(damage.damage);
        }
    }

    protected override int GetEntityIndex()
    {
        return GetComponent<Player>().playerNum;
    }
}