using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleport : InteractableArea
{
    [SerializeField]
    private string sceneName;
    public override void Interact()
    {
        SceneManager.LoadScene(sceneName);
    }
}
