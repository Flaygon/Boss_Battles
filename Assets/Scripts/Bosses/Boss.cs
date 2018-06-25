using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : Entity
{
    public LevelManager levelManager;
    public PhaseManager phaseManager;

    public float playerRange;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Damage damage = collision.gameObject.GetComponent<Damage>();
        if (damage)
        {
            phaseManager.OnHit(damage.damage);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Damage damage = collider.gameObject.GetComponent<Damage>();
        if (damage)
        {
            phaseManager.OnHit(damage.damage);
        }
    }

    public override void OnHit(int damage)
    {
        phaseManager.OnHit(damage);
    }

    protected override int GetEntityIndex()
    {
        return 0;
    }
}