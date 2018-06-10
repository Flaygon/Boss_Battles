using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase_Transform : Phase
{
    public float transformTime;
    protected float currentTransformTime;

    [HideInInspector]
    public int phase;

    private enum States
    {
        BEGIN,
        MIDDLE,
        END,
    }
    private States mainState = States.BEGIN;

    private void Update()
    {
        if (!running)
            return;

        body.velocity = Vector2.zero;
        body.gravityScale = 0.0f;

        currentTransformTime += Time.deltaTime;
        if (currentTransformTime >= transformTime)
        {
            currentTransformTime = 0.0f;

            boss.SetPhase(phase);
            animator.SetTrigger("IDLE");
        }
    }
}