using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleMechanicManipulator : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] ScaleMode scaleMode;
    [SerializeField] private float scaleGrowthAmount = 1.0f;
    [SerializeField] private float scaleGrowthDuration = 1.0f;
    [SerializeField] private AnimationCurve scaleGrowthCurve;
    [SerializeField] private float scaleGrowthLoopDelay;

    [Header("Components")]
    [SerializeField] private ScaleMechanicComponent target;
    [SerializeField] private GameObject growthSource;

    private float totalElapsed = 0.0f;
    private bool isGrowing = false;

    private void Awake()
    {
        if(target == null)
        {
            Destroy(gameObject);
            Debug.LogError(name + " has ScaleMechanicManipulator but no ScaleMechanicComponent assigned, it has been removed");
        }
    }

    private void Update()
    {
        if(growthSource != null && !growthSource.activeSelf)
        {
            // source has been removed
            TryStopGrowing();
            return;
        }

        if (target.IsDraggingGizmo())
        {
            // player is dragging it
            TryStopGrowing();
            totalElapsed = scaleGrowthDuration;
            return;
        }

        float elapsed = Time.deltaTime;
        totalElapsed += elapsed;

        if(totalElapsed >= scaleGrowthDuration + scaleGrowthLoopDelay)
        {
            // done looping
            totalElapsed -= scaleGrowthDuration + scaleGrowthLoopDelay;
        }
        else if (totalElapsed >= scaleGrowthDuration)
        {
            // waiting for delay to loop again
            TryStopGrowing();
            return;
        }

        TryStartGrowing();
        TryUpdateGrowing(elapsed);
    }

    private void TryStartGrowing()
    {
        if(isGrowing)
        {
            return;
        }
        //target.DisableDraggingGizmo();
        target.StartManualUpdateSize();
        isGrowing = true;
    }

    private void TryStopGrowing()
    {
        if(!isGrowing)
        {
            return;
        }
        //target.EnableDraggingGizmo();
        target.EndManualUpdateSize();
        isGrowing = false;
    }

    private void TryUpdateGrowing(float elapsed)
    {
        if(!isGrowing)
        {
            return;
        }

        float prevValue = scaleGrowthCurve.Evaluate((totalElapsed - elapsed) / scaleGrowthDuration);
        float currValue = scaleGrowthCurve.Evaluate(totalElapsed / scaleGrowthDuration);
        target.UpdateSizeFromManipulator(scaleMode, (currValue - prevValue) * scaleGrowthAmount);
    }
}