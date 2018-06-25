using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viking : Character
{
    private bool powerAxing;

    public GameObject throwArrowObject;

    public GameObject axeAsset;
    public GameObject powerAxeAsset;

    public float throwTimer;
    private float currentThrowTimer = 0.0f;
    public float throwVelocity;

    protected override void UpdateMovement()
    {
        if (powerAxing)
            return;

        base.UpdateMovement();
    }

    protected override void UpdateAttack()
    {
        powerAxing = !airborn && player.GetPositiveAxis("Left Bumper", 0.2f);

        float horizontalAim = player.GetAxis("Horizontal Aim");
        float verticalAim = player.GetAxis("Vertical Aim");
        if (!Mathf.Approximately(horizontalAim, 0.0f) || !Mathf.Approximately(verticalAim, 0.0f))
        {
            throwArrowObject.SetActive(true);

            Vector3 direction = new Vector3(horizontalAim, verticalAim, 0.0f).normalized;

            throwArrowObject.transform.rotation = Quaternion.identity;
            throwArrowObject.transform.Rotate(0.0f, 0.0f, Vector3.SignedAngle(Vector3.down, direction, Vector3.forward));

            if(player.GetNegativeAxis("Triggers", 0.0f))
            {
                if (currentThrowTimer >= throwTimer)
                {
                    currentThrowTimer = 0.0f;

                    GameObject newAxe = Instantiate(powerAxing ? powerAxeAsset : axeAsset, transform.position + direction, throwArrowObject.transform.rotation);
                    newAxe.GetComponent<Movement>().velocity = direction * throwVelocity;
                }
            }
        }
        else
        {
            throwArrowObject.SetActive(false);
        }

        currentThrowTimer += Time.deltaTime;
    }
}