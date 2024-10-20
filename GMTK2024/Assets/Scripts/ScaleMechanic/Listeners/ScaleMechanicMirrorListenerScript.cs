using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MirrorScript))]
public class ScaleMechanicMirrorListenerScript : ScaleMechanicListenerScript
{
    [SerializeField] private GameObject mirrorEnabledSprite;
    [SerializeField] private GameObject mirrorDisabledSprite;

    private MirrorScript mirror;

    protected override void Awake()
    {
        mirror = GetComponent<MirrorScript>();
        base.Awake();
    }

    protected override void OnScaleStart()
    {
        base.OnScaleStart();
    }

    protected override void OnScaleEnd()
    {
        base.OnScaleEnd();
    }

    protected override void OnScaleUpdate(Vector2 newPosition, Vector2 newSize)
    {
        base.OnScaleUpdate(newPosition, newSize);

        float smaller = Mathf.Min(newSize.x, newSize.y);
        float larger = Mathf.Max(newSize.x, newSize.y);
        if (smaller * 10 < larger)
        {
            if (!mirror.enabled)
            {
                mirror.enabled = true;
                mirrorEnabledSprite?.SetActive(true);
                mirrorDisabledSprite?.SetActive(false);
                if (GameController.Instance.GetState() == GameState.Play)
                {
                    AudioManager.Instance.PlaySFX("WallToSpawner");
                }
            }
        }
        else
        {
            if (mirror.enabled)
            {
                mirror.enabled = false;
                mirrorEnabledSprite?.SetActive(false);
                mirrorDisabledSprite?.SetActive(true);
                if (GameController.Instance.GetState() == GameState.Play)
                {
                    AudioManager.Instance.PlaySFX("SpawnerToWall");
                }
            }
        }
    }
}
