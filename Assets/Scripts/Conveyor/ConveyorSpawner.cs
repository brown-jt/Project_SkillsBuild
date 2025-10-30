using System.Collections;
using UnityEngine;

public class ConveyorSpawner : MonoBehaviour
{
    [Header("Robot Parts to Spawn")]
    [SerializeField] GameObject[] smallRobotParts;
    [SerializeField] GameObject[] mediumRobotParts;
    [SerializeField] GameObject[] largeRobotParts;

    [Header("Spawn Settings")]
    [SerializeField] bool autoSpawnOnStart = true;
    [SerializeField] float minSpawnDelay = 3f;
    [SerializeField] float maxSpawnDelay = 5f;

    private int spawnIndex = 0;
    private int spawnedItems = 0;

    private void Start()
    {
        if (autoSpawnOnStart)
        {
            StartCoroutine(SpawnItems());
        }
    }
    private void SpawnRandomObject()
    {
        GameObject prefabToSpawn;

        if (spawnedItems < 4)
        {
            prefabToSpawn = smallRobotParts[spawnIndex];
        }
        else if (spawnedItems < 8)
        {
            prefabToSpawn = mediumRobotParts[spawnIndex];
        }
        else
        {
            prefabToSpawn = largeRobotParts[spawnIndex];
        }

        // Random Y rotation for the object so it appears varied
        prefabToSpawn.transform.rotation = Quaternion.Euler(
            prefabToSpawn.transform.rotation.x, 
            Random.Range(0, 360), prefabToSpawn.
            transform.rotation.z
        );

        Debug.Log($"Spawning Part #{spawnedItems + 1}: {prefabToSpawn.name}");
        Instantiate(prefabToSpawn, transform.position, prefabToSpawn.transform.rotation);

        spawnIndex = (spawnIndex + 1) % 4;
        spawnedItems = (spawnedItems + 1) % (smallRobotParts.Length + mediumRobotParts.Length + largeRobotParts.Length);
    }

    IEnumerator SpawnItems()
    {
        while (true)
        {
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);

            SpawnRandomObject();
        }
    }
}
