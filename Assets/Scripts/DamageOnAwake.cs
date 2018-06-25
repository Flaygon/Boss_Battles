using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnAwake : MonoBehaviour
{
    public int damage;
    public float radius;

    public ContactFilter2D filter;

    private void Start()
    {
        Collider2D[] results = new Collider2D[4];
        int numHits = Physics2D.OverlapCircle(transform.position, radius, filter, results);

        for(int iResult = 0; iResult < numHits; ++iResult)
        {
            Debug.Log(results[iResult].gameObject);
            Entity hitEntity = results[iResult].gameObject.GetComponent<Entity>();
            if (hitEntity)
                hitEntity.OnHit(damage);
        }
    }
}