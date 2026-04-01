using UnityEngine;

public class AutoCenterChildren : MonoBehaviour
{
    public float spacing = 0.3f;

    public void Recenter()
    {
        int activeCount = 0;

        // Count active children
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy)
                activeCount++;
        }

        if (activeCount == 0) return;

        // Calculate starting position so it's centered
        float startZ = -(activeCount - 1) * spacing * 0.5f;

        int index = 0;

        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeInHierarchy)
                continue;

            Vector3 pos = child.localPosition;

            pos.z = startZ + index * spacing;

            child.localPosition = pos;

            index++;
        }
    }
}