using UnityEngine;
using System.Collections;

public class CameraFocusController : MonoBehaviour
{
    public float transitionSpeed = 5f;

    private Transform originalParent;
    private Transform newParent;
    private Vector3 originalPos;
    private Quaternion originalRot;
    private bool isFocusing;

    private int focusChain = 0;

    private void LateUpdate()
    {
        if (!isFocusing || newParent == null) return;

        Vector3 euler = transform.eulerAngles;
        euler.x = newParent.eulerAngles.x;
        transform.eulerAngles = euler;
    }


    private void Start()
    {
        originalParent = transform.parent;
    }

    public void FocusOnTerminal(Transform target)
    {
        newParent = target;
        StartCoroutine(FocusRoutine());
    }

    public void ReturnToPlayer()
    {
        if (!isFocusing) return;

        isFocusing = false;
        focusChain = 0;
        StartCoroutine(ReturnRoutine());
    }

    private IEnumerator FocusRoutine()
    {
        isFocusing = true;
        focusChain++;

        if (focusChain == 1)
        {
            originalParent = transform.parent;
            originalPos = transform.position;
            originalRot = transform.rotation;
        }

        transform.parent = null;

        float t = 0f;

        Vector3 startPos = transform.position;
        Vector3 targetPos = newParent.position;

        Quaternion startRot = transform.rotation;
        Quaternion targetRot = newParent.rotation;

        Vector3 euler = targetRot.eulerAngles;
        euler.z = 0f;
        targetRot = Quaternion.Euler(euler);

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        transform.position = newParent.position;
        transform.rotation = targetRot;
        transform.parent = newParent;
    }

    private IEnumerator ReturnRoutine()
    {
        transform.parent = null;

        float t = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = originalPos;

        Quaternion startRot = transform.rotation;
        Quaternion targetRot = originalRot;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        transform.position = originalPos;
        transform.rotation = originalRot;
        transform.parent = originalParent;
    }
}
