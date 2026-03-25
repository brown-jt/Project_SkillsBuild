using UnityEngine;
using System.Collections;

public class QuestionBoxSpawner : MonoBehaviour
{
    public GameObject questionBoxPrefab;
    public Transform spawnPoint;
    public float pushForce = 500f;
    public float spawnDelay = 0.25f;
    public WarehouseQuizManager quizManager;

    [SerializeField] private TruckController truckController;

    public void SpawnBoxes()
    {
        StartCoroutine(SpawnBoxesCoroutine());
    }

    private IEnumerator SpawnBoxesCoroutine()
    {
        int boxCount = quizManager.questionSet != null ? quizManager.questionSet.questions.Count : 0;

        quizManager.StartQuiz(); // Start the quiz timer when spawning boxes

        for (int i = 0; i < boxCount; i++)
        {
            GameObject box = Instantiate(questionBoxPrefab, spawnPoint.position, spawnPoint.rotation);
            box.GetComponent<QuestionBox>().Initialise(quizManager.questionSet.questions[i]); // Pass question data to box

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

        truckController.StartDriving();
    }
}
