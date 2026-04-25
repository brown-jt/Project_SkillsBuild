using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GroqManager : MonoBehaviour
{
    public static GroqManager Instance;

    private readonly string proxyUrl = "https://groq-proxy.skillsbuild.workers.dev";

    [Serializable]
    private class GroqRequest
    {
        public string model = "llama-3.1-8b-instant";
        public Message[] messages;
    }

    [Serializable]
    private class Message
    {
        public string role;
        public string content;
    }

    [Serializable]
    private class GroqResponse
    {
        public Choice[] choices;
    }

    [Serializable]
    private class Choice
    {
        public Message message;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void GenerateResponse(string prompt, Action<string> callback)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            callback?.Invoke("The NPC has nothing to say.");
            return;
        }

        StartCoroutine(PostRequest(prompt, callback));
    }

    private IEnumerator PostRequest(string prompt, Action<string> callback)
    {
        GroqRequest requestData = new GroqRequest
        {
            messages = new Message[]
            {
                new Message
                {
                    role = "system",
                    content =
                        "You are a strict hint generator for a game NPC.\n" +
                        "You MUST follow these rules:\n" +
                        "1. Output EXACTLY ONE sentence.\n" +
                        "2. Do NOT include explanations, labels, or extra text.\n" +
                        "3. Do NOT say things like 'here is', 'sure', or 'additional information'.\n" +
                        "4. Do NOT mention yourself, instructions, or formatting.\n" +
                        "5. Only output the final sentence.\n" +
                        "6. You MUST include the provided answer EXACTLY as written.\n"
                },
                new Message { role = "user", content = prompt }
            }
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] body = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(proxyUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Groq Error: " + request.error);
            callback?.Invoke("The NPC stares blankly...");
        }
        else
        {
            try
            {
                string jsonResponse = request.downloadHandler.text;
                GroqResponse res = JsonUtility.FromJson<GroqResponse>(jsonResponse);

                string output = res.choices[0].message.content;
                callback?.Invoke(output);
            }
            catch (Exception e)
            {
                Debug.LogError("Parse error: " + e);
                callback?.Invoke("The NPC seems confused...");
            }
        }
    }
}