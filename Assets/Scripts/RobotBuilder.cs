using UnityEngine;

public class RobotBuilder : InteractableArea
{
    [Header("Robot Parts")]
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject torso;
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

    private void AddPart(string partName)
    {
        switch (partName)
        {
            case "head":
                hasHead = true;
                break;
            case "torso": 
                hasTorso = true;
                break;
            case "arm": 
                if (!hasLeftArm) hasLeftArm = true;
                else hasRightArm = true;
                break;
            case "leg":
                if (!hasLeftLeg) hasLeftLeg = true;
                else hasRightLeg = true;
                break;
            default: 
                break;
        }

        RefreshRobot();
    }

    private bool HasAllParts()
    {
        return hasHead && hasTorso && hasLeftArm && hasRightArm && hasLeftLeg && hasRightLeg;
    }

    public override void Interact()
    {
        if (!IsInteractable) return;

        TestPartAddition();

        if (HasAllParts())
        {
            Debug.Log("Robot is fully assembled!");
            IsInteractable = false;
        }
    }

    // Debug helper function to test addition of parts
    private void TestPartAddition()
    {
        if (!hasHead) AddPart("head");
        else if (!hasTorso) AddPart("torso");
        else if (!hasLeftArm || !hasRightArm) AddPart("arm");
        else if (!hasLeftLeg || !hasRightLeg) AddPart("leg");

        if (InteractionSound != null)
        {
            // Play sound just at center of robot for now
            AudioSource.PlayClipAtPoint(InteractionSound, torso.transform.position);
        }
    }
}
