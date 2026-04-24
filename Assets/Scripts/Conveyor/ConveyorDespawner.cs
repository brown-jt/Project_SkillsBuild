using UnityEngine;

public class ConveyorDespawner : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item") || other.CompareTag("Robot"))
        {
            Destroy(other.gameObject);
        }
    }
}
