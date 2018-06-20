using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HealthManager : MonoBehaviour
{
    public abstract void Init(int health);
    public abstract void SetHealth(int health);
}