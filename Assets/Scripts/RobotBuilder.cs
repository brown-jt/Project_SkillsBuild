using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Robot Refs")]
    [SerializeField] private RobotStaticMover rsm;
    [SerializeField] private GameObject replacementRobot;
    [SerializeField] private Transform[] movementPoints;

    [Header("Related Quest")]
    [SerializeField] private QuestData relatedQuest;

    private GameObject oldRobot;

    private bool hasTorso = false;
    private bool hasHead = false;
    private bool hasLeftArm = false;
    private bool hasRightArm = false;
    private bool hasLeftLeg = false;
    private bool hasRightLeg = false;
    private bool IsComplete => hasTorso && hasHead && hasLeftArm && hasRightArm && hasLeftLeg && hasRightLeg;

    private void Awake()
    {
        RefreshRobot();
    }

    private void Start()
    {
        rsm.GetComponent<Animator>().enabled = false; // Start with animator disabled until complete
        if (QuestManager.Instance.IsQuestCompleted(relatedQuest))
        {
            hasHead = true;
            hasTorso = true;
            hasLeftArm = true;
            hasRightArm = true;
            hasLeftLeg = true;
            hasRightLeg = true;
        }
        RefreshRobot();
    }

    private void Update()
    {
        if (gameObject.scene != SceneManager.GetActiveScene()) return;

        // If related quest not active we won't make it interactable
        if (!QuestManager.Instance.activeQuests.Any(q => q.questData.questId == relatedQuest.questId)) {
            IsInteractable = false;
        }

        if (oldRobot != null && oldRobot.Equals(null))
        {
            oldRobot = null;
        }

        if (IsComplete && IsInteractable)
        {
            IsInteractable = false;
            rsm.GetComponent<Animator>().enabled = true; // Enable animator when robot is complete
        }

        if (IsComplete && !rsm.IsBuilt && oldRobot == null && replacementRobot.name == "Small_Robot_Dissolve")
        {
            Debug.Log("Robot complete! Starting to walk...");
            var resetPos = rsm.gameObject.transform.position;
            var resetRot = rsm.gameObject.transform.rotation;
            rsm.Build();
            oldRobot = rsm.gameObject;
            StartCoroutine(DelayedReplacement(resetPos, resetRot));
        }
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

        AudioManager.Instance.PlaySFX("Pickup_Item");

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
        }
        else if (!hasTorso && HaveItem(robotTorsoItemData))
        {
            AddPart("torso");
        }
        else if ((!hasLeftArm || !hasRightArm) && HaveItem(robotArmItemData))
        {
            AddPart("arm");
        }
        else if ((!hasLeftLeg || !hasRightLeg) && HaveItem(robotLegItemData))
        {
            AddPart("leg");
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

    IEnumerator DelayedReplacement(Vector3 position, Quaternion rotation)
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second before replacing
        if (replacementRobot != null)
        {
            var newRobot = Instantiate(replacementRobot, position, rotation, gameObject.transform.parent);
            rsm = newRobot.GetComponentInChildren<RobotStaticMover>(); // Update reference to new robot
            rsm.SetMovementPoints(movementPoints); // Set movement points for new robot
        }
    }
}
