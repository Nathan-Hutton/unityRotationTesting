using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureSpawner : MonoBehaviour
{
    private float timeSinceLastSpawn;
    public float spawnRate = 5f;
    public GameObject objectToSpawn;
    void Start()
    {
        timeSinceLastSpawn = Time.time;
    }
    void Update()
    {
        if(Time.time - timeSinceLastSpawn >= spawnRate)
        {
               Instantiate(objectToSpawn, transform.position + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5)), Quaternion.identity);
               timeSinceLastSpawn = Time.time;
        }
    }
}
