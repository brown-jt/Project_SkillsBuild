using System.Collections;
using UnityEngine;

public class TruckController : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public Transform reverseStopPoint;
    public Transform forwardsStopPoint;

    private Animator animator;

    private bool isMoving = false;
    private int moveDirection; // 1 = forwards and -1 = reverse

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isMoving)
        {
            MoveTruck();
        }
    }

    public void StartReversing()
    {
        moveDirection = -1;
        isMoving = true;
    }

    public void StartDriving()
    {
        moveDirection = 1;
        isMoving = true;
    }

    private void MoveTruck()
    {
        transform.Translate(Vector3.forward * moveDirection * moveSpeed * Time.deltaTime);

        Transform stopPointToUse = moveDirection == 1 ? forwardsStopPoint : reverseStopPoint;

        float distanceToStopPoint = Vector3.Distance(transform.position, stopPointToUse.position);

        if (distanceToStopPoint < 0.5f)
        {
            isMoving = false;

            // Only call if reversing
            if (moveDirection == -1) OpenDoors();
        }
    }

    void OpenDoors()
    {
        StartCoroutine(OpenDoorsCoroutine());
    }

    IEnumerator OpenDoorsCoroutine()
    {
        animator.SetTrigger("OpenLeftDoor");
        animator.SetTrigger("OpenRightDoor");
        yield return new WaitForSeconds(2f);

        QuestionBoxSpawner spawner = GetComponentInParent<QuestionBoxSpawner>();
        if (spawner != null)
        {
            spawner.SpawnBoxes();
        }
    }
}
