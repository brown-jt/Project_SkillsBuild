using UnityEngine;

public class RobotBuilder : MonoBehaviour
{
    [Header("Robot Parts")]
    [SerializeField] private GameObject torso;
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject leftArm;
    [SerializeField] private GameObject rightArm;
    [SerializeField] private GameObject leftLeg;
    [SerializeField] private GameObject rightLeg;

    [Header("Materials")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material outlineMaterial;

    private bool hasTorso = false;
    private bool hasHead = false;
    private bool hasLeftArm = false;
    private bool hasRightArm = false;
    private bool hasLeftLeg = false;
    private bool hasRightLeg = false;

    private void Awake()
    {
        RefreshRobot();
    }

    private void RefreshRobot()
    {
        SetPart(torso, hasTorso);
        SetPart(head, hasHead);
        SetPart(leftArm, hasLeftArm);
        SetPart(rightArm, hasRightArm);
        SetPart(leftLeg, hasLeftLeg);
        SetPart(rightLeg, hasRightLeg);
    }

    private void SetPart(GameObject part, bool hasPart)
    {
        if (part == null) return;

        var renderer = part.GetComponent<MeshRenderer>();
        if (renderer == null) return;

        if (hasPart)
        {
            // Normal visible part
            part.SetActive(true);
            renderer.material = normalMaterial;
        }
        else
        {
            // Flashing outline only
            part.SetActive(true);
            renderer.material = outlineMaterial;
        }
    }
}
