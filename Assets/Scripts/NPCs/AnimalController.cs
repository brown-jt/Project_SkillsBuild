using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AnimalController : MonoBehaviour, IOnSceneActivated
{
    public enum State
    {
        REST,
        WALK,
        RUN
    }

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float runSpeed = 3.5f;

    [Header("Rest Timing")]
    [SerializeField] private float minRestTime = 3f;
    [SerializeField] private float maxRestTime = 10f;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;

    private State currentState;
    private Coroutine stateRoutine;

    private void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!agent) agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        InitialiseAnimals();
    }

    public void OnSceneActivated()
    {
        InitialiseAnimals();
    }

    private void InitialiseAnimals()
    {
        // Kill old behavior
        if (stateRoutine != null)
            StopCoroutine(stateRoutine);

        // Reset NavMeshAgent safely
        agent.ResetPath();
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        // Reset animator
        animator.Rebind();
        animator.Update(0f);

        // Restart behavior
        stateRoutine = StartCoroutine(StateCycle());
    }

    private IEnumerator StateCycle()
    {
        while (true)
        {
            yield return RestPhase();
            yield return MovePhase();
        }
    }

    private IEnumerator RestPhase()
    {
        currentState = State.REST;

        agent.isStopped = true;
        animator.SetTrigger("Down");

        float restTime = Random.Range(minRestTime, maxRestTime);
        yield return new WaitForSeconds(restTime);

        animator.SetTrigger("Up");

        // Wait for the "Up" animation to finish before exiting out of the Rest Phase
        yield return null;
        float length = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length);
    }

    private IEnumerator MovePhase()
    {
        currentState = (Random.value > 0.3f) ? State.WALK : State.RUN;

        bool walking = currentState == State.WALK;

        animator.SetBool("Walk", walking);
        animator.SetBool("Run", !walking);

        agent.speed = walking ? walkSpeed : runSpeed;
        agent.isStopped = false;

        if (NavMeshUtils.RandomPointOnNavMesh(out Vector3 pos))
        {
            agent.SetDestination(pos);
        }

        yield return new WaitUntil(() =>
            !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance);

        animator.SetBool("Walk", false);
        animator.SetBool("Run", false);
    }
}
