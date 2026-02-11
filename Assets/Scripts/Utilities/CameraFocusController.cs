using UnityEngine;
using System.Collections;

public class CameraFocusController : MonoBehaviour
{
    public float transitionSpeed = 5f;

    private Transform originalParent;
    private Vector3 originalPos;
    private Quaternion originalRot;
    private bool isFocusing;

    public void FocusOnTerminal(Transform target)
    {
        if (isFocusing) return;
        StartCoroutine(FocusRoutine(target));
    }

    public void ReturnToPlayer()
    {
        if (!isFocusing) return;
        StartCoroutine(ReturnRoutine());
    }

    private IEnumerator FocusRoutine(Transform target)
    {
        isFocusing = true;

        originalParent = transform.parent;
        originalPos = transform.position;
        originalRot = transform.rotation;

        transform.parent = null;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            transform.position = Vector3.Lerp(originalPos, target.position, t);
            transform.rotation = Quaternion.Slerp(originalRot, target.rotation, t);
            yield return null;
        }
    }

    private IEnumerator ReturnRoutine()
    {
        float t = 0f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            transform.position = Vector3.Lerp(startPos, originalPos, t);
            transform.rotation = Quaternion.Slerp(startRot, originalRot, t);
            yield return null;
        }

        transform.parent = originalParent;
        isFocusing = false;
    }
}
