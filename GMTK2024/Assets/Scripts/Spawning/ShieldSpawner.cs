using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSpawner : MonoBehaviour
{
    [SerializeField] private SpawnManager manager;
    [SerializeField] private float spawnRate = 1.0f;

    public int spawnVolume = 100;
    private float timeTillNextSpawn;

    // Start is called before the first frame update
    void Start()
    {
        timeTillNextSpawn = 1.0f / spawnRate;
    }

    // Update is called once per frame
    void Update()
    {
        timeTillNextSpawn -= Time.deltaTime;
        if(timeTillNextSpawn > 0.0f)
        {
            return;
        }

        for (int i = 0; i < spawnVolume; i++)
        {
            GameObject shield = manager.SpawnInstance();
            shield.transform.position = GameController.Instance.GetRandomPositionInGameArea(Vector2.one * 80);
        }
        timeTillNextSpawn = 1.0f / spawnRate;
    }
}
