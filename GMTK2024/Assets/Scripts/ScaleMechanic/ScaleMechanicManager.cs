using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class ScaleMechanicManager : MonoBehaviour
{
    private List<ScaleMechanicComponent> components = new List<ScaleMechanicComponent>();

    private static ScaleMechanicManager instance;
    public static ScaleMechanicManager Instance { get { return instance; } }

    private ScaleMechanicComponent prevHoveredObject = null;
    private ScaleMechanicComponent selectedObject = null;
    private const float hoverDetectionBuffer = 0.05f;

    private const float timeScaleAdjustSpeed = 3.0f;
    private float targetTimeScale = 1.0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
            Debug.LogWarning(gameObject.name + " had an extra GameController, which was deleted");
        }
    }

    public void ResetHighlights()
    {
        UpdateHoveredObject(null);
        UpdateSelectedObject(null);
        SetTimeScale(1.0f, true);
    }

    private void Update()
    {
        UpdateHoveredObject(GetHoveredObject());

        if (Input.GetMouseButtonDown(0))
        {
            UpdateSelectedObject(prevHoveredObject);
        }
        else if(selectedObject != null && !selectedObject.gameObject.activeInHierarchy)
        {
            UpdateSelectedObject(null);
        }

        UpdateTimeScale();
    }

    public void AddComponent(ScaleMechanicComponent component)
    {
        components.Add(component);
        component.SetGizmoMode(ScaleGizmoMode.Hidden);
    }

    private void UpdateSelectedObject(ScaleMechanicComponent newSelectedObject)
    {
        if(selectedObject == newSelectedObject)
        {
            return;
        }

        if (selectedObject != null)
        {
            selectedObject.SetGizmoMode(ScaleGizmoMode.Hidden);

            if (GameController.Instance.GetState() == GameState.Play && selectedObject.transform.parent != null)
            {
                HighlightHelperScript highlightHelper = selectedObject.transform.parent.GetComponent<HighlightHelperScript>();
                highlightHelper?.ResetHighlight();
                SetTimeScale(1.0f);
            }
        }

        selectedObject = newSelectedObject;

        if (selectedObject != null)
        {
            selectedObject.SetGizmoMode(ScaleGizmoMode.Shown);

            if (GameController.Instance.GetState() == GameState.Play && selectedObject.transform.parent != null)
            {
                HighlightHelperScript highlightHelper = selectedObject.transform.parent.GetComponent<HighlightHelperScript>();
                highlightHelper?.Highlight();
                SetTimeScale(0.05f);
            }
        }
    }

    private void UpdateHoveredObject(ScaleMechanicComponent currentHoveredObject)
    {
        if(prevHoveredObject == currentHoveredObject)
        {
            return;
        }

        if(prevHoveredObject != null && prevHoveredObject != selectedObject)
        {
            prevHoveredObject.SetGizmoMode(ScaleGizmoMode.Hidden);
        }

        prevHoveredObject = currentHoveredObject;

        if (prevHoveredObject != null && prevHoveredObject != selectedObject)
        {
            prevHoveredObject.SetGizmoMode(ScaleGizmoMode.Minimal);
        }
    }

    private ScaleMechanicComponent GetHoveredObject()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        ScaleMechanicComponent hovered = null;
        float minSqrDistance = 0.0f;

        foreach (ScaleMechanicComponent component in components)
        {
            if(!component.gameObject.activeInHierarchy)
            {
                continue;
            }

            Vector2 pos = component.transform.position;
            Vector2 extents = component.CurrentSize / 2.0f;
            extents.x *= component.transform.lossyScale.x;
            extents.y *= component.transform.lossyScale.y;
            extents.x += hoverDetectionBuffer;
            extents.y += hoverDetectionBuffer;

            Vector2 boundsMin = pos - extents;
            Vector2 boundsMax = pos + extents;

            if(boundsMin.x <= mousePos.x && mousePos.x <= boundsMax.x
                && boundsMin.y <= mousePos.y && mousePos.y <= boundsMax.y)
            {
                if(component == selectedObject)
                {
                    return component;
                }

                float sqrDistance = (mousePos - pos).sqrMagnitude;
                if(hovered == null || sqrDistance < minSqrDistance)
                {
                    hovered = component;
                }
            }
        }

        return hovered;
    }

    private void SetTimeScale(float timeScale, bool immediate = false)
    {
        targetTimeScale = timeScale;

        if (immediate)
        {
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = 0.02f * timeScale;
        }
    }

    private void UpdateTimeScale()
    {
        float maxDiff = timeScaleAdjustSpeed * Time.unscaledDeltaTime;
        float minDiff = -maxDiff;
        float diff = Mathf.Clamp(targetTimeScale - Time.timeScale, minDiff, maxDiff);

        float nextTimeScale = Time.timeScale + diff;

        Time.timeScale = nextTimeScale;
        Time.fixedDeltaTime = 0.02f * nextTimeScale;
    }
}