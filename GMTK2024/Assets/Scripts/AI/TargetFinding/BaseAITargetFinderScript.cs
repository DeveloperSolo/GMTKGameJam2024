using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAITargetFinderScript : MonoBehaviour
{
    protected GameObject target = null;

    public GameObject GetTarget()
    {
        return target;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (target != null)
        {
            Gizmos.DrawSphere(target.transform.position, 0.25f);
        }
    }
}
