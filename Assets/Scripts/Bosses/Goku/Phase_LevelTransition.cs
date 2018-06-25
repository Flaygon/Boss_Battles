using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase_LevelTransition : PhaseNode
{
    public float beginTransitionTime;
    protected float currentBeginTransitionTime;

    public float middleTransitionTime;
    protected float currentMiddleTransitionTime;

    public float endTransitionTime;
    protected float currentEndTransitionTime;

    public GameObject beginAsset;
    public GameObject endAsset;

    public int levelToTransitionTo;

    private enum States
    {
        BEGIN,
        MIDDLE,
        END,
    }
    private States mainState = States.BEGIN;

    public override void OnBegin()
    {
        manager.animator.SetTrigger("BEGIN");

        if (beginAsset != null)
            Instantiate(beginAsset);
    }

    public override void UpdateNode()
    {
        manager.body.velocity = Vector2.zero;
        manager.body.gravityScale = 0.0f;

        switch(mainState)
        {
            case States.BEGIN:
                currentBeginTransitionTime += Time.deltaTime;
                if (currentBeginTransitionTime >= beginTransitionTime)
                {
                    currentBeginTransitionTime = 0.0f;

                    mainState = States.MIDDLE;
                    manager.animator.SetTrigger("MIDDLE");

                    manager.levelManager.TransitionToLevel(levelToTransitionTo, middleTransitionTime);
                }
                break;
            case States.MIDDLE:
                currentMiddleTransitionTime += Time.deltaTime;
                if (currentMiddleTransitionTime >= middleTransitionTime)
                {
                    currentMiddleTransitionTime = 0.0f;

                    mainState = States.END;
                    manager.animator.SetTrigger("END");

                    if (endAsset != null)
                        Instantiate(endAsset);
                }
                break;
            case States.END:
                currentEndTransitionTime += Time.deltaTime;
                if (currentEndTransitionTime >= endTransitionTime)
                {
                    currentEndTransitionTime = 0.0f;

                    triggered1 = true;
                    manager.animator.SetTrigger("IDLE");
                }
                break;
        }
    }
}