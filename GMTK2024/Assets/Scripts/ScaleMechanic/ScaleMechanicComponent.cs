using System.Collections.Generic;
using UnityEngine;

public class ScaleMechanicComponent : MonoBehaviour
{
    [Header("Cost Settings")]
    [SerializeField] private bool flipCost = false;
    [SerializeField] private float gainResourceMultiplier = 1.0f;
    [SerializeField] private float lossResourceMultiplier = 1.0f;


    [Header("Scaling Settings")]
    [SerializeField] private bool alwaysPreserveAspectRatio = false;
    [SerializeField] private bool enableInputGrow = true;
    [SerializeField] private bool enableInputShrink = true;
    [SerializeField] private Vector2 startSize = Vector2.one;

    private Vector2 prevSetSize = Vector2.one;
    private Vector2 currentSize = Vector2.one;
    private Vector2 pivotPoint = DefaultPivotPoint;

    public Vector2 StartSize { get { return startSize; } }
    public Vector2 CurrentSize { get { return currentSize; } }
    private static readonly Vector2 DefaultPivotPoint = new Vector2(0.5f, 0.5f);

    [Header("Components")]
    [SerializeField] private SpriteRenderer border;
    [SerializeField] private List<Transform> draggablePoints;
    [SerializeField] private List<BoxCollider2D> draggableEdges;

    private ScaleMechanicGizmoScript currentDraggingGizmo = null;
    private bool isDraggingGizmoEnabled = true;
    private bool isManuallyScaling = false;
    private List<ScaleMechanicListenerScript> listeners = new List<ScaleMechanicListenerScript>();
    private List<ScaleMechanicListenerScript> listenersToRemove = new List<ScaleMechanicListenerScript>();

    public bool IsDraggingGizmoEnabled { get { return isDraggingGizmoEnabled; } }
    public bool IsManuallyScaling { get { return isManuallyScaling; } }

    private void Awake()
    {
        InitializeGizmoScripts();

        HealthScript health = GetComponentInParent<HealthScript>();
        if(health != null)
        {
            health.ScalableOwner = this;
        }
        DamageScript damage = GetComponentInParent<DamageScript>();
        if(damage != null)
        {
            damage.ScalableOwner = this;
        }
    }

    private void OnEnable()
    {
        StartManualUpdateSize();
        ManualUpdateSize(ScaleMode.None, startSize);
        EndManualUpdateSize();
    }

    private void Update()
    {
        UpdateDraggingGizmo();
        ProcessListenersToRemove();
    }

    #region Gizmos

    private void InitializeGizmoScripts()
    {
        foreach (Transform point in draggablePoints)
        {
            point.GetComponent<ScaleMechanicGizmoScript>().Initialize(this);
        }
        foreach (BoxCollider2D edge in draggableEdges)
        {
            edge.GetComponent<ScaleMechanicGizmoScript>().Initialize(this);
        }
    }

    public bool IsDraggingGizmo()
    {
        return currentDraggingGizmo != null;
    }

    public void EnableDraggingGizmo()
    {
        isDraggingGizmoEnabled = true;
        gameObject.SetActive(true);
    }

    public void DisableDraggingGizmo()
    {
        isDraggingGizmoEnabled = false;
        if(IsDraggingGizmo())
        {
            currentDraggingGizmo.EndDrag();
        }
        gameObject.SetActive(false);
    }

    public void StartDraggingGizmo(ScaleMechanicGizmoScript gizmo)
    {
        if(IsDraggingGizmoEnabled)
        {
            currentDraggingGizmo = gizmo;
            SendEvent(ScaleMechanicEvent.EventType.Start);
        }
    }

    private void UpdateDraggingGizmo()
    {
        if (currentDraggingGizmo == null)
        {
            return;
        }

        Vector2 currPosition = currentDraggingGizmo.transform.position;
        Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition = GameController.Instance.SmartClampToGameArea(targetPosition, Vector2.zero);
        Vector2 posDiff = targetPosition - currPosition;

        ScaleMode scaleMode = currentDraggingGizmo.ScaleMode;
        UpdateSizeFromGizmoDragging(scaleMode, posDiff, Input.GetKey(KeyCode.LeftShift));
    }

