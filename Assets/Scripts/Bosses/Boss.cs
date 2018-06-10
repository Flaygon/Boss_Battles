using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public LevelManager levelManager;

    public float playerRange;

    public Image healthBar;
    [HideInInspector]
    public int currentHealth;

    public Phase[] phases;
    [HideInInspector]
    public int currentPhase = 0;

    private void Start()
    {
        SetPhase(currentPhase);
    }

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
        phases[currentPhase].OnHit(damage);

        healthBar.fillAmount = currentHealth / (float)phases[currentPhase].health;
    }

    public void SetPhase(int phase)
    {
        // Clear dependencies on previous phase
        phases[currentPhase].running = false;

        currentPhase = phase;

        // Make new dependencies on next phase
        phases[currentPhase].running = true;
        currentHealth = phases[currentPhase].health;
    }

    public Phase GetCurrentPhase()
    {
        return phases[currentPhase];
    }
}