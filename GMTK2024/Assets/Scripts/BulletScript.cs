using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletScript : MonoBehaviour
{
    [SerializeField] private float speed;

    public void Initialize(Vector2 dir)
    {
        transform.forward = dir;
        GetComponent<Rigidbody2D>().velocity = dir * speed;
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