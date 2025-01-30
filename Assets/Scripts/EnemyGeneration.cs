using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGeneration : MonoBehaviour
{
    public GameObject enemyPrefab; // Enemy prefab to spawn
    public BoxCollider spawnArea; // Box collider defining spawn area
    public float waveInterval = 10f; // Time between waves
    public int enemiesPerWave = 5; // Number of enemies per wave

    void Start()
    {
        SpawnWave();
        StartCoroutine(SpawnWaveRoutine());
    }

    IEnumerator SpawnWaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(waveInterval);
            SpawnWave();
        }
    }

    void SpawnWave()
    {
        if (enemyPrefab == null || spawnArea == null)
        {
            Debug.LogError("Missing enemy prefab or spawn area!");
            return;
        }

        for (int i = 0; i < enemiesPerWave; i++)
        {
            Vector3 spawnPosition = GetRandomPointInBounds(spawnArea.bounds);
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y =  2.24f; // Keep Y consistent to avoid floating enemies
        float z = Random.Range(bounds.min.z, bounds.max.z);
        return new Vector3(x, y, z);
    }
}

