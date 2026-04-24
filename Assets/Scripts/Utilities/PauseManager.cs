using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject helpMenuUI;
    [SerializeField] private GameObject settingsMenuUI;
    [SerializeField] private InputActionReference pauseAction;

    private bool isPaused = false;
    private bool isHelpMenuOpen = false;
    private bool isSettingsMenuOpen = false;

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
        if (!PlayerInputHandler.Instance.IsPlayerInputEnabled()) return;

        if (isHelpMenuOpen)
        {
            // If help menu is open, close it instead of toggling pause
            CloseHelpMenu();
            return;
        }

        else if (isSettingsMenuOpen)
        {
            // If settings menu is open, close it instead of toggling pause
            CloseSettingsMenu();
            return;
        }

        else if (isPaused) ResumeGame();

        else PauseGame();
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

    public void OpenSettingsMenu()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
        isSettingsMenuOpen = true;
    }

    public void CloseSettingsMenu()
    {
        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        isSettingsMenuOpen = false;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
