using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public abstract class InteractableArea : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private string interactName = "";
    [SerializeField] private BoxCollider interactionZone;
    [SerializeField] private string interactionPrompt = "CHANGE ME";
    [SerializeField] private AudioClip interactionSound;

    private bool isInteractable = true;
    public string InteractName => interactName;
    public string InteractionPrompt => interactionPrompt;
    public AudioClip InteractionSound => interactionSound;
    public bool IsInteractable
    {
        get => isInteractable;
        set => isInteractable = value;
    }

    public abstract void Interact();

    private void Start()
    {
        // Ensure the interaction zone is assigned and set as a trigger
        interactionZone = GetComponent<BoxCollider>();
        interactionZone.isTrigger = true;
    }
}
