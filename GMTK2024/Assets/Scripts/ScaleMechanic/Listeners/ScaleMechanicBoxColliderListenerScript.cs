using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleMechanicBoxColliderListenerScript : ScaleMechanicListenerScript
{
    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    protected override void OnScaleStart()
    {
        base.OnScaleStart();
        if(!boxCollider2D.isTrigger)
        {
            boxCollider2D.enabled = false;
        }
    }

    protected override void OnScaleUpdate(Vector2 newPosition, Vector2 newSize)
    {
        boxCollider2D.offset = newPosition;
        boxCollider2D.size = newSize;
    }

    protected override void OnScaleEnd()
    {
        boxCollider2D.offset = Vector2.zero;
        if (!boxCollider2D.isTrigger)
        {
            boxCollider2D.enabled = true;
        }
    }
}
