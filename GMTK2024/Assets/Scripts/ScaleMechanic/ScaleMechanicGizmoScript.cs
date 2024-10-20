using UnityEditor.Search;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScaleMechanicGizmoScript : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ScaleMode scaleMode;
    [SerializeField] private TweenScript tween;

    private new Collider2D collider;

    public ScaleMode ScaleMode { get { return scaleMode; } }

    private ScaleMechanicComponent parent = null;
    private bool isDragging = false;
    private bool isHighlighted = false;

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
    }

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
        //if (parent.IsDraggingGizmoEnabled)
        //{
            parent.StartDraggingGizmo(this);
            isDragging = true;
        //}
    }

    public void EndDrag()
    {
        parent.EndDraggingGizmo(this);
        isDragging = false;
    }

    public void SetInteractable(ScaleGizmoMode mode, bool syncVisuals = true)
    {
        switch (mode)
        {
            case ScaleGizmoMode.Hidden:
                SetInteractable(false);
                break;
            case ScaleGizmoMode.Shown:
                SetInteractable(true);
                break;
            case ScaleGizmoMode.Minimal:
                SetInteractable(false);
                break;
        }

        if (syncVisuals)
        {
            SetDisplay(mode);
        }
    }

    private void SetDisplay(ScaleGizmoMode mode)
    {
        switch(mode)
        {
            case ScaleGizmoMode.Hidden:
                SetVisible(false);
                break;
            case ScaleGizmoMode.Shown:
                SetVisible(true);
                break;
            case ScaleGizmoMode.Minimal:
                SetVisible(!ScaleMode.IsCombination());
                break;
        }
    }

    private void SetInteractable(bool isInteractable)
    {
        collider.enabled = isInteractable;

        if(!isInteractable && isDragging)
        {
            EndDrag();
        }
    }

    private void SetVisible(bool isVisible)
    {
        for(int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(isVisible);
        }
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