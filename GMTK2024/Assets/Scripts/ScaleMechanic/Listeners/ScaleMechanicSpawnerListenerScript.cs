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

        if(Mathf.Abs(newSize.x - newSize.y) < 0.1f)
        {
            spawner.enabled = true;
            spawnerEnabledSprite?.SetActive(true);
            spawnerDisabledSprite?.SetActive(false);
        }
        else
        {
            spawner.enabled = false;
            spawnerEnabledSprite?.SetActive(false);
            spawnerDisabledSprite?.SetActive(true);
        }
    }
}
