using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Vector3 velocity;
    public Vector3 gravity;

    private void Update()
    {
        velocity.x += gravity.x * Time.deltaTime;
        velocity.y += gravity.y * Time.deltaTime;
        velocity.z += gravity.z * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
    }
}