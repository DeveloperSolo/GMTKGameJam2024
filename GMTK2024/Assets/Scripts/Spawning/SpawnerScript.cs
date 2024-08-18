using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour, IEntityInfoSource
{
    [SerializeField] private SpawnManager manager;
    [SerializeField] private float spawnRate;

    private float timeTillNextSpawn;

    private void OnEnable()
    {
        timeTillNextSpawn = spawnRate;
    }

    private void Update()
    {
        ProcessContinuousSpawn(Time.deltaTime);
    }

    private void ProcessContinuousSpawn(float elapsed)
    {
        timeTillNextSpawn -= elapsed;
        if(timeTillNextSpawn > 0 )
        {
            return;
        }

        GameObject instance = manager.SpawnInstance();
        instance.transform.position = transform.position;

        timeTillNextSpawn += spawnRate;
    }

    public void GetSizeValueForInfoDisplay(EntityInfoScript.Info info)
    {
        info.InfoValue = spawnRate.ToString("F1") + " units/s";
    }
}