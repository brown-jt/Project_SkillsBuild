using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Conveyor : MonoBehaviour
{
    public enum ConveyorDirection
    {
        FORWARD,
        BACKWARD,
        LEFT,
        RIGHT
    }

    [Header("Conveyor Settings")]
    [SerializeField] private ConveyorDirection directionSetting = ConveyorDirection.FORWARD;
    [SerializeField] private float speed = 2f; // movement speed in units/sec

    private Vector3 direction;
    private BoxCollider movementZone;

    private Rigidbody currentItemRb; // only one item allowed

    private void Awake()
    {
        movementZone = GetComponent<BoxCollider>();
        movementZone.isTrigger = true;
        direction = GetDirectionVector();
    }

    private void OnValidate()
    {
        direction = GetDirectionVector();
    }

    private Vector3 GetDirectionVector()
    {
        return directionSetting switch
        {
            ConveyorDirection.FORWARD => Vector3.forward,   // world +Z
            ConveyorDirection.BACKWARD => Vector3.back,     // world -Z
            ConveyorDirection.LEFT => Vector3.left,         // world -X
            ConveyorDirection.RIGHT => Vector3.right,       // world +X
            _ => Vector3.forward
        };
    }


    private void FixedUpdate()
    {
        if (currentItemRb == null) return;

        // Constant movement along the conveyor direction
        Vector3 move = direction.normalized * speed * Time.fixedDeltaTime;
        currentItemRb.MovePosition(currentItemRb.position + move);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentItemRb != null) return; // conveyor already occupied
        if (!other.CompareTag("Item")) return; // not an item

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        currentItemRb = rb;
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (currentItemRb == rb)
        {
            currentItemRb = null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (currentItemRb != null) return; // conveyor already occupied

        if (!other.CompareTag("Item")) return; // not an item

        Rigidbody rb = other.attachedRigidbody; 
        if (rb == null) return;

        currentItemRb = rb; // new item can now enter after previous exited so lets set it
    }

    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null) return;

        Vector3 worldCenter = box.bounds.center;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(worldCenter, worldCenter + GetDirectionVector().normalized * 2f);
        Gizmos.DrawSphere(worldCenter, 0.05f);
    }
}
