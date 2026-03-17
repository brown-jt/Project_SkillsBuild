using System.Collections;
using UnityEngine;

public class TruckController : MonoBehaviour
{
    public float reverseSpeed = 2.0f;
    public Transform stopPoint;

    private Animator animator;
    private bool isReversing = false;

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

    public void StartReversing()
    {
        isReversing = true;
    }

    private void ReverseTruck()
    {
        transform.Translate(Vector3.back * reverseSpeed * Time.deltaTime);

        float distanceToStopPoint = Vector3.Distance(transform.position, stopPoint.position);

        if (distanceToStopPoint < 0.5f)
        {
            isReversing = false;
            OpenDoors();
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
