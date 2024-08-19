using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [Header("Resources")]
    [SerializeField] private float startScaleResource;

    private float currentScaleResource;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scaleResourceText;

    [Header("Debugging")]
    [SerializeField] private bool isDebugMode;

    private static ResourceManager instance = null;
    public static ResourceManager Instance { get { return instance; } }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this);
            Debug.LogWarning(gameObject.name + " had an extra ResourceManager, which was deleted");
        }
    }

    private void Start()
    {
        ResetResources();
    }

    public bool TryGainOrSpendScaleResource(Vector2 scaleDiff, float gainMultiplier, float lossMultiplier, bool flipCost = false)
    {
        if(isDebugMode)
        {
            return true;
        }
        if(GameController.Instance.GetState() != GameState.Play)
        {
            return true;
        }
        float resourceDiff = -scaleDiff.x - scaleDiff.y;
        resourceDiff *= (flipCost) ? -1 : 1;
        resourceDiff *= (resourceDiff > 0) ? gainMultiplier : lossMultiplier;

        if (resourceDiff < 0.0f && Mathf.Abs(resourceDiff) > currentScaleResource)
        {
            return false;
        }
        UpdateScaleResource(currentScaleResource + resourceDiff);
        return true;
    }

    private void ResetResources()
    {
        UpdateScaleResource(startScaleResource);
    }

    private void UpdateScaleResource(float newAmount)
    {
        currentScaleResource = Mathf.Max(newAmount, 0);
        scaleResourceText.text = currentScaleResource.ToString("F1") + " scales left";
    }
}