using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleMechanicSpriteListenerScript : ScaleMechanicListenerScript
{
    private List<SpriteRenderer> sprites = new List<SpriteRenderer>();

    protected override void Awake()
    {
        base.Awake();
        sprites.AddRange(GetComponentsInChildren<SpriteRenderer>());
    }

    protected override void OnScaleUpdate(Vector2 newPosition, Vector2 newSize)
    {
        transform.localPosition = newPosition;
        foreach(SpriteRenderer sprite in sprites)
        {
            if(sprite.drawMode == SpriteDrawMode.Simple)
            {
                sprite.transform.localScale = newSize;
            }
            else
            {
                sprite.size = newSize;
            }
        }
    }

    protected override void OnScaleEnd()
    {
        transform.localPosition = Vector2.zero;
    }
}
