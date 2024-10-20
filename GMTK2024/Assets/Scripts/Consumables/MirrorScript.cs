using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MirrorScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInChildren<BulletScript>() != null)
        {
            // reflect the bullet's velocity.
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            Debug.Log(collision.name + " Velocity Before Reflection: " + rb.velocity);
            Vector2 curVel = rb.velocity;

            RaycastHit2D[] hits = Physics2D.RaycastAll(collision.transform.position, rb.velocity, 10.0f);
            Vector2 normal = Vector2.zero;
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.name == collision.name)
                    continue;

                normal = hit.normal.normalized;
                Debug.Log("raycast hit " + hit.transform.name);
                Debug.Log("raycast normal " + hit.normal);
                break;
            }

            //Vector3 dirVec = (transform.position - collision.transform.position).normalized;

            //float upV = Vector2.Dot(dirVec, transform.up);
            //float rV = Vector2.Dot(dirVec, transform.right);
            //Vector2 normal = Vector2.zero;
            //if (Mathf.Abs(upV) > Mathf.Abs(rV))
            //{
            //    normal = transform.up;
            //    if (upV < 0)
            //        normal *= -1;
            //}
            //else
            //{
            //    normal = transform.right;
            //    if (rV < 0)
            //        normal *= -1;
            //}

            rb.velocity = Vector2.Reflect(curVel, normal);
            Debug.Log(collision.name + " Velocity After Reflection: " + rb.velocity);
        }
    }
}
