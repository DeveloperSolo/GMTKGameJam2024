using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleMechanicSpriteListenerScript : ScaleMechanicListenerScript
{
    protected override void OnScaleUpdate(Vector2 newPosition, Vector2 newSize)
    {
        transform.localPosition = newPosition;
        transform.localScale = newSize;
    }

    protected override void OnScaleEnd()
    {
        transform.localPosition = Vector2.zero;
    }
}
