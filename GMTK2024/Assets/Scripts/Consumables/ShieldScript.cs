using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{
    private Transform originalParent;
    private bool isAttached = false;

    private void Reset()
    {
        isAttached = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        originalParent = transform.parent;
    }

    private void OnEnable()
    {
        transform.SetParent(originalParent);
        gameObject.layer = LayerMask.NameToLayer("Shield");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.gameObject.name + "has entered shield");
        if(collision.gameObject.GetComponent<ShieldScript>() != null)
        {
            return;
        }

        if (!isAttached)
        {
            //transform.position = collision.gameObject.transform.position;
            transform.SetParent(collision.gameObject.transform);
            gameObject.layer = collision.gameObject.layer;
            isAttached = true;
        }
    }
}
