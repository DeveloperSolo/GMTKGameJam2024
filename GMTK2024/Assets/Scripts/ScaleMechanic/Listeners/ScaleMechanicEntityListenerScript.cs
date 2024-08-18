using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ScaleMechanicEntityListenerScript : ScaleMechanicListenerScript
{
    private ScaleMechanicBoxColliderListenerScript colliderListener;
    private ScaleMechanicRectTransformListenerScript canvasListener;
    private ScaleMechanicSpriteListenerScript spriteListener;

    private const float maxDeletionSize = 0.1f;

    private void Awake()
    {
        colliderListener = GetComponent<ScaleMechanicBoxColliderListenerScript>();
        canvasListener = GetComponentInChildren<ScaleMechanicRectTransformListenerScript>();
        spriteListener = GetComponentInChildren<ScaleMechanicSpriteListenerScript>();
    }

    protected override void OnScaleEnd()
    {
        base.OnScaleEnd();

        transform.position = new Vector3(Source.transform.position.x, Source.transform.position.y, transform.position.z);
        Source.transform.localPosition = new Vector3(0.0f, 0.0f, Source.transform.localPosition.z);

        if((Source.CurrentSize.x * Source.CurrentSize.y) < maxDeletionSize)
        {
            gameObject.SetActive(false);
        }
    }
}
