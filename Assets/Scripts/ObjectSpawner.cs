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

    [Header("Exclusion Area Settings")]
    [SerializeField] private BoxCollider exclusionZone;


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

        while (spawnedCount < numberOfObjects)
        {
            // Random position within terrain bounds
            float randomX = Random.Range(0f, terrainData.size.x);
            float randomZ = Random.Range(0f, terrainData.size.z);
            float y = terrain.SampleHeight(new Vector3(randomX, 0f, randomZ) + terrainPos);

            Vector3 spawnPos = new Vector3(randomX, y, randomZ) + terrainPos;

            // Spawn the object
            GameObject obj = Instantiate(prefabToSpawn, spawnPos, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), parentContainer);

            // Random scale
            float scale = Random.Range(minScale, maxScale);
            obj.transform.localScale = Vector3.one * scale;
            obj.transform.SetParent(parentContainer, true);

            spawnedCount++;
        }

        RemoveFromExclusionZone();
    }

    private void RemoveFromExclusionZone()
    {
        if (exclusionZone == null)
        {
            Debug.LogWarning("No exclusion zone assigned.");
            return;
        }

        if (parentContainer == null)
        {
            Debug.LogWarning("No parent container assigned.");
            return;
        }

        // World-space center of the BoxCollider (accounts for center offset)
        Vector3 worldCenter = exclusionZone.transform.TransformPoint(exclusionZone.center);

        // Half-extents in local space, then scale by lossyScale to get world half-extents
        Vector3 localHalf = exclusionZone.size * 0.5f;
        Vector3 worldHalf = Vector3.Scale(localHalf, exclusionZone.transform.lossyScale);

        // Use the collider's rotation
        Quaternion orientation = exclusionZone.transform.rotation;

        // Include triggers just in case tree colliders are triggers
        Collider[] hits = Physics.OverlapBox(worldCenter, worldHalf, orientation, ~0, QueryTriggerInteraction.Collide);

        if (hits == null || hits.Length == 0)
        {
            Debug.Log("No colliders detected overlapping exclusion zone.");
            return;
        }

        // Keep track of which root children we've already destroyed
        var destroyed = new System.Collections.Generic.HashSet<GameObject>();
        int removedCount = 0;

        foreach (Collider hit in hits)
        {
            if (hit == null) continue;

            // Walk up the hierarchy until we find a direct child of parentContainer
            Transform t = hit.transform;
            Transform topChild = null;

            // If the hit transform is the parent container itself, skip
            if (t == parentContainer) continue;

            // Find the direct child of parentContainer this collider belongs to
            while (t != null && t != parentContainer)
            {
                topChild = t;
                t = t.parent;
            }

            // If we didn't reach parentContainer (i.e. not a descendant), skip
            if (t != parentContainer || topChild == null) continue;

            GameObject candidate = topChild.gameObject;

            // Only delete trees (tag check) and prevent duplicate destroys
            if (!destroyed.Contains(candidate) && candidate.CompareTag("Tree"))
            {
                destroyed.Add(candidate);
                Destroy(candidate);
                removedCount++;
            }
        }

        Debug.Log($"Removed {removedCount} trees inside exclusion zone (checked {hits.Length} colliders).");
    }
}
