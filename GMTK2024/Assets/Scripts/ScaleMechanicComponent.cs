using System.Collections.Generic;
using UnityEngine;

public class ScaleMechanicComponent : MonoBehaviour
{
    [Header("Gameplay Variables")]
    [SerializeField]
    private Vector2 currentSize = Vector2.one;
    [SerializeField]
    private Vector2 pivotPoint = DefaultPivotPoint;

    private static readonly Vector2 DefaultPivotPoint = new Vector2(0.5f, 0.5f);

    [Header("Components")]
    [SerializeField] private SpriteRenderer border;
    [SerializeField] private List<Transform> draggablePoints;
    [SerializeField] private List<BoxCollider2D> draggableEdges;

    private ScaleMechanicGizmoScript currentDraggingGizmo = null;

    private void Start()
    {
        InitializeGizmoScripts();
    }

    private void Update()
    {
        UpdateDraggingGizmo();
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

    public void StartDraggingGizmo(ScaleMechanicGizmoScript gizmo)
    {
        currentDraggingGizmo = gizmo;
    }

    private void UpdateDraggingGizmo()
    {
        if (currentDraggingGizmo == null)
        {
            return;
        }

        Vector2 currPosition = currentDraggingGizmo.transform.position;
        Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 posDiff = targetPosition - currPosition;

        ScaleMode scaleMode = currentDraggingGizmo.ScaleMode;
        UpdateSize(scaleMode, posDiff);
    }

    public void EndDraggingGizmo(ScaleMechanicGizmoScript gizmo)
    {
        if(gizmo != currentDraggingGizmo)
        {
            return;
        }
        currentDraggingGizmo = null;
    }

    #endregion Gizmos

    #region Size/Scaling

    private void UpdateSize(ScaleMode scaleMode, Vector2 sizeChange)
    {
        SetPivotPoint(scaleMode);
        UpdateSize(currentSize + CullSizeChange(scaleMode, sizeChange));
        UpdateSizeVisuals();
        ResetPivotPoint();
    }

    private void UpdateSize(Vector2 newSize)
    {
        Vector2 position = transform.localPosition;
        position += (DefaultPivotPoint - pivotPoint) * (newSize - currentSize);

        transform.localPosition = new Vector3(position.x, position.y, transform.localPosition.z);
        currentSize = newSize;
    }

    private Vector2 CullSizeChange(ScaleMode scaleMode, Vector2 sizeChange)
    {
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
        return sizeChange;
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
