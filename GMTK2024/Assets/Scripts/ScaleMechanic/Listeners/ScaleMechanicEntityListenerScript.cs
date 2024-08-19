using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScaleMechanicEntityListenerScript : ScaleMechanicListenerScript
{
    [SerializeField] private List<EntityStatScaling> statScalings;
    private const float maxDeletionSize = 0.1f;

    protected override void OnScaleUpdate(Vector2 newPosition, Vector2 newSize)
    {
        base.OnScaleUpdate(newPosition, newSize);

        foreach(EntityStatScaling statScaling in statScalings)
        {
            statScaling.SetStat(this);
        }
    }

    protected override void OnScaleEnd()
    {
        base.OnScaleEnd();

        transform.position = new Vector3(Source.transform.position.x, Source.transform.position.y, transform.position.z);
        Source.transform.localPosition = new Vector3(0.0f, 0.0f, Source.transform.localPosition.z);

        if((Source.CurrentSize.x * Source.CurrentSize.y) < maxDeletionSize)
        {
            HealthScript health = GetComponent<HealthScript>();
            if(health != null)
            {
                health.Kill();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}

[System.Serializable]
public struct EntityStatScaling
{
    [SerializeField] private string statName;
    [SerializeField] private float startValue;
    [SerializeField] private float statPerScale;
    [SerializeField] private UnityEvent<float> StatSetter;

    public void SetStat(ScaleMechanicEntityListenerScript entityListener)
    {
        SetStat(entityListener.Source.StartSize, entityListener.Source.CurrentSize);
    }

    public void SetStat(Vector2 startSize, Vector2 currentSize)
    {
        SetStat(startSize.x * startSize.y, currentSize.x * currentSize.y);
    }

    public void SetStat(float startSize, float currentSize)
    {
        float newStat = startValue + ((currentSize - 1) * statPerScale);
        StatSetter.Invoke(newStat);
    }
}