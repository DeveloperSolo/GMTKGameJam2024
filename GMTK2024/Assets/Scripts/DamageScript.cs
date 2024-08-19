using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageScript : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int damage;
    [SerializeField] private float knockbackStrength;
    [SerializeField] private LayerMask mask;

    [Header("Events")]
    [SerializeField] private UnityEvent onDamageEvent;
    [SerializeField] private UnityEvent onKillEvent;

    public UnityEvent OnDamage { get { return onDamageEvent; } }
    public UnityEvent OnKill { get { return onKillEvent; } }

    public ScaleMechanicComponent ScalableOwner { get; set; }


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
        if(mask != (mask | (1 << health.gameObject.layer)))
        {
            return;
        }

        health.TakeDamage(damage);
        if (health.IsAlive())
        {
            onDamageEvent?.Invoke();

            if (knockbackStrength > 0)
            {
                Rigidbody2D rbody = health.GetComponent<Rigidbody2D>();
                if (rbody != null)
                {
                    Vector2 dir = (rbody.transform.position - transform.position).normalized;
                    rbody.AddForce(dir * knockbackStrength);
                }
            }
        }
        else
        {
            onKillEvent?.Invoke();

            if(ScalableOwner != null)
            {
                ScalableOwner.StealSizeFrom(health.ScalableOwner);
            }
        }
    }

    public void GetValueForInfoDisplay(EntityInfoScript.Info info)
    {
        info.InfoValue = damage.ToString();
    }

    public void SetDamageFromScaling(float newDamage)
    {
        damage = Mathf.FloorToInt(newDamage);
    }
}