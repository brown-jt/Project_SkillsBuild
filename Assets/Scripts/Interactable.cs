using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public abstract class InteractableArea : MonoBehaviour
{
    [SerializeField] private BoxCollider interactionZone;
    [SerializeField] private string interactionPrompt = "Press 'E' to interact";
    private bool isInteractable = true;

    public string InteractionPrompt => interactionPrompt;
    public bool IsInteractable
    {
        get => isInteractable;
        set => isInteractable = value;
    }

    public abstract void Interact();

    private void Awake()
    {
        // Ensure the interaction zone is assigned and set as a trigger
        interactionZone = GetComponent<BoxCollider>();
        interactionZone.isTrigger = true;
    }
}
