using UnityEngine;

public abstract class InteractableItem : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private string itemName = "Unnamed Item";
    [SerializeField] private string interactionPrompt = "CHANGE ME";
    [SerializeField] private AudioClip interactionSound;

    private bool isInteractable = true;

    public string ItemName => itemName;
    public string InteractionPrompt => interactionPrompt;
    public AudioClip InteractionSound => interactionSound;
    public bool IsInteractable
    {
        get => isInteractable;
        set => isInteractable = value;
    }

    public abstract void Interact();
}