    public void EndDraggingGizmo(ScaleMechanicGizmoScript gizmo)
    {
        if(gizmo != currentDraggingGizmo)
        {
            return;
        }
        currentDraggingGizmo = null;
        prevSetSize = currentSize;
        HighlightGizmos(ScaleMode.None);
        SendEvent(ScaleMechanicEvent.EventType.End);
    }

    private void UpdateSizeFromGizmoDragging(ScaleMode scaleMode, Vector2 mouseDelta, bool preserveAspectRatio)
    {
        Vector2 newSize = currentSize + GetSizeChangeFromDragging(scaleMode, mouseDelta);
        if (alwaysPreserveAspectRatio || preserveAspectRatio)
        {
            newSize = GetPreservedAspectRatio(scaleMode, newSize);
        }

        newSize.x = Mathf.Max(newSize.x, 0.0f);
        newSize.y = Mathf.Max(newSize.y, 0.0f);

        if (ResourceManager.Instance.TryGainOrSpendScaleResource(newSize - currentSize, gainResourceMultiplier, lossResourceMultiplier, flipCost))
        {
            UpdateSize(scaleMode, newSize);
        }
    }

    public void HighlightGizmos(ScaleMode highlightMode)
    {
        bool isCombination = highlightMode.IsCombination();
        foreach(Transform point in draggablePoints)
        {
            ScaleMechanicGizmoScript pointGizmo = point.GetComponent<ScaleMechanicGizmoScript>();
            if(pointGizmo == null)
            {
                continue;
            }

            bool shouldHighlight = (isCombination) ? highlightMode == pointGizmo.ScaleMode : pointGizmo.ScaleMode.Contains(highlightMode);
            if (shouldHighlight)
            {
                pointGizmo.StartHighlight();
            }
            else
            {
                pointGizmo.StopHighlight();
            }
        }
    }

    #endregion Gizmos

    #region Manipulators

    public void StartManualUpdateSize()
    {
        SendEvent(ScaleMechanicEvent.EventType.Start);
        isManuallyScaling = true;
    }

    public void EndManualUpdateSize()
    {
        SendEvent(ScaleMechanicEvent.EventType.End);
        isManuallyScaling = false;
        prevSetSize = currentSize;
    }

    public void ManualUpdateSize(ScaleMode scaleMode, Vector2 newSize)
    {
        UpdateSize(scaleMode, newSize);
    }

    public void StealSizeFrom(ScaleMechanicComponent other)
    {
        if(other == null)
        {
            return;
        }

        StartManualUpdateSize();
        ManualUpdateSize(ScaleMode.None, currentSize + (other.CurrentSize * 0.01f));
        EndManualUpdateSize();
    }

    public void UpdateSizeFromManipulator(ScaleMode scaleMode, float addedAmount)
    {
        float newSizeAmount = (currentSize.x * currentSize.y) + addedAmount;
        float aspectRatio = currentSize.x / currentSize.y;

        Vector2 newSize = Vector2.zero;
        newSize.y = Mathf.Sqrt(newSizeAmount / aspectRatio);
        newSize.x = newSize.y * aspectRatio;

        ManualUpdateSize(scaleMode, newSize);
    }

    #endregion Manipulators

    #region Listeners

    public void RegisterListener(ScaleMechanicListenerScript listener)
    {
        if (!HasListener(listener))
        {
            listeners.Add(listener);
        }
    }

    public bool HasListener(ScaleMechanicListenerScript listener)
    {
        return listeners.Contains(listener);
    }

    public void RemoveListener(ScaleMechanicListenerScript listener)
    {
        listenersToRemove.Add(listener);
    }

    private void ProcessListenersToRemove()
    {
        foreach(ScaleMechanicListenerScript listener in listenersToRemove)
        {
            listeners.Remove(listener);
        }
        listenersToRemove.Clear();
    }

