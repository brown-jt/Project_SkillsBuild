using UnityEngine;

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

    private Camera mainCamera;
    private bool isPromptVisible;

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        mainCamera = Camera.main;
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
            currentArea = interactableArea;

            if (interactableArea.IsInteractable)
            {
                promptUI.Show($"{interactableArea.InteractionPrompt}", $"{interactableArea.InteractName}");
                isPromptVisible = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out InteractableArea interactableArea))
        {
            currentArea = null;

            if (isPromptVisible)
            {
                promptUI.Hide();
                isPromptVisible = false;
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
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

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

                    if (!isPromptVisible)
                    {
                        promptUI.Show($"{interactableItem.InteractionPrompt}", $"{interactableItem.InteractableName}");
                        isPromptVisible = true;
                    }
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

                    if (isPromptVisible)
                    {
                        promptUI.Hide();
                        isPromptVisible = false;
                    }
                }

                return;
            }
        }

        // Hide prompt if not looking at interactable
        if (currentItem != null)
        {
            currentItem = null;
            promptUI.Hide();
            isPromptVisible = false;
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
