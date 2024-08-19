using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletScript : MonoBehaviour
{
    [SerializeField] private float speed;

    private float lifeTime = 0.0f;

    public void Initialize(Vector2 dir, int damage, float range, ScaleMechanicComponent owner)
    {
        transform.up = dir;
        GetComponent<Rigidbody2D>().velocity = dir * speed;
        DamageScript damageScript = GetComponent<DamageScript>();
        damageScript.SetDamageFromScaling(damage);

        lifeTime = range / speed;

        if (owner != null)
        {
            damageScript.ScalableOwner = owner;
        }
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0.0f || GameController.Instance.IsOutOfGameArea(transform.position))
        {
            DestroySelf();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DestroySelf();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DestroySelf();
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}