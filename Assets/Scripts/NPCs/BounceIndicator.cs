using UnityEngine;

public class BounceIndicator : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float amplitude = 0.1f; // How high it bounces
    public float frequency = 2f;   // How fast it bounces
    [SerializeField] private Transform agentTransform;

    private Transform playerTransform;
    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;

        // Find the player in the scene
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player != null ? player.transform : null;
    }

    void Update()
    {
        if (agentTransform == null) QuestBounce();
        else HintBounce();

        FacePlayer();

    }

    private void QuestBounce()
    {
        // Apply sine wave to the y position for quest indicator
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;

        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    private void HintBounce()
    {
        // Base position (add a fixed height offset so it sits above the agent)
        Vector3 basePosition = agentTransform.position + Vector3.up * 0.25f;

        // Small smooth bounce on top of that
        float bounce = Mathf.Sin(Time.time * frequency) * amplitude;

        transform.position = basePosition + Vector3.up * bounce;
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
