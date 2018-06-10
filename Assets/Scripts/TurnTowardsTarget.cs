using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTowardsTarget : MonoBehaviour
{
    [Range(0.0f, 360.0f)]
    public float turnSpeed;

    public Transform target;

    public Vector3 forwardDirection;
    public Vector3 perpendicularToForward;

    public bool initialTarget;

    private void Start()
    {
        if(initialTarget)
        {
            Vector3 localForward = transform.rotation * forwardDirection;
            Vector3 targetDirection = (target.position - transform.position).normalized;
            if (Vector3.Dot(localForward, targetDirection) <= 0.999f)
            {
                Vector3 localPerpendicular = transform.rotation * perpendicularToForward;
                transform.Rotate(0.0f, 0.0f, Vector3.SignedAngle(localForward, targetDirection, localForward));
            }
        }
    }

    private void Update()
    {
        Vector3 localForward = transform.rotation * forwardDirection;
        Vector3 targetDirection = (target.position - transform.position).normalized;
        if (Vector3.Dot(localForward, targetDirection) <= 0.999f)
        {
            Vector3 localPerpendicular = transform.rotation * perpendicularToForward;
            if(Vector3.Dot(localPerpendicular, targetDirection) >= 0.0f)
            {
                transform.Rotate(0.0f, 0.0f, turnSpeed * Time.deltaTime);
            }
            else
            {
                transform.Rotate(0.0f, 0.0f, -turnSpeed * Time.deltaTime);
            }
        }
    }
}