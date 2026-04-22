using UnityEngine;

public class RobotStaticMover : MonoBehaviour
{

    public Transform[] movementPoints;
    public float moveSpeed = 1f;

    private int currentPoint = 0;
    private bool walking = false;

    private void Update()
    {
        if (!walking) return;

        Transform target = movementPoints[currentPoint];
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            currentPoint++;

            if (currentPoint == 2) walking = false;

            else if (currentPoint >= movementPoints.Length) Destroy(gameObject);
        }
    }

    public void StartWalking()
    {
        walking = true;
    }
}
