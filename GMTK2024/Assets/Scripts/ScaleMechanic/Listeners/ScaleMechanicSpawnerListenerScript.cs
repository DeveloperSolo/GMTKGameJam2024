using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpawnerScript))]
public class ScaleMechanicSpawnerListenerScript : ScaleMechanicListenerScript
{
    [SerializeField] private GameObject spawnerEnabledSprite;
    [SerializeField] private GameObject spawnerDisabledSprite;

    private SpawnerScript spawner;

    protected override void Awake()
    {
        base.Awake();
        spawner = GetComponent<SpawnerScript>();
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
        if(Mathf.Abs(newSize.x - newSize.y) < smaller * 0.1f)
        {
            if(!spawner.enabled)
            {
                spawner.enabled = true;
                spawnerEnabledSprite?.SetActive(true);
                spawnerDisabledSprite?.SetActive(false);
                if(GameController.Instance.GetState() == GameState.Play)
                {
                    AudioManager.Instance.PlaySFX("WallToSpawner");
                }
            }
        }
        else
        {
            if(spawner.enabled)
            {
                spawner.enabled = false;
                spawnerEnabledSprite?.SetActive(false);
                spawnerDisabledSprite?.SetActive(true);
                if (GameController.Instance.GetState() == GameState.Play)
                {
                    AudioManager.Instance.PlaySFX("SpawnerToWall");
                }
            }
        }
    }
}
