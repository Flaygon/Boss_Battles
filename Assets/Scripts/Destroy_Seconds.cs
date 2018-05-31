using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy_Seconds : MonoBehaviour
{
    public float seconds;
    private float currentSeconds = 0.0f;

    private void LateUpdate()
    {
        currentSeconds += Time.deltaTime;
        if(currentSeconds >= seconds)
        {
            Destroy(gameObject);
        }
    }
}