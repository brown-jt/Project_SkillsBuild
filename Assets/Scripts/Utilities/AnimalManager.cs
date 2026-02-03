using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject horseAnimal;
    [SerializeField] private GameObject bisonAnimal;
    [SerializeField] private GameObject dogAnimal;
    [Space]
    [SerializeField] private int minSpawn;
    [SerializeField] private int maxSpawn;
    [Space]
    [SerializeField] private Terrain terrain;
    [SerializeField] private Transform parentContainer;

    [Header("Randomization Settings")]
    [SerializeField] private float minScale = 0.8f;
    [SerializeField] private float maxScale = 1.2f;

    // TODO: Normal / Fake (discoloured) textures

    private void Start()
    {
        SpawnAnimals();
    }

    private void SpawnAnimals()
    {
        int numberToSpawn = Random.Range(minSpawn, maxSpawn);

        for (int i = 0; i < numberToSpawn; i++)
        {
            if (NavMeshUtils.RandomPointOnNavMesh(out Vector3 pos))
            {
                var animal = Instantiate(horseAnimal, pos, Quaternion.identity, parentContainer);

                float scale = Random.Range(minScale, maxScale);
                animal.transform.localScale *= scale;
            }
        }
    }
}
