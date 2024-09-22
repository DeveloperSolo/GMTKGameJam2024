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
            if(health != null && health.enabled)
            {
                health.Kill();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    public float GetScaleForStat(string statName, float targetStat)
    {
        foreach (EntityStatScaling statScaling in statScalings)
        {
            if(statScaling.StatName != statName)
            {
                continue;
            }

            return statScaling.GetSizeFromStat(this, targetStat);
        }
        return 0.0f;
    }
}

[System.Serializable]
public struct EntityStatScaling
{
    [SerializeField] private string statName;
    [SerializeField] private float startValue;
    [SerializeField] private UnityEvent<float> StatSetter;

    public string StatName { get { return statName; } }

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
        float statPerScale = startValue / startSize;

        float newStat = startValue + ((currentSize - startSize) * statPerScale);
        StatSetter.Invoke(newStat);
    }

    public float GetSizeFromStat(ScaleMechanicEntityListenerScript entityListener, float targetStat)
    {
        return GetSizeFromStat(entityListener.Source.StartSize, targetStat);
    }

    public float GetSizeFromStat(Vector2 startSize, float targetStat)
    {
        return GetSizeFromStat(startSize.x * startSize.y, targetStat);
    }

    public float GetSizeFromStat(float startSize, float targetStat)
    {
        float statPerScale = startValue / startSize;
        return ((targetStat - startValue) / statPerScale) + startSize;
    }
}