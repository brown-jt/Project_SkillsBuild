using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorSpawner : MonoBehaviour, IOnSceneActivated
{
    [Header("Robot Parts to Spawn")]
    [SerializeField] GameObject[] robotParts;

    [Header("Spawn Settings")]
    [SerializeField] bool autoSpawnOnStart = true;
    [SerializeField] float minSpawnDelay = 3f;
    [SerializeField] float maxSpawnDelay = 5f;
    [SerializeField] Transform spawnParent;

    private List<GameObject> availableParts = new List<GameObject>();
    private Coroutine spawnRoutine;

    private void Awake()
    {
        ResetRobotParts();
    }

    public void OnSceneActivated()
    {
        // Destroy all previously spawned items to start fresh as I had
        // issues restarting movement of pre-existing conveyor items
        if (spawnParent != null)
        {
            // Loop backwards and safely remove all children
            for (int i = spawnParent.childCount - 1; i >= 0; i--)
            {
                Destroy(spawnParent.GetChild(i).gameObject);
            }
        }

        // Stop any existing coroutine if it somehow exists
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        ResetRobotParts();

        if (autoSpawnOnStart)
        {
            spawnRoutine = StartCoroutine(SpawnItems());
        }
    }

    private void OnDisable()
    {
        // Optional: stop coroutine to be safe
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }


    private void SpawnRandomObject()
    {
        // Reset parts if needed
        if (availableParts.Count == 0)
        {
            ResetRobotParts();
        }

        // Our randomly chosen prefab to spawn
        int index = Random.Range(0, availableParts.Count);
        GameObject prefabToSpawn = availableParts[index];

        // Remove it so it can't be chosen again this spawn cycle
        availableParts.RemoveAt(index);

        // Spawning prefab
        GameObject spawnedPrefab = Instantiate(prefabToSpawn, transform.position, prefabToSpawn.transform.rotation, spawnParent);

        // Random Y rotation for the prefab object so it appears varied
        spawnedPrefab.transform.rotation = Quaternion.Euler(
            prefabToSpawn.transform.rotation.x,
            Random.Range(0, 360),
            prefabToSpawn.transform.rotation.z
        );
    }

    private void ResetRobotParts()
    {
        availableParts = new List<GameObject>(robotParts);
    }

    private IEnumerator SpawnItems()
    {
        while (true)
        {
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
            SpawnRandomObject();
        }
    }
}
