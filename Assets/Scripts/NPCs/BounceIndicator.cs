using UnityEngine;

public class BounceIndicator : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float amplitude = 0.1f; // How high it bounces
    public float frequency = 2f;   // How fast it bounces
    [SerializeField] private Transform agentTransform;

    private Transform playerTransform;

    void Start()
    {
        // Find the player in the scene
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player != null ? player.transform : null;
    }

    void Update()
    {
        if (agentTransform == null) return;

        // Base position (add a fixed height offset so it sits above the agent)
        Vector3 basePosition = agentTransform.position + Vector3.up * 0.25f;

        // Small smooth bounce on top of that
        float bounce = Mathf.Sin(Time.time * frequency) * amplitude;

        transform.position = basePosition + Vector3.up * bounce;

        FacePlayer();
    }

    private void FacePlayer()
    {
        if (playerTransform == null) return;

        Vector3 directionToPlayer = playerTransform.position - transform.position;

        directionToPlayer.y = 0; // Keep only horizontal direction as we don't want the agent tilting up/down

        if (directionToPlayer.sqrMagnitude > 0.001f) // Avoids a zero-length vector
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = targetRotation;
        }
    }

}
