using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Terrain terrain;
    [SerializeField] private int numberOfObjects = 100;
    [SerializeField] private Transform parentContainer;

    [Header("Randomization Settings")]
    [SerializeField] private float minScale = 0.8f;
    [SerializeField] private float maxScale = 1.2f;

    [Header("Positioning Settings")]
    [SerializeField] private BoxCollider exclusionArea;


    private void Start()
    {
        SpawnObjects();
    }

    private void SpawnObjects()
    {
        if (prefabToSpawn == null || terrain == null)
        {
            Debug.LogError("Invalid Assignments - Please Check");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;

        int spawnedCount = 0;
        int attempts = 0;
        int maxAttempts = numberOfObjects * 10; // safety to avoid infinite loop

        while (spawnedCount < numberOfObjects && attempts < maxAttempts)
        {
            attempts++;

            // Random position within terrain bounds
            float randomX = Random.Range(0f, terrainData.size.x);
            float randomZ = Random.Range(0f, terrainData.size.z);
            float y = terrain.SampleHeight(new Vector3(randomX, 0f, randomZ) + terrainPos);

            Vector3 spawnPos = new Vector3(randomX, y, randomZ) + terrainPos;

            // Skip if inside exclusion area
            if (exclusionArea != null && exclusionArea.bounds.Contains(spawnPos))
                continue;

            // Spawn the object
            GameObject obj = Instantiate(prefabToSpawn, spawnPos, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), parentContainer);

            // Random scale
            float scale = Random.Range(minScale, maxScale);
            obj.transform.localScale = Vector3.one * scale;
            obj.transform.SetParent(parentContainer, true);

            spawnedCount++;
        }

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Reached max attempts. Some objects may not have been spawned due to exclusion area.");
        }
        else
        {
            Debug.Log($"Spawned {spawnedCount} objects in {attempts} attempts.");
        }
    }
}
