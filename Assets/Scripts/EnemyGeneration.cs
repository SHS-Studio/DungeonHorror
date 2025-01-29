using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGeneration : MonoBehaviour
{
    public GameObject enemyPrefab; // Enemy prefab to spawn
    public BoxCollider spawnArea; // Box collider defining spawn area
    public float spawnInterval = 5f; // Time between spawns

    void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null || spawnArea == null)
        {
            Debug.LogError("Missing enemy prefab or spawn area!");
            return;
        }

        Vector3 spawnPosition = GetRandomPointInBounds(spawnArea.bounds);
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

       
    }

    Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = bounds.min.y; // Keep Y consistent to avoid floating enemies
        float z = Random.Range(bounds.min.z, bounds.max.z);
        return new Vector3(x, y, z);
    }
}
