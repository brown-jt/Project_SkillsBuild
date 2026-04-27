using UnityEngine;
using System.Collections;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsInteracting { get; private set; }

    public void StartInteraction()
    {
        IsInteracting = true;
    }

    public void EndInteraction()
    {
        IsInteracting = false;
    }

    public IEnumerator DelayedEndInteraction()
    {
        yield return new WaitForSeconds(0.2f);
        EndInteraction();
    }
}