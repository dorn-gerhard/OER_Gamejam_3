using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject EnemyType;
    public float spawnDelay = 1;
    public float maxSpaceBetweenSpawns = 25;

    float timeSinceLastSpawn = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnDelay)
        {
            timeSinceLastSpawn = 0;
            var enemy = Instantiate(EnemyType, transform);
            enemy.transform.position += new Vector3(Random.Range(-maxSpaceBetweenSpawns, maxSpaceBetweenSpawns), Random.Range(-maxSpaceBetweenSpawns, maxSpaceBetweenSpawns));
        }
    }
}
