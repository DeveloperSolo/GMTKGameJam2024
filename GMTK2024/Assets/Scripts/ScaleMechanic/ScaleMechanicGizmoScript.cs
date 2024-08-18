using UnityEngine;
using UnityEngine.EventSystems;

public class ScaleMechanicGizmoScript : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ScaleMode scaleMode;
    [SerializeField] private TweenScript tween;

    public ScaleMode ScaleMode { get { return scaleMode; } }

    private ScaleMechanicComponent parent = null;
    private bool isDragging = false;
    private bool isHighlighted = false;

    public void Initialize(ScaleMechanicComponent parent)
    {
        this.parent = parent;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!parent.IsDraggingGizmo())
        {
            parent.HighlightGizmos(scaleMode);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!parent.IsDraggingGizmo())
        {
            parent.HighlightGizmos(ScaleMode.None);
        }
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

    public void StartHighlight()
    {
        if(tween == null || isHighlighted)
        {
            return;
        }
        tween.StopAll();
        tween.Play("StartHighlight");
        isHighlighted = true;
    }

    public void StopHighlight()
    {
        if (tween == null || !isHighlighted)
        {
            return;
        }
        tween.StopAll();
        tween.Play("StopHighlight");
        isHighlighted = false;
    }
}