using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerInteract : MonoBehaviour
{
    [Header("Interact Prompt UI Reference")]
    [SerializeField] private InteractionPromptUI promptUI;

    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 3f;

    [Header("Holdable Item Settings")]
    [SerializeField] private Transform holdPoint;
    private HoldableItem currentlyHeldItem;

    public bool IsHoldingItem => currentlyHeldItem != null;
    public HoldableItem HeldItem => currentlyHeldItem;

    private PlayerInputHandler inputHandler;
    private InteractableArea currentArea;
    private InteractableItem currentItem;

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        HandleDropInput();
        HandleAreaInteration();
        HandleItemInteraction();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out InteractableArea interactableArea))
        {
            if (interactableArea.IsInteractable)
            {
                currentArea = interactableArea;
                promptUI.Show($"{interactableArea.InteractionPrompt}", $"{interactableArea.InteractName}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out InteractableArea interactableArea))
        {
            if (interactableArea == currentArea)
            {
                currentArea = null;
                promptUI.Hide();
            }
        }
    }

    private void HandleAreaInteration() 
    {
        if (currentArea == null) return;

        // Check if still interactable whilst in area
        if (!currentArea.IsInteractable)
        {
            currentArea = null;
            promptUI.Hide();
            return;
        }

        // Handle interaction
        if (currentArea.IsInteractable && inputHandler.InteractTriggered)
        {
            currentArea.Interact();
        }
    }

    private void HandleItemInteraction() 
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        int itemLayerMask = LayerMask.GetMask("Item", "QuestNPC", "ItemInteractable", "HoldableItem");
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, itemLayerMask))
        {
            if (hit.collider.TryGetComponent(out InteractableItem interactableItem))
            {
                // Check if interactable first
                if (!interactableItem.IsInteractable)
                {
                    if (currentItem != null)
                    {
                        currentItem = null;
                        promptUI.Hide();
                    }
                    return;
                }

                // Show prompt
                if (currentItem != interactableItem)
                {
                    currentItem = interactableItem;
                    promptUI.Show($"{interactableItem.InteractionPrompt}", $"{interactableItem.InteractableName}");
                }

                // Interact if key pressed
                if (inputHandler.InteractTriggered)
                {
                    // Check to see if it's a holdable item and handle pick up differently else normal interaction
                    if (interactableItem is HoldableItem holdableItem)
                    {
                        PickUpItem(holdableItem);
                    }
                    else
                    {
                        interactableItem.Interact();
                    }

                    // For now, hide prompt after any item interaction
                    // TODO - Handle differently for pick up vs other interactions
                    promptUI.Hide();
                }

                return;
            }
        }

        // Hide prompt if not looking at interactable
        if (currentItem != null)
        {
            currentItem = null;
            promptUI.Hide();
        }
    }

    public void PickUpItem(HoldableItem item)
    {
        if (currentlyHeldItem != null)
        {
            FeedbackNotificationsUI.Instance.AddNotification("You are already holding something!");
            return;
        }

        currentlyHeldItem = item;
        currentlyHeldItem.PickUp(holdPoint);
    }

    public void DropHeldItem()
    {
        if (currentlyHeldItem != null)
        {
            currentlyHeldItem.Drop();
            currentlyHeldItem = null;
        }
    }

    private void HandleDropInput()
    {
        if (IsHoldingItem && inputHandler.DropTriggered)
        {
            DropHeldItem();
        }
    }
}
