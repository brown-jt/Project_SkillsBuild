using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private InteractionPromptUI promptUI;

    private PlayerInputHandler inputHandler;
    private InteractableArea currentInteractableArea;

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        if (currentInteractableArea == null) return;

        // Check if still interactable whilst in area
        if (!currentInteractableArea.IsInteractable)
        {
            currentInteractableArea = null;
            promptUI.Hide();
            return;
        }


        // Handle interaction
        if (currentInteractableArea.IsInteractable && inputHandler.InteractTriggered)
        {
            currentInteractableArea.Interact();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out InteractableArea interactableArea))
        {
            if (interactableArea.IsInteractable)
            {
                currentInteractableArea = interactableArea;
                promptUI.Show(interactableArea.InteractionPrompt);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out InteractableArea interactableArea))
        {
            if (interactableArea == currentInteractableArea)
            {
                currentInteractableArea = null;
                promptUI.Hide();
            }
        }
    }
}
