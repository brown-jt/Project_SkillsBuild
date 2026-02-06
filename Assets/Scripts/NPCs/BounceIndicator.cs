using UnityEngine;

public class BounceIndicator : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float amplitude = 0.1f; // How high it bounces
    public float frequency = 2f;   // How fast it bounces

    private Vector3 startPos;

    void Start()
    {
        // Store the initial position
        startPos = transform.position;
    }

    void Update()
    {
        // Apply sine wave to the y position
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