    private void SendEvent(ScaleMechanicEvent.EventType type)
    {
        ScaleMechanicEvent ev = new ScaleMechanicEvent(type, transform.localPosition, currentSize);
        foreach (ScaleMechanicListenerScript listener in listeners)
        {
            listener.Recieve(ev);
        }
    }

    #endregion Listeners

    #region Size/Scaling

    private void UpdateSize(ScaleMode scaleMode, Vector2 newSize)
    {
        SetPivotPoint(scaleMode);

        UpdateSize(newSize);
        UpdateSizeVisuals();

        ResetPivotPoint();
    }

    private void UpdateSize(Vector2 newSize)
    {
        newSize.x = Mathf.Max(newSize.x, 0.0f);
        newSize.y = Mathf.Max(newSize.y, 0.0f);

        Vector2 position = transform.localPosition;
        position += (DefaultPivotPoint - pivotPoint) * (newSize - currentSize);

        transform.localPosition = new Vector3(position.x, position.y, transform.localPosition.z);
        currentSize = newSize;
        SendEvent(ScaleMechanicEvent.EventType.Update);
    }

    private Vector2 GetSizeChangeFromDragging(ScaleMode scaleMode, Vector2 mouseDelta)
    {
        Vector2 sizeChange = mouseDelta;
        if (scaleMode.Contains(ScaleMode.Bottom))
        {
            sizeChange.y *= -1;
        }
        else if (!scaleMode.Contains(ScaleMode.Top))
        {
            sizeChange.y = 0.0f;
        }
        if (scaleMode.Contains(ScaleMode.Left))
        {
            sizeChange.x *= -1.0f;
        }
        else if (!scaleMode.Contains(ScaleMode.Right))
        {
            sizeChange.x = 0.0f;
        }

        if (!enableInputGrow)
        {
            sizeChange.x = Mathf.Min(sizeChange.x, 0);
            sizeChange.y = Mathf.Min(sizeChange.y, 0);
        }
        if (!enableInputShrink)
        {
            sizeChange.x = Mathf.Max(sizeChange.x, 0);
            sizeChange.y = Mathf.Max(sizeChange.y, 0);
        }
        return sizeChange;
    }

    private Vector2 GetPreservedAspectRatio(ScaleMode scaleMode, Vector2 newSize)
    {
        float prevAspectRatio = prevSetSize.x / prevSetSize.y;

        if (scaleMode.Contains(ScaleMode.Top | ScaleMode.Bottom) && !scaleMode.Contains(ScaleMode.Left | ScaleMode.Right))
        {
            newSize.x = prevAspectRatio * newSize.y;
        }
        else if (scaleMode.Contains(ScaleMode.Left | ScaleMode.Right) && !scaleMode.Contains(ScaleMode.Top | ScaleMode.Bottom))
        {
            newSize.y = newSize.x / prevAspectRatio;
        }
        else
        {
            float newAspectRatio = newSize.x / newSize.y;
            if(newAspectRatio > prevAspectRatio)
            {
                newSize.y = newSize.x / prevAspectRatio;
            }
            else
            {
                newSize.x = prevAspectRatio * newSize.y;
            }
        }
        return newSize;
    }

    private void UpdateSizeVisuals()
    {
        border.size = currentSize;

        Vector2 padding = new Vector2(0.05f, 0.05f);
        Vector2 extents = (currentSize / 2.0f) - padding;

        DraggablePoint_TopRight.localPosition = extents;
        DraggablePoint_TopLeft.localPosition = extents * new Vector2(-1, 1);
        DraggablePoint_BotLeft.localPosition = extents * -1;
        DraggablePoint_BotRight.localPosition = extents * new Vector2(1, -1);

        Transform examplePoint = DraggablePoint_TopRight;
        Vector2 EdgeSize = (examplePoint.localPosition - (examplePoint.localScale / 2.0f)) * 2.0f;

        DraggableEdge_Top.transform.localPosition = new Vector2(0.0f, extents.y);
        DraggableEdge_Top.size = new Vector2(EdgeSize.x, DraggableEdge_Top.size.y);

        DraggableEdge_Left.transform.localPosition = new Vector2(-extents.x, 0.0f);
        DraggableEdge_Left.size = new Vector2(DraggableEdge_Left.size.x, EdgeSize.y);

        DraggableEdge_Bot.transform.localPosition = new Vector2(0.0f, -extents.y);
        DraggableEdge_Bot.size = new Vector2(EdgeSize.x, DraggableEdge_Bot.size.y);

        DraggableEdge_Right.transform.localPosition = new Vector2(extents.x, 0.0f);
        DraggableEdge_Right.size = new Vector2(DraggableEdge_Right.size.x, EdgeSize.y);
    }

