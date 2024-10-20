using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightHelperScript : MonoBehaviour
{
    private List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    private List<Canvas> canvases = new List<Canvas>();

    private void Awake()
    {
        sprites.AddRange(GetComponentsInChildren<SpriteRenderer>(true));
        canvases.AddRange(GetComponentsInChildren<Canvas>(true));
    }

    public void Highlight()
    {
        SetSortingLayer(SortingLayer.NameToID("Highlight"));
        GameController.Instance.MainCamera.StartHighlight();
    }

    public void ResetHighlight()
    {
        SetSortingLayer(SortingLayer.NameToID("Default"));
        GameController.Instance.MainCamera.StopHighlight();
    }

    private void SetSortingLayer(int sortingLayerID)
    {
        foreach(SpriteRenderer sprite in sprites)
        {
            sprite.sortingLayerID = sortingLayerID;
        }
        foreach (Canvas canvas in canvases)
        {
            canvas.sortingLayerID = sortingLayerID;
        }
    }
}
