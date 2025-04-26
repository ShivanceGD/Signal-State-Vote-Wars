using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Text;

public class HuggingFaceTextToSpeech : MonoBehaviour
{
    public string huggingFaceApiKey = "YOUR_HUGGINGFACE_API_KEY";
    public string textGenModel = "gpt2";
    public string ttsModel = "espnet/kan-bayashi_ljspeech_vits";
    public TMP_InputField promptInput;
    public AudioSource audioSource;

    public void Generate()
    {
        StartCoroutine(GenerateText(promptInput.text));
    }

    IEnumerator GenerateText(string prompt)
    {
        var data = new { inputs = prompt };
        string json = JsonUtility.ToJson(data);

        UnityWebRequest request = UnityWebRequest.PostWwwForm($"https://api-inference.huggingface.co/models/{textGenModel}", json);
        byte[] body = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {huggingFaceApiKey}");
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string generatedText = ExtractGeneratedText(request.downloadHandler.text);
            StartCoroutine(GenerateSpeech(generatedText));
        }
        else Debug.LogError("Text Error: " + request.error);
    }

    IEnumerator GenerateSpeech(string text)
    {
        var data = new { inputs = text };
        string json = JsonUtility.ToJson(data);

        UnityWebRequest request = UnityWebRequest.PostWwwForm($"https://api-inference.huggingface.co/models/{ttsModel}", json);
        byte[] body = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {huggingFaceApiKey}");
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            /*byte[] audioBytes = request.downloadHandler.data;
            AudioClip clip = WavUtility.ToAudioClip(audioBytes, 0, "AI Speech");
            audioSource.clip = clip;
            audioSource.Play();*/
        }
        else Debug.LogError("Speech Error: " + request.error);
    }

    string ExtractGeneratedText(string json)
    {
        int start = json.IndexOf("\"generated_text\":\"") + 18;
        int end = json.IndexOf("\"", start);
        return json.Substring(start, end - start);
    }
}
