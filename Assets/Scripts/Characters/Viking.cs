using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viking : MonoBehaviour
{
    public Sprite icon;

    public Player player;
    public Transform feet;

    public Vector3 velocity;
    public float gravity;
    public float maxGravityVelocity;

    public float movementSpeed;

    private bool airborne = true;

    private bool powerAxing;

    public GameObject throwArrow;

    public GameObject axeAsset;
    public GameObject powerAxeAsset;

    public float throwTimer;
    private float currentThrowTimer = 0.0f;
    public float throwVelocity;

    private void Update()
    {
        UpdateAttack();
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        if (powerAxing)
            return;

        if (!airborne && player.GetPositiveAxis("Jump", 0.5f))
        {
            airborne = true;

            velocity.y = maxGravityVelocity;
        }

        float horizontalAxis = player.GetAxis("Horizontal");
        if (horizontalAxis > 0.2f || horizontalAxis < -0.2f)
        {
            velocity.x = horizontalAxis * movementSpeed;
        }
        else
        {
            velocity.x = 0.0f;
        }

        if (airborne)
        {
            velocity.y = Mathf.Clamp(velocity.y - gravity * Time.deltaTime, -maxGravityVelocity, 9999.0f);
        }
        else
        {
            velocity.y = 0.0f;
        }

        Vector3 newPosition = transform.position;
        newPosition += velocity * Time.deltaTime;

        if (airborne && newPosition.y < -(Camera.main.orthographicSize + feet.localPosition.y))
        {
            newPosition.y = -(Camera.main.orthographicSize + feet.localPosition.y);

            airborne = false;
        }

        // Restraining within view
        if (newPosition.x < -Camera.main.orthographicSize)
        {
            newPosition.x = -Camera.main.orthographicSize;
        }
        else if (newPosition.x > Camera.main.orthographicSize)
        {
            newPosition.x = Camera.main.orthographicSize;
        }

        transform.position = newPosition;
    }

    private void UpdateAttack()
    {
        powerAxing = !airborne && player.GetPositiveAxis("Left Bumper", 0.2f);

        float horizontalAim = player.GetAxis("Horizontal Aim");
        float verticalAim = player.GetAxis("Vertical Aim");
        if (!Mathf.Approximately(horizontalAim, 0.0f) || !Mathf.Approximately(verticalAim, 0.0f))
        {
            throwArrow.SetActive(true);

            Vector3 direction = new Vector3(horizontalAim, verticalAim, 0.0f).normalized;

            throwArrow.transform.rotation = Quaternion.identity;
            throwArrow.transform.Rotate(0.0f, 0.0f, Vector3.SignedAngle(Vector3.down, direction, Vector3.forward));

            if(player.GetNegativeAxis("Triggers", 0.0f))
            {
                if (currentThrowTimer >= throwTimer)
                {
                    currentThrowTimer = 0.0f;

                    GameObject newAxe = Instantiate(powerAxing ? powerAxeAsset : axeAsset, transform.position + direction, throwArrow.transform.rotation);
                    newAxe.GetComponent<Movement>().velocity = direction * throwVelocity;
                }
            }
        }
        else
        {
            throwArrow.SetActive(false);
        }

        currentThrowTimer += Time.deltaTime;
    }
}