using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public int damage;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Entity entity = collision.gameObject.GetComponent<Entity>();
        if(entity)
        {
            entity.OnHit(damage);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Entity entity = collider.gameObject.GetComponent<Entity>();
        if (entity)
        {
            entity.OnHit(damage);
        }
    }
}