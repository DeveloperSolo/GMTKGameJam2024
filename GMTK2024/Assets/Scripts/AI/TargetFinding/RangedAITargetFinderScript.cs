using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAITargetFinderScript : BaseAITargetFinderScript
{
    [Header("Stats")]
    [SerializeField] private float range;
    [SerializeField] private LayerMask layerMask;

    private void Update()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, range, Vector2.up, 0.0f, layerMask.value);
        target = null;
        float minDistance = 0.0f;
        foreach(RaycastHit2D hit in hits)
        {
            float distance = (hit.collider.transform.position - transform.position).sqrMagnitude;
            if(target == null || distance < minDistance)
            {
                target = hit.rigidbody.gameObject;
                minDistance = distance;
            }
        }
    }

    public void SetRangeFromScaling(float value)
    {
        range = value;
    }
}
