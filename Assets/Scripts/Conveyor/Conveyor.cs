using System.Collections.Generic;
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
    [SerializeField] private float speed = 10f;

    private readonly HashSet<ConveyorItem> itemsOnConveyor = new();

    public float Speed => speed;

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    /// <summary>
    /// Returns the conveyor direction in WORLD SPACE.
    /// </summary>
    public Vector3 GetDirectionVector()
    {
        return directionSetting switch
        {
            ConveyorDirection.FORWARD => Vector3.forward,
            ConveyorDirection.BACKWARD => Vector3.back,
            ConveyorDirection.LEFT => Vector3.left,
            ConveyorDirection.RIGHT => Vector3.right,
            _ => Vector3.forward
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Item")) return;

        var item = other.GetComponent<ConveyorItem>();
        if (item == null) return;

        if (itemsOnConveyor.Add(item))
        {
            item.OnEnterConveyor(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Item")) return;

        var item = other.GetComponent<ConveyorItem>();
        if (item == null) return;

        if (itemsOnConveyor.Remove(item))
        {
            item.OnExitConveyor(this);
        }
    }

    private void OnDrawGizmos()
    {
        var box = GetComponent<BoxCollider>();
        if (box == null) return;

        Vector3 worldCenter = box.bounds.center;
        Vector3 dir = GetDirectionVector().normalized;
        float length = 1f;

        Vector3 end = worldCenter + dir * length;

        // Draw main line
        Gizmos.color = Color.green;
        Gizmos.DrawLine(worldCenter, end);
        Gizmos.DrawSphere(worldCenter, 0.05f);

        // Draw arrowhead
        float arrowHeadAngle = 25f;
        float arrowHeadLength = 0.2f;

        // Calculate arrowhead direction lines
        Vector3 right = Quaternion.LookRotation(dir) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(dir) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;

        Gizmos.DrawLine(end, end + right * arrowHeadLength);
        Gizmos.DrawLine(end, end + left * arrowHeadLength);
    }
}
