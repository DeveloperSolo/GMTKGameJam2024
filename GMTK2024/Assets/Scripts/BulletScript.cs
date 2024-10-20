using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletScript : MonoBehaviour
{
    [SerializeField] private float speed;

    private float lifeTime = 0.0f;

    private TrailRenderer trailRenderer;

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
            transform.localScale = owner.CurrentSize;
            
            if(trailRenderer == null)
                trailRenderer = GetComponentInChildren<TrailRenderer>();

            trailRenderer.widthMultiplier = 0.2f * Mathf.Max(owner.CurrentSize.x,owner.CurrentSize.y);
        }
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0.0f || GameController.Instance.IsOutOfGameArea(transform.position))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameObject.SetActive(false);
    }
}