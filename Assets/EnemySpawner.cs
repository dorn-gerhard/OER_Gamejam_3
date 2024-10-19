using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject EnemyType;
    public Transform spawnPoint;
    public float spawnDelay = 1;
    public float maxSpaceBetweenSpawns = 25;
    public float minDistanceToPlayer = 5;

    float timeSinceLastSpawn = 0;
    float maxTries = 4;
    float tryCounter = 0;
    // Update is called once per frame
    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnDelay)
        {
            timeSinceLastSpawn = 0;
            var enemy = Instantiate(EnemyType, transform);

            Vector3 randomPosition = CalculateRandomPosition();

            while (tryCounter < maxTries && Vector3.Distance(randomPosition, PlayerController.current.transform.position) < minDistanceToPlayer)
            {
                randomPosition = CalculateRandomPosition();
                tryCounter++;
            }

            tryCounter = 0;

            while (tryCounter < maxTries && Vector3.Distance(randomPosition, PlayerController.current.transform.position) < 5)
            {
                randomPosition = CalculateRandomPosition();
            }

            enemy.transform.position = randomPosition;
        }
    }

    Vector3 CalculateRandomPosition()
    {
        return spawnPoint.transform.position + new Vector3(Random.Range(-maxSpaceBetweenSpawns, maxSpaceBetweenSpawns), Random.Range(-maxSpaceBetweenSpawns, maxSpaceBetweenSpawns));
    }
}
