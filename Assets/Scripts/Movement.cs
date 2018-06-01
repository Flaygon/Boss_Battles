using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public bool local;

    public Vector3 velocity;
    public Vector3 gravity;

    private void Update()
    {
        velocity.x += gravity.x * Time.deltaTime;
        velocity.y += gravity.y * Time.deltaTime;
        velocity.z += gravity.z * Time.deltaTime;
        if(local)
        {
            Vector3 newVelocity = transform.rotation * velocity;
            transform.localPosition += newVelocity * Time.deltaTime;
        }
        else
        {
            transform.position += velocity * Time.deltaTime;
        }
    }
}