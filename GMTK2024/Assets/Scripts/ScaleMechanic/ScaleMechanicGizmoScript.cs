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
        parent.StartDraggingGizmo(this);
        isDragging = true;
    }

    private void Update()
    {
        if(isDragging && Input.GetMouseButtonUp(0))
        {
            parent.EndDraggingGizmo(this);
            isDragging = false;
        }
    }
}

public enum ScaleMode
{
    None        = 0b0000,

    Top         = 0b0001,
    Left        = 0b0010,
    Bottom      = 0b0100,
    Right       = 0b1000,

    TopRight    = Top | Right,
    TopLeft     = Top | Left,
    BotLeft     = Bottom | Left,
    BotRight    = Bottom | Right,
}

static class ScaleModeMethods
{
    public static bool Contains(this ScaleMode scaleMode, ScaleMode other)
    {
        return (scaleMode & other) != ScaleMode.None;
    }
}