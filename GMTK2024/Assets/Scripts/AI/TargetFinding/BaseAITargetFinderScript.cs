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
}
