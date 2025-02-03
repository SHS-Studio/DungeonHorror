using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzelmanager : MonoBehaviour
{
    public static Puzzelmanager instance { get; set; }
    public List<GameObject> PuzzelSlots; // List to hold weapon slots
    public GameObject  activePuzzelSlots; // List to hold weapon slots
    public GameObject[] PuzzelPices; // Enemy prefab to spawn
    public BoxCollider spawnArea; // Box collider defining spawn area
    public float SpawnInterval = 10f; // Time between waves
    public int spawncounth = 0;

    public void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        activePuzzelSlots = PuzzelSlots[0];
        StartCoroutine(SpawnWaveRoutine());
    }
    public void Update()
    {
        PuzzelCollector();
    }

    IEnumerator SpawnWaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(SpawnInterval);
            SpawnPuzzelPieces();
        }
    }

    void SpawnPuzzelPieces()
    {
        if (PuzzelPices == null || spawnArea == null)
        {
            Debug.LogError("Missing enemy prefab or spawn area!");
            return;
        }

        for (int i = 0; i < spawncounth; i++)
        {
            foreach (GameObject enemytype in PuzzelPices)
            {
                Vector3 spawnPosition = GetRandomPointInBounds(spawnArea.bounds);
                Instantiate(enemytype, spawnPosition, Quaternion.identity);
            }
        }
    }

    Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = 2.24f; // Keep Y consistent to avoid floating enemies
        float z = Random.Range(bounds.min.z, bounds.max.z);
        return new Vector3(x, y, z);
    }

    public void PickUpPuzzelpieces(GameObject puzzelpieces)
    {
        AddpuzzelpieceinActiveslot(puzzelpieces);
    }

    private void AddpuzzelpieceinActiveslot(GameObject Pickedpiece)
    {
        Pickedpiece.transform.SetParent(activePuzzelSlots.transform, true);

        PuzzelPiece m_pieces = Pickedpiece.GetComponent<PuzzelPiece>();

        Pickedpiece.transform.localPosition = activePuzzelSlots.transform.localPosition;
        Pickedpiece.transform.localRotation = activePuzzelSlots.transform.localRotation;

    }

    public void PuzzelCollector()
    {
        if (PickUpManager.instance.totalpiececount == 3)
        {
            activePuzzelSlots.SetActive(true);
        }
    }
}
