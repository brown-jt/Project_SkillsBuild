using UnityEngine;

public class GroqTestRunner : MonoBehaviour
{
    private void Start()
    {
        GroqManager.Instance.GenerateResponse(
            "Say hello in exactly one short sentence.",
            OnResponse
        );
    }

    private void OnResponse(string response)
    {
        Debug.Log("GROQ RESPONSE: " + response);
    }
}
