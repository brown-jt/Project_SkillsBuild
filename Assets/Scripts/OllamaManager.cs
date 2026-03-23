using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class OllamaManager : MonoBehaviour
{
    public static OllamaManager Instance;

    private readonly string url = "http://localhost:11434/api/generate";
    private readonly string model = "ibm/granite4:1b";

    [Serializable]
    public class OllamaResponse
    {
        public string response;
    }

    // New class to represent the request to Ollama
    [Serializable]
    private class OllamaRequest
    {
        public string model;
        public string prompt;
        public bool stream = false;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GenerateResponse(string prompt, Action<string> callback)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            Debug.LogWarning("OllamaManager: Prompt is empty. Skipping LLM request.");
            callback?.Invoke("The NPC has nothing to say.");
            return;
        }

        StartCoroutine(PostRequest(prompt, callback));
    }

    private IEnumerator PostRequest(string prompt, Action<string> callback)
    {
        // Build request object and serialize to JSON safely
        OllamaRequest requestData = new OllamaRequest
        {
            model = model,
            prompt = prompt
        };

        string json = JsonUtility.ToJson(requestData);

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Ollama Error: " + request.error + " | Response: " + request.downloadHandler.text);
            callback?.Invoke("The NPC stares blankly at you. (OLLAMA Error)");
        }
        else
        {
            try
            {
                OllamaResponse res = JsonUtility.FromJson<OllamaResponse>(request.downloadHandler.text);
                callback?.Invoke(res.response);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to parse Ollama response: " + e);
                callback?.Invoke("The NPC seems confused. (Parsing Error)");
            }
        }
    }
}