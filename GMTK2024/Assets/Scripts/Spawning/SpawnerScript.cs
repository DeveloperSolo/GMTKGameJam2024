using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    [SerializeField] private SpawnManager manager;
    [SerializeField] private float spawnRate = 1;

    private float spawnWaveDuration;
    private float spawnWaveCount;

    private float timeTillNextSpawn;
    private float spawnCountRemainder;

    private ScaleMechanicComponent sizeSource;

    private void Awake()
    {
        CalculateStats();
        sizeSource = GetComponentInChildren<ScaleMechanicComponent>();
    }

    private void Update()
    {
        ProcessContinuousSpawn(Time.deltaTime);
    }

    private void ProcessContinuousSpawn(float elapsed)
    {
        if(spawnRate <= 0.0f)
        {
            return;
        }

        timeTillNextSpawn -= elapsed;
        if(timeTillNextSpawn > 0)
        {
            return;
        }

        spawnCountRemainder += spawnWaveCount;
        int count = Mathf.FloorToInt(spawnCountRemainder);
        spawnCountRemainder -= count;

        for(int i = 0; i < count; ++i)
        {
            GameObject instance = manager.SpawnInstance();
            Vector2 extents = sizeSource.CurrentSize / 2.0f;
            Vector3 randOffset = new Vector3(Random.Range(-extents.x, extents.x), Random.Range(-extents.y, extents.y), 0.0f);
            instance.transform.position = transform.position + randOffset;
        }

        timeTillNextSpawn += spawnWaveDuration;
    }

    public void GetValueForInfoDisplay(EntityInfoScript.Info info)
    {
        info.InfoValue = spawnRate.ToString("F2") + "\n(" + spawnWaveCount + " in " + spawnWaveDuration.ToString("F1") +  "s)";
    }

    public void SetSpawnRateFromScaling(float value)
    {
        spawnRate = Mathf.Max(value, 0.0f);
        CalculateStats();
    }

    private void CalculateStats()
    {
        // spawn rate = 2 => duration = 1, count = 2
        // spawn rate = 1.5 => duration = 1.333, count = 2
        // spawn rate = 1.1 => duration = 1.81818181818181, count = 2
        // spawn rate = 1 => duration = 1, count = 1
        // spawn rate = 0.5 => duration = 2, count = 1

        if (spawnRate <= 0)
        {
            spawnWaveDuration = 0.0f;
            spawnWaveCount = 0.0f;
            return;
        }

        spawnWaveDuration = 1.0f / spawnRate;
        spawnWaveCount = Mathf.Ceil(spawnRate);
        spawnWaveDuration *= spawnWaveCount;

        timeTillNextSpawn = spawnWaveDuration;
    }
}