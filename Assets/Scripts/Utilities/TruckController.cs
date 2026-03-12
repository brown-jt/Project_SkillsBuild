using UnityEngine;

public class TruckController : MonoBehaviour
{
    public float reverseSpeed = 2.0f;
    public Transform stopPoint;

    private Animator animator;
    private bool isReversing = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isReversing)
        {
            ReverseTruck();
        }
    }

    void ReverseTruck()
    {
        transform.Translate(Vector3.back * reverseSpeed * Time.deltaTime);

        float distanceToStopPoint = Vector3.Distance(transform.position, stopPoint.position);
        Debug.Log("Distance to stop point: " + distanceToStopPoint);

        if (distanceToStopPoint < 0.5f)
        {
            isReversing = false;
            OpenDoors();
        }
    }

    void OpenDoors()
    {
        animator.SetTrigger("OpenLeftDoor");
        animator.SetTrigger("OpenRightDoor");
    }
}
