using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerNum;

    public bool dummy;

    [HideInInspector]
    public LevelManager levelManager;

    public float GetAxis(string axis)
    {
        return !dummy ? Input.GetAxis(axis + " " + playerNum) : 0.0f;
    }

    public bool GetPositiveAxis(string axis, float threshold)
    {
        return !dummy ? GetAxis(axis) > threshold : false;
    }

    public bool GetNegativeAxis(string axis, float threshold)
    {
        return !dummy ? GetAxis(axis) < threshold : false;
    }
}