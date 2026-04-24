using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject horseAnimal;
    [SerializeField] private GameObject horseAnimalQuest;
    [SerializeField] private GameObject bisonAnimal;
    [SerializeField] private GameObject bisonAnimalQuest;
    [SerializeField] private GameObject dogAnimal;
    [SerializeField] private GameObject dogAnimalQuest;
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
        GameObject[] animalPrefabs = new GameObject[] { horseAnimal, bisonAnimal, dogAnimal };

        for (int i = 0; i < numberToSpawn; i++)
        {
            if (NavMeshUtils.RandomPointOnNavMesh(out Vector3 pos))
            {
                var animal = Instantiate(animalPrefabs[i % animalPrefabs.Length], pos, Quaternion.identity, parentContainer);

                float scale = Random.Range(minScale, maxScale);
                animal.transform.localScale *= scale;
            }
        }

        // For now just spawn the quest animal as part of this bigger function
        //SpawnQuestAnimal();
    }

    private void SpawnQuestAnimal()
    {
        if (NavMeshUtils.RandomPointOnNavMesh(out Vector3 pos))
        {
            var questAnimal = Instantiate(horseAnimalQuest, pos, Quaternion.identity, parentContainer);
        }
    }
}
