using UnityEngine;

public class MainMenuManager : MonoBehaviour
{ 
    [SerializeField] private GameObject GameUI;
    [SerializeField] private GameObject MainMenuUI;
    void Start()
    {
        PlayerInputHandler.Instance.DisablePlayerInput();
        PlayerInputHandler.Instance.DisablePlayerUIInput();
        FirstPersonController.Instance.SetCameraLookLocked(true);
        GameUI.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        MainMenuUI.SetActive(true);
    }

    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerInputHandler.Instance.EnablePlayerInput();
        PlayerInputHandler.Instance.EnablePlayerUIInput();
        FirstPersonController.Instance.SetCameraLookLocked(false);
        GameUI.SetActive(true);
        MainMenuUI.SetActive(false);
        AudioManager.Instance.PlayMusic("Hub_Music");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
