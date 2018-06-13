using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public LevelManager levelManager;
    public PhaseManager phaseManager;

    public float playerRange;

    public Image healthBar;
    [HideInInspector]
    public int currentHealth;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Damage damage = collision.gameObject.GetComponent<Damage>();
        if (damage)
        {
            OnHit(damage.damage);
        }
    }

    public void OnHit(int damage)
    {
        phaseManager.OnHit(damage);

        SetHealth(currentHealth);
    }

    public void SetHealth(int health)
    {
        currentHealth = health;

        healthBar.fillAmount = currentHealth / (float)phaseManager.currentPhase.health;
    }
}