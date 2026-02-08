using UnityEngine;

[RequireComponent (typeof(SphereCollider))]
public class QuestExploreTrigger : MonoBehaviour
{
    private QuestTarget target;

    private void Awake()
    {
        target = GetComponent<QuestTarget>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Trigger the event when explored (i.e., entered the radius)
        QuestEvents.AreaExplored(target.targetId);

        // Destroy to prevent double triggers
        Destroy(this);
    }
}
