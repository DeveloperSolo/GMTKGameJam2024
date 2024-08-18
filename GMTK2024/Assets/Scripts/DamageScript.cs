using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageScript : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int damage;
    [SerializeField] private float knockbackStrength;

    [Header("Events")]
    [SerializeField] private UnityEvent onDamagedEvent;
    [SerializeField] private UnityEvent onDeathEvent;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HealthScript health = collision.gameObject.GetComponent<HealthScript>();
        if (health != null)
        {
            DealDamage(health);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HealthScript health = collision.GetComponent<HealthScript>();
        if (health != null)
        {
            DealDamage(health);
        }
    }

    private void DealDamage(HealthScript health)
    {
        health.TakeDamage(damage);
        if (health.IsAlive())
        {
            onDamagedEvent?.Invoke();
        }
        else
        {
            onDeathEvent?.Invoke();
        }

        if(knockbackStrength > 0)
        {
            Rigidbody2D rbody = health.GetComponent<Rigidbody2D>();
            if(rbody != null)
            {
                Vector2 dir = (rbody.transform.position - transform.position).normalized;
                rbody.AddForce(dir * knockbackStrength);
            }
        }
    }
}