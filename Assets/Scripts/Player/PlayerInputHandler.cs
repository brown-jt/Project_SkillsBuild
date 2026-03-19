using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputHandler : MonoBehaviour
{
    #region Editor Variables
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name Reference")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name References")]
    [SerializeField] private string movement = "Move";
    [SerializeField] private string rotation = "Look";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string interact = "Interact";
    [SerializeField] private string attack = "Attack";
    [SerializeField] private string drop = "Drop";
    #endregion

    private InputAction movementAction;
    private InputAction rotationAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction interactAction;
    private InputAction dropAction;
    public InputAction InteractAction => interactAction;
    private InputAction attackAction;

    public Vector2 MovementInput { get; private set; }
    public Vector2 RotationInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool SprintTriggered { get; private set; }
    public bool InteractTriggered { get; private set; }
    public bool AttackTriggered { get; private set; }
    public bool DropTriggered { get; private set; }

    // Singleton usage
    public static PlayerInputHandler Instance;

    /// <summary>
    /// When loading script we ensure we find the correct action map
    /// as well as associated actions from it that we care about
    /// </summary>
    private void Awake()
    {
        Instance = this;

        InputActionMap mapReference = playerControls.FindActionMap(actionMapName);

        movementAction = mapReference.FindAction(movement);
        rotationAction = mapReference.FindAction(rotation);
        jumpAction = mapReference.FindAction(jump);
        sprintAction = mapReference.FindAction(sprint);
        interactAction = mapReference.FindAction(interact);
        attackAction = mapReference.FindAction(attack);
        dropAction = mapReference.FindAction(drop);

        SubscribeActionValuesToInputEvents();
    }

    private void Update()
    {
        // One shot attack
        AttackTriggered = attackAction.WasPerformedThisFrame();

        // One shot interact
        InteractTriggered = interactAction.WasPerformedThisFrame();

        // One shot drop
        DropTriggered = dropAction.WasPerformedThisFrame();
    }

    /// <summary>
    /// Function to subscribe all values to their respective input events
    /// </summary>
    private void SubscribeActionValuesToInputEvents()
    {
        movementAction.performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
        movementAction.canceled += inputInfo => MovementInput = Vector2.zero;

        rotationAction.performed += inputInfo => RotationInput = inputInfo.ReadValue<Vector2>();
        rotationAction.canceled += inputInfo => RotationInput = Vector2.zero;

        jumpAction.performed += inputInfo => JumpTriggered = true;
        jumpAction.canceled += inputInfo => JumpTriggered = false;

        sprintAction.performed += inputInfo => SprintTriggered = true;
        sprintAction.canceled += inputInfo => SprintTriggered = false;
    }

    /// <summary>
    /// Helper function to enable the action map when required
    /// </summary>
    public void EnablePlayerInput()
    {
        playerControls.FindActionMap(actionMapName).Enable();
    }

    /// <summary>
    /// Helper function to disable the action map when required
    /// </summary>
    public void DisablePlayerInput()
    {
        playerControls.FindActionMap(actionMapName).Disable();

        // Clear any cached input so character doesn't keep moving upon interaction
        MovementInput = Vector2.zero;
        RotationInput = Vector2.zero;
        JumpTriggered = false;
        SprintTriggered = false;
        InteractTriggered = false;
        AttackTriggered = false;
    }
}
