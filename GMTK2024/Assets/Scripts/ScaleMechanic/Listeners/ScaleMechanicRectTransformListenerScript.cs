using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleMechanicRectTransformListenerScript : ScaleMechanicListenerScript
{
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    protected override void OnScaleUpdate(Vector2 newPosition, Vector2 newSize)
    {
        rectTransform.localPosition = newPosition;
        rectTransform.sizeDelta = newSize * 100.0f;
    }

    protected override void OnScaleEnd()
    {
        rectTransform.localPosition = Vector2.zero;
    }
}
