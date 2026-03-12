using UnityEngine;
using System.Collections;

public class QuestionBoxSpawner : MonoBehaviour
{
    public GameObject questionBoxPrefab; // Assign your box prefab here
    public Transform spawnPoint; // Assign the empty spawn point at truck back
    public float pushForce = 500f; // Adjust for how fast you want boxes to shoot
    public float spawnDelay = 0.25f; // Delay between each box spawn for better visual effect

    // Debug test question set
    public QuestionSetData questionSet;

    public void SpawnBoxes()
    {
        StartCoroutine(SpawnBoxesCoroutine());
    }

    private IEnumerator SpawnBoxesCoroutine()
    {
        int boxCount = questionSet != null ? questionSet.questions.Count : 0;

        for (int i = 0; i < boxCount; i++)
        {
            GameObject box = Instantiate(questionBoxPrefab, spawnPoint.position, spawnPoint.rotation);
            box.GetComponent<QuestionBox>().Initialise(questionSet.questions[i]); // Pass question data to box

            if (box.TryGetComponent<Rigidbody>(out var rb))
            {
                Vector3 pushDirection = -spawnPoint.forward;

                // Add forward force relative to spawn point direction
                rb.AddForce(pushDirection * pushForce);

                // Add slight random torque for realism
                rb.AddTorque(Random.insideUnitSphere * 5f);
            }

            // Wait before spawning the next box
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
