using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public float maxHP = 100.0f;
    public float currentHP = 0.0f;
    public GameObject[] dropItems;

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHP = Mathf.Clamp(currentHP - damageAmount, 0f, maxHP);

        if (Mathf.Approximately(currentHP, 0.0f))
            Die();
    }

    private void Die()
    {
        SpawnDropItems();
        Destroy(gameObject);
    }

    private void SpawnDropItems()
    {
        foreach (GameObject item in dropItems)
        {
            Vector3 position = GetRandomDropPoint(1.0f, 6.0f);
            GameObject droppedItem = Instantiate(item, position, Quaternion.identity);
        }
    }

    private Vector3 GetRandomDropPoint(float dropMinRadius, float dropMaxRadius)
    {
        Vector3 randomDirection = Random.onUnitSphere;
        randomDirection.y = Mathf.Abs(randomDirection.y);
        randomDirection.Normalize();

        float randomRadius = Random.Range(dropMinRadius, dropMaxRadius);
        Vector3 randomPoint = transform.position + randomDirection * randomRadius;

        if (Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit))
            randomPoint = hit.point;

        return randomPoint;
    }
}

