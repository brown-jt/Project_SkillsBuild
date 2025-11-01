using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    #region Editor Variables
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;
    [SerializeField] private float climbSpeed = 2.0f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravityMultiplier = 1.0f;

    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float upDownLookRange = 80f;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private Animator animator;
    #endregion

    private Vector3 currentMovement;
    private float verticalRotation;
    private float CurrentSpeed => walkSpeed * (playerInputHandler.SprintTriggered ? sprintMultiplier : 1);

    private bool isClimbing = false;

    //private bool iFramesActive = false;
    //private float iFramesTimer = 0.5f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Ensure animator knows we are grounded and speed 0 at start
        //animator.SetBool("IsGrounded", true);
        animator.SetFloat("Speed", 0f);
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    /// <summary>
    /// Helper function to calculate world direction
    /// </summary>
    /// <returns>Normalized world direction</returns>
    private Vector3 CalculateWorldDirection()
    {
        Vector3 inputDirection = new Vector3(playerInputHandler.MovementInput.x, 0f, playerInputHandler.MovementInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        return worldDirection.normalized;
    }

    /// <summary>
    /// Helper function to handle jumping used in HandleMovement
    /// </summary>
    private void HandleJumping()
    {
        if (characterController.isGrounded)
        {
            //animator.SetBool("IsJumping", false);
            currentMovement.y = -0.5f;
            if (playerInputHandler.JumpTriggered)
            {
                currentMovement.y = jumpForce;

                // Also fire the animator Jump trigger
                //animator.SetTrigger("Jump");
            }
        }
        else
        {
            //animator.SetBool("IsJumping", true);
            currentMovement.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }
    }

    /// <summary>
    /// Main function to handle movement of a player character
    /// </summary>
    private void HandleMovement()
    {
        Vector3 worldDirection = CalculateWorldDirection();
        currentMovement.x = worldDirection.x * CurrentSpeed;
        currentMovement.z = worldDirection.z * CurrentSpeed;

        if (isClimbing)
        {
            // Override vertical movement when climbing
            currentMovement.y = playerInputHandler.MovementInput.y * climbSpeed;
        }
        else
        {
            // Handle jumping as part of movement if not climbing
            HandleJumping();
        }

        characterController.Move(currentMovement * Time.deltaTime);

        // Updating animator to match movement
        // Ensuring speed is normalized and lerped to allow for smooth animation transitions
        float maxSpeed = walkSpeed * sprintMultiplier;
        float speed = new Vector3(currentMovement.x, 0f, currentMovement.z).magnitude;
        float normalizedSpeed = speed / maxSpeed;
        float lerpedSpeed = Mathf.Lerp(animator.GetFloat("Speed"), normalizedSpeed, Time.deltaTime * 10f);

        animator.SetFloat("Speed", lerpedSpeed);
        //animator.SetBool("IsGrounded", characterController.isGrounded);
    }

    /// <summary>
    /// Helper function to apply horizontal rotation
    /// </summary>
    /// <param name="rotationAmount">Amount of rotation to apply</param>
    private void ApplyHorizontalRotation(float rotationAmount)
    {
        transform.Rotate(0, rotationAmount, 0);
    }

    /// <summary>
    /// Helper function to apply vertical rotation
    /// </summary>
    /// <param name="rotationAmount">Amount of rotation to apply</param>
    private void ApplyVerticalRotation(float rotationAmount)
    {
        verticalRotation = Mathf.Clamp(verticalRotation - rotationAmount, -upDownLookRange, upDownLookRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    /// <summary>
    /// Main function to handle both horizontal and vertical rotations
    /// </summary>
    private void HandleRotation()
    {
        float mouseXRotation = playerInputHandler.RotationInput.x * mouseSensitivity;
        float mouseYRotation = playerInputHandler.RotationInput.y * mouseSensitivity;

        ApplyHorizontalRotation(mouseXRotation);
        ApplyVerticalRotation(mouseYRotation);
    }

    /// <summary>
    /// Public access function to set climbing state (called from Ladder trigger)
    /// </summary>
    /// <param name="val"></param>
    public void SetClimbing(bool val)
    {
        isClimbing = val;
        if (isClimbing)
        {
            // Reset vertical movement when starting to climb
            currentMovement.y = 0f;
        }
    }
}
