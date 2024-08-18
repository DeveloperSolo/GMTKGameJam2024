using UnityEngine;
using UnityEngine.EventSystems;

public class ScaleMechanicGizmoScript : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private ScaleMode scaleMode;

    public ScaleMode ScaleMode { get { return scaleMode; } }

    private ScaleMechanicComponent parent = null;
    private bool isDragging = false;

    public void Initialize(ScaleMechanicComponent parent)
    {
        this.parent = parent;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        TryStartDrag();
    }

    private void Update()
    {
        if(isDragging && GameController.GetMouseButtonUp(MouseButton.Left))
        {
            EndDrag();
        }
    }

    private void TryStartDrag()
    {
        if (parent.IsDraggingGizmoEnabled)
        {
            parent.StartDraggingGizmo(this);
            isDragging = true;
        }
    }

    public void EndDrag()
    {
        parent.EndDraggingGizmo(this);
        isDragging = false;
    }
}