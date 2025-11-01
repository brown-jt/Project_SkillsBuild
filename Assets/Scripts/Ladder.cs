using UnityEngine;

public class Ladder : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out FirstPersonController controller))
        {
            controller.SetClimbing(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out FirstPersonController controller))
        {
            controller.SetClimbing(false);
        }
    }
}