    public void GetValueForInfoDisplay(EntityInfoScript.Info info)
    {
        int roundedValue = Mathf.FloorToInt(currentSize.x * currentSize.y * 10);
        float value = (float)roundedValue / 10.0f;
        if(value >= 1.0f)
        {
            info.InfoValue = Mathf.FloorToInt(value).ToString();
        }
        else
        {
            info.InfoValue = value.ToString();
        }
    }

    #endregion Size/Scaling

    #region PivotPoint

    private void SetPivotPoint(ScaleMode scaleMode)
    {
        if (scaleMode.Contains(ScaleMode.Top))
        {
            pivotPoint.y = 0.0f;
        }
        else if (scaleMode.Contains(ScaleMode.Bottom))
        {
            pivotPoint.y = 1.0f;
        }
        if (scaleMode.Contains(ScaleMode.Left))
        {
            pivotPoint.x = 1.0f;
        }
        else if (scaleMode.Contains(ScaleMode.Right))
        {
            pivotPoint.x = 0.0f;
        }
    }

    private void ResetPivotPoint()
    {
        pivotPoint = DefaultPivotPoint;
    }

    #endregion PivotPoint

    #region Property Shortcuts

    private Transform DraggablePoint_TopRight
    {
        get { return (draggablePoints.Count >= 1) ? draggablePoints[0] : null; }
    }
    private Transform DraggablePoint_TopLeft
    {
        get { return (draggablePoints.Count >= 2) ? draggablePoints[1] : null; }
    }
    private Transform DraggablePoint_BotLeft
    {
        get { return (draggablePoints.Count >= 3) ? draggablePoints[2] : null; }
    }
    private Transform DraggablePoint_BotRight
    {
        get { return (draggablePoints.Count >= 4) ? draggablePoints[3] : null; }
    }

    private BoxCollider2D DraggableEdge_Top
    {
        get { return (draggableEdges.Count >= 1) ? draggableEdges[0] : null; }
    }
    private BoxCollider2D DraggableEdge_Left
    {
        get { return (draggableEdges.Count >= 2) ? draggableEdges[1] : null; }
    }
    private BoxCollider2D DraggableEdge_Bot
    {
        get { return (draggableEdges.Count >= 3) ? draggableEdges[2] : null; }
    }
    private BoxCollider2D DraggableEdge_Right
    {
        get { return (draggableEdges.Count >= 4) ? draggableEdges[3] : null; }
    }

    #endregion Property Shortcuts
}

public enum ScaleMode
{
    None = 0b0000,

    Top = 0b0001,
    Left = 0b0010,
    Bottom = 0b0100,
    Right = 0b1000,

    TopRight = Top | Right,
    TopLeft = Top | Left,
    BotLeft = Bottom | Left,
    BotRight = Bottom | Right,
}

static class ScaleModeMethods
{
    public static bool Contains(this ScaleMode scaleMode, ScaleMode other)
    {
        return (scaleMode & other) != ScaleMode.None;
    }

    public static bool IsCombination(this ScaleMode scaleMode)
    {
        bool hasOnlyOneBit = (scaleMode & (scaleMode - 1)) == ScaleMode.None;
        return scaleMode != ScaleMode.None && !hasOnlyOneBit;
    }
}