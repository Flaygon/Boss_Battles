using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOverTime : MonoBehaviour
{
    private Vector3 beginScale;
    public Vector3 targetScale;

    public float time;
    private float currentTime;

    private void Start()
    {
        beginScale = transform.localScale;
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        transform.localScale = Vector3.Lerp(beginScale, targetScale, currentTime / time);
    }
}