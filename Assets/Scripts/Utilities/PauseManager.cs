using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject helpMenuUI;
    [SerializeField] private InputActionReference pauseAction;

    private bool isPaused = false;
    private bool isHelpMenuOpen = false;

    private void Start()
    {
        pauseMenuUI.SetActive(false);
        helpMenuUI.SetActive(false);
    }

    private void Update()
    {
        if (isPaused)
        {
            // Ensure the cursor remains visible and unlocked while paused
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            FirstPersonController.Instance.SetCameraLookLocked(true);
        }
    }

    private void OnEnable()
    {
        pauseAction.action.Enable();
        pauseAction.action.performed += ToggleMenu;
    }

    private void OnDisable()
    {
        pauseAction.action.performed -= ToggleMenu;
        pauseAction.action.Disable();
    }

    private void ToggleMenu(InputAction.CallbackContext ctx)
    {
        Debug.Log("Pause button pressed");
        if (isHelpMenuOpen)
        {
            Debug.Log("Help menu is open, closing it instead of toggling pause");
            // If help menu is open, close it instead of toggling pause
            CloseHelpMenu();
            return;
        }

        if (isPaused)
        {
            ResumeGame();
            Debug.Log("Resuming game");
        }

        else
        {
            PauseGame();
            Debug.Log("Pausing game");
        }
    }

    private void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        FirstPersonController.Instance.SetCameraLookLocked(true);
        pauseMenuUI.SetActive(true);
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        FirstPersonController.Instance.SetCameraLookLocked(false);
        pauseMenuUI.SetActive(false);
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenHelpMenu()
    {
        pauseMenuUI.SetActive(false);
        helpMenuUI.SetActive(true);
        isHelpMenuOpen = true;
    }

    public void CloseHelpMenu()
    {
        helpMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        isHelpMenuOpen = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
