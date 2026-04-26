using UnityEngine;

public abstract class HoldableItem : InteractableItem
{
    [Header("Holdable Settings")]
    [SerializeField] private Vector3 localPositionOffset = Vector3.zero;
    [SerializeField] private Vector3 localRotationOffset = Vector3.zero;

    private Transform holdParent; // The transform where the item will be held
    private bool isHeld = false;

    public bool IsHeld => isHeld;

    /// <summary>
    /// Called by the player to pick up the item.
    /// </summary>
    public void PickUp(Transform holdPoint)
    {
        if (isHeld)
            return;

        if (holdPoint == null)
        {
            Debug.LogError($"HoldableItem {gameObject.name} requires a transform point for pickup.");
            return;
        }

        holdParent = holdPoint;

        // Attach to player's hand
        transform.SetParent(holdParent);
        transform.localPosition = localPositionOffset;
        transform.localEulerAngles = localRotationOffset;
        transform.localScale = Vector3.one;

        // Disable physics
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        isHeld = true;

        // Play feedback
        AudioManager.Instance.PlaySFX("Pickup_Box");

        OnPickedUp();
    }

    /// <summary>
    /// Called by the player to drop the item.
    /// </summary>
    public void Drop()
    {
        if (!isHeld)
            return;

        // Detach from player
        transform.SetParent(null);

        // Re-enable physics
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }

        isHeld = false;

        OnDropped();
    }

    protected virtual void OnPickedUp()
    {
        // Override in derived classes for custom pickup behavior
    }

    protected virtual void OnDropped()
    {
        // Override in derived classes for custom drop behavior
    }

    /// <summary>
    /// Override Interact to do nothing on its own; the player controls pickup/drop
    /// </summary>
    public override void Interact()
    {
        // Interaction is handled by PlayerInteract
    }
}