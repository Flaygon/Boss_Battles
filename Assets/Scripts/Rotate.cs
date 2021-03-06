﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 rotation;

    private void Update()
    {
        transform.Rotate(rotation.x * Time.deltaTime, rotation.y * Time.deltaTime, rotation.z * Time.deltaTime);
    }
}