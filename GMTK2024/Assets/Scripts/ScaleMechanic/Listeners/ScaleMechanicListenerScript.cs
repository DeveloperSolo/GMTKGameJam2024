using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleMechanicListenerScript : MonoBehaviour
{
    [SerializeField]
    private ScaleMechanicComponent source;

    //protected ScaleMechanicComponent Source { get { return source; } }

    private void Start()
    {
        source.RegisterListener(this);
    }
    private void OnDestroy()
    {
        source.RemoveListener(this);
    }
    private void OnEnable()
    {
        source.RemoveListener(this);
    }
    private void OnDisable()
    {
        source.RemoveListener(this);
    }

    public void Recieve(ScaleMechanicEvent ev)
    {
        switch(ev.Type)
        {
            case ScaleMechanicEvent.EventType.Start:
                OnScaleStart();
                break;
            case ScaleMechanicEvent.EventType.Update:
                OnScaleUpdate(ev.UpdatedPosition, ev.UpdatedSize);
                break;
            case ScaleMechanicEvent.EventType.End:
                OnScaleEnd();
                break;
        }
    }

    protected virtual void OnScaleStart()
    {
    }

    protected virtual void OnScaleUpdate(Vector2 newPosition, Vector2 newSize)
    {
    }

    protected virtual void OnScaleEnd()
    {
    }
}

public struct ScaleMechanicEvent
{
    public enum EventType
    {
        Start,
        Update,
        End,
    }

    public EventType Type { get; set; }
    public Vector2 UpdatedPosition { get; set; }
    public Vector2 UpdatedSize { get; set; }

    public ScaleMechanicEvent(EventType type, Vector2 newPosition, Vector2 newSize)
    {
        Type = type;
        UpdatedPosition = newPosition;
        UpdatedSize = newSize;
    }
}