using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public float maxHP = 0;
    public float curntHP= 0;

    public GameObject[] dropItems;
    // Start is called before the first frame update
    void Start()
    {
        curntHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        maxHP = curntHP;
        Die();
    }

    public void Takedamage(float damage)
    {
       curntHP -= damage;
       Mathf.Clamp(damage, 0, curntHP);
    }

    public void Die()
    {
        int i;
        if (maxHP <= 0)
        {
            for ( i = 0; i < 1; i ++)
            {
                spawnItemonDeath();

            }
          
          Destroy(gameObject);
        }

    }

    public void spawnItemonDeath()
    {

        
        foreach (GameObject item in dropItems) 
        {
            GameObject itemclone = Instantiate(item, GetRandomPointInBounds(transform), Quaternion.identity);
        }
    }

    Vector3 GetRandomPointInBounds(Transform transform)
    {
        float x = Random.Range(-transform.position.x , transform.position.x );
        float y = 2.24f; // Keep Y consistent to avoid floating enemies
        float z = Random.Range(-transform.position.z , transform.position.z );
        return new Vector3(x, y, z);
    }
}
