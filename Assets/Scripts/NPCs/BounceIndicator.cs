using UnityEngine;

public class BounceIndicator : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float amplitude = 0.1f; // How high it bounces
    public float frequency = 2f;   // How fast it bounces

    private Transform playerTransform;

    private Vector3 startPos;

    void Start()
    {
        // Store the initial position
        startPos = transform.position;

        // Find the player in the scene
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player != null ? player.transform : null;
    }

    void Update()
    {
        // Apply sine wave to the y position
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        // Rotate to face the player (horizontally only)
        if (playerTransform != null)
        {
            Vector3 directionToPlayer = playerTransform.position - transform.position;

            directionToPlayer.y = 0; // Keep only horizontal direction

            if (directionToPlayer.sqrMagnitude > 0.001f) // Avoids a zero-length vector
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = targetRotation;
            }
        }
    }
}
