using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class ConveyorItem : InteractableItem
{
    private Rigidbody rb;

    private Conveyor currentConveyor;
    private Conveyor nextConveyor;

    private bool centering = false;
    private Vector3 targetCenterPos;

    [Header("Centering Force Settings")]
    [SerializeField] private float centeringThreshold = 0.05f;
    [SerializeField] private float centeringSpeedMultiplier = 1.5f;

    [Header("Item Data")]
    [SerializeField] private ItemData itemData;
    [SerializeField] private int amount = 1;

    private bool isMoveable = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (itemData != null)
        {
            SetInteractableName(itemData.itemName);
        }
        else
        {
            SetInteractableName("No Item Data");
        }
    }

    public void OnEnterConveyor(Conveyor conveyor)
    {
        // First time entering any conveyor
        if (currentConveyor == null)
        {
            currentConveyor = conveyor;
            return;
        }

        // Entered a new conveyor while still on another
        if (conveyor != currentConveyor)
        {
            nextConveyor = conveyor;
        }
    }

    public void OnExitConveyor(Conveyor conveyor)
    {
        if (conveyor == currentConveyor)
        {
            if (nextConveyor != null)
            {
                StartCentering(nextConveyor);
                currentConveyor = nextConveyor;
                nextConveyor = null;
            }
            else
            {
                currentConveyor = null;
                centering = false;
            }
        }
        else if (conveyor == nextConveyor)
        {
            nextConveyor = null;
        }
    }

    private void StartCentering(Conveyor newConveyor)
    {
        centering = true;
        targetCenterPos = GetCenterlineTarget(newConveyor);
    }

    /// <summary>
    /// Finds the world-space point on the centerline of the given conveyor closest to this item.
    /// </summary>
    private Vector3 GetCenterlineTarget(Conveyor conveyor)
    {
        var box = conveyor.GetComponent<BoxCollider>();
        Vector3 boxCenter = box.bounds.center;

        // Conveyor world-space forward and right
        Vector3 forward = conveyor.GetDirectionVector();
        Vector3 up = Vector3.up;
        Vector3 right = Vector3.Cross(up, forward).normalized;

        Vector3 itemPos = transform.position;

        // Remove world-space lateral offset relative to conveyor width
        Vector3 lateralOffset = Vector3.Project(itemPos - boxCenter, right);
        return itemPos - lateralOffset;
    }

    private void FixedUpdate()
    {
        if (currentConveyor == null) return;

        Vector3 moveDir;
        float moveSpeed = currentConveyor.Speed;

        if (centering)
        {
            Vector3 toCenter = targetCenterPos - transform.position;

            if (toCenter.magnitude < centeringThreshold)
            {
                centering = false;
                moveDir = currentConveyor.GetDirectionVector();
            }
            else
            {
                moveDir = toCenter.normalized;
                moveSpeed *= centeringSpeedMultiplier;
            }
        }
        else
        {
            moveDir = currentConveyor.GetDirectionVector();
        }

        // Move in world space
        if (isMoveable)
        {
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
        }
    }

    public void ToggleMovement(bool b)
    {
        isMoveable = b;
    }

    public override void Interact()
    {
        bool added = InventoryManager.Instance.AddItem(itemData, amount);

        if (added)
        {
            AudioSource.PlayClipAtPoint(InteractionSound, transform.position);
            Destroy(gameObject);
            FeedbackNotificationsUI.Instance.AddNotification($"{amount}x {itemData.itemName} added to inventory");
        }
        else
        {
            FeedbackNotificationsUI.Instance.AddNotification("Unable to pickup. Inventory full.");
        }
    }
}
