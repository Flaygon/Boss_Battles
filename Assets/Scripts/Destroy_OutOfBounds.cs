using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy_OutOfBounds : MonoBehaviour
{
    public float fuzzy = 1.0f;

    private void LateUpdate()
    {
        if (transform.position.x < -Camera.main.orthographicSize - fuzzy)
            Destroy(gameObject);
        if (transform.position.x > Camera.main.orthographicSize + fuzzy)
            Destroy(gameObject);
        if (transform.position.y < -Camera.main.orthographicSize - fuzzy)
            Destroy(gameObject);
        if (transform.position.y > Camera.main.orthographicSize + fuzzy)
            Destroy(gameObject);
    }
}