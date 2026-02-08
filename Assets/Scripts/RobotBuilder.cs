using UnityEngine;

public class RobotBuilder : InteractableArea
{
    [Header("Robot In-Game Model Parts")]
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject torso;
    [SerializeField] private GameObject leftArm;
    [SerializeField] private GameObject rightArm;
    [SerializeField] private GameObject leftLeg;
    [SerializeField] private GameObject rightLeg;

    [Header("Robot Parts ItemData")]
    [SerializeField] private ItemData robotHeadItemData;
    [SerializeField] private ItemData robotTorsoItemData;
    [SerializeField] private ItemData robotArmItemData;
    [SerializeField] private ItemData robotLegItemData;

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

        TryAddPart();

        if (HasAllParts())
        {
            Debug.Log("Robot is fully assembled!");

            string targetId = GetComponent<QuestTarget>()?.targetId ?? "unknown";
            QuestEvents.ItemBuilt(targetId);

            IsInteractable = false;
        }
    }

    // Debug helper function to test addition of parts
    private void TryAddPart()
    {
        if (!hasHead && HaveItem(robotHeadItemData))
        {
            AddPart("head");
            AudioSource.PlayClipAtPoint(InteractionSound, torso.transform.position);
        }
        else if (!hasTorso && HaveItem(robotTorsoItemData))
        {
            AddPart("torso");
            AudioSource.PlayClipAtPoint(InteractionSound, torso.transform.position);
        }
        else if ((!hasLeftArm || !hasRightArm) && HaveItem(robotArmItemData))
        {
            AddPart("arm");
            AudioSource.PlayClipAtPoint(InteractionSound, torso.transform.position);
        }
        else if ((!hasLeftLeg || !hasRightLeg) && HaveItem(robotLegItemData))
        {
            AddPart("leg");
            AudioSource.PlayClipAtPoint(InteractionSound, torso.transform.position);
        }
    }

    private bool HaveItem(ItemData requiredItem)
    {
        if (InventoryManager.Instance.HasItem(requiredItem, 1))
        {
            InventoryManager.Instance.RemoveItem(requiredItem, 1);
            return true;
        }

        Debug.Log($"Missing required part: {requiredItem.itemName}");
        return false;
    }
}
