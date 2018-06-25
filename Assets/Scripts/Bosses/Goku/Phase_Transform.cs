using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase_Transform : PhaseNode
{
    public float transformTime;
    protected float currentTransformTime;

    public AudioSource transformBeginAudio;
    public float transformLoopStartWait;
    private float currentTransformLoopStartWait;
    public AudioSource transformLoopAudio;

    private enum States
    {
        BEGIN,
        MIDDLE,
        END,
    }
    private States mainState = States.BEGIN;

    public override void OnBegin()
    {
        manager.animator.SetTrigger("TRANSFORM");

        transformBeginAudio.Play();
    }

    public override void UpdateNode()
    {
        manager.body.velocity = Vector2.zero;
        manager.body.gravityScale = 0.0f;

        currentTransformLoopStartWait += Time.deltaTime;
        if(currentTransformLoopStartWait >= transformLoopStartWait)
        {
            if (!transformLoopAudio.isPlaying)
                transformLoopAudio.Play();
        }

        currentTransformTime += Time.deltaTime;
        if (currentTransformTime >= transformTime)
        {
            currentTransformTime = 0.0f;

            triggered1 = true;
            manager.animator.SetTrigger("IDLE");

            transformLoopAudio.Stop();
        }
    }
}