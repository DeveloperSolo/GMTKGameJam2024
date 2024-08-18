using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    [SerializeField] private SpawnManager manager;
    [SerializeField] private float spawnRate;

    private float timeTillNextSpawn;

    private void OnEnable()
    {
        timeTillNextSpawn = 1.0f / spawnRate;
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

        timeTillNextSpawn += 1.0f / spawnRate;
    }

    public void GetValueForInfoDisplay(EntityInfoScript.Info info)
    {
        info.InfoValue = spawnRate.ToString("F2");
    }
}