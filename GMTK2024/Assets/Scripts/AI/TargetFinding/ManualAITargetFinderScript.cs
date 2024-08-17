using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualAITargetFinderScript : BaseAITargetFinderScript
{
    [SerializeField]
    private GameObject inspectorAssignedTarget;

    private void Awake()
    {
        target = inspectorAssignedTarget;
    }
}
