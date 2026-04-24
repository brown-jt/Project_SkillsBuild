using UnityEngine;

public class RobotStaticMover : MonoBehaviour
{
    public Transform[] movementPoints;
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;
    public float acceleration = 5f;

    public DissolveController dissolveController;
    public Animator animator;

    private int currentPoint = 0;
    private bool walking = false;
    private bool isBuilt = false;

    private float currentSpeed = 0f;

    public bool IsWalking => walking;
    public bool IsBuilt => isBuilt;

    private void Update()
    {
        if (!walking || movementPoints.Length == 0 || currentPoint >= movementPoints.Length)
        {
            UpdateAnimation(0f);
            return;
        }

        Transform target = movementPoints[currentPoint];

        // Direction to next point
        Vector3 direction = (target.position - transform.position);
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance > 0.01f)
        {
            Vector3 dirNormalized = direction.normalized;

            // --- Smooth rotation ---
            Quaternion targetRotation = Quaternion.LookRotation(dirNormalized);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            // --- Only move when mostly facing target ---
            float angle = Vector3.Angle(transform.forward, dirNormalized);

            if (angle < 10f)
            {
                // Smooth acceleration
                currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, Time.deltaTime * acceleration);

                Vector3 movement = transform.forward * currentSpeed;
                transform.position += movement * Time.deltaTime;

                UpdateAnimation(currentSpeed / moveSpeed);
            }
            else
            {
                // Slow down while turning
                currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime * acceleration);
                UpdateAnimation(0f);
            }
        }

        // Reached point
        if (distance < 0.1f)
        {
            currentPoint++;

            if (currentPoint == 2)
            {
                walking = false;
            }
            else if (currentPoint >= movementPoints.Length)
            {
                walking = false;
                DestroyImmediate(gameObject);
            }
        }
    }

    public void StartWalking()
    {
        walking = true;
    }

    public void Build()
    {
        isBuilt = true;
        StartWalking();
    }
    
    public void SetMovementPoints(Transform[] points)
    {
        movementPoints = points;
    }

    private void UpdateAnimation(float normalizedSpeed)
    {
        float lerpedSpeed = Mathf.Lerp(
            animator.GetFloat("Speed"),
            normalizedSpeed,
            Time.deltaTime * 10f
        );

        if (lerpedSpeed < 0.01f)
        {
            lerpedSpeed = 0f;
        }

        animator.SetFloat("Speed", lerpedSpeed);
    }
}