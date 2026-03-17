using UnityEngine;

public class Shelf : InteractableItem
{
    private PlayerInteract playerInteract;

    private void Start()
    {
        playerInteract = FindFirstObjectByType<PlayerInteract>();
        IsInteractable = false; // Initially, the shelf is not interactable as player won't be holding an item
    }

    private void Update()
    {
        if (playerInteract.IsHoldingItem)
        {
            IsInteractable = true;
        }
        else
        {
            IsInteractable = false;
        }
    }

    public override void Interact()
    {
        if (IsInteractable)
        {
            // Safeguard to ensure we are holding item before trying to "deposit" on the shelf
            if (playerInteract.IsHoldingItem)
            {
                // Drop item on shelf - for now we just destroy it, but this is where I could update UI to show item on shelf or something like that
                HoldableItem tempItem = playerInteract.HeldItem;
                playerInteract.DropHeldItem();
                DestroyImmediate(tempItem.gameObject);
            }
        }
    }
}
