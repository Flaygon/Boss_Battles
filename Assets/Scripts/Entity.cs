using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public int health;
    [HideInInspector]
    public int currentHealth;

    [HideInInspector]
    public HealthManager healthManager;

    [HideInInspector]
    public bool incapacitated;

    protected virtual void Awake()
    {
        currentHealth = health;
    }

    public virtual void OnHit(int damage)
    {
        currentHealth -= damage;

        healthManager.UpdateHealth(currentHealth, health, GetEntityIndex());

        if (currentHealth <= 0)
        {
            incapacitated = true;
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    protected abstract int GetEntityIndex();
}