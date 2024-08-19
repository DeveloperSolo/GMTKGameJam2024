using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletScript : MonoBehaviour
{
    [SerializeField] private float speed;

    public void Initialize(Vector2 dir, int damage, ScaleMechanicComponent owner)
    {
        transform.up = dir;
        GetComponent<Rigidbody2D>().velocity = dir * speed;
        DamageScript damageScript = GetComponent<DamageScript>();
        damageScript.SetDamageFromScaling(damage);

        if(owner != null)
        {
            damageScript.ScalableOwner = owner;
        }
    }

    private void Update()
    {
        if(GameController.Instance.IsOutOfGameArea(transform.position))
        {
            DestroySelf();
        }
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}