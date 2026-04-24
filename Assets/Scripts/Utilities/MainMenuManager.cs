using UnityEngine;

public class MainMenuManager : MonoBehaviour
{ 
    [SerializeField] private GameObject GameUI;
    [SerializeField] private GameObject MainMenuUI;
    void Start()
    {
        PlayerInputHandler.Instance.DisablePlayerInput();
        FirstPersonController.Instance.SetCameraLookLocked(true);
        GameUI.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerInputHandler.Instance.EnablePlayerInput();
        FirstPersonController.Instance.SetCameraLookLocked(false);
        GameUI.SetActive(true);
        MainMenuUI.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
