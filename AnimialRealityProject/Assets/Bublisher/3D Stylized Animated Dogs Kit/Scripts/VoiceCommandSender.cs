using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class VoiceCommandSender : MonoBehaviour
{
    public string fileName = "voice_command.wav";
    public string whisperServerUrl = "http://127.0.0.1:5000/transcribe";

    public void SendToWhisper()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            StartCoroutine(SendAudioFile(path));
        }
        else
        {
            Debug.LogError($"Audio file not found at: {path}");
        }
    }

    IEnumerator SendAudioFile(string filePath)
    {
        Debug.Log("Sending audio to Whisper server...");

        byte[] audioData = File.ReadAllBytes(filePath);
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", audioData, "voice_command.wav", "audio/wav");

        UnityWebRequest www = UnityWebRequest.Post(whisperServerUrl, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Whisper request failed: " + www.error);
        }
        else
        {
            string response = www.downloadHandler.text;
            Debug.Log("Whisper response: " + response);

            // Optional: parse the response and act on the command
            WhisperResult result = JsonUtility.FromJson<WhisperResult>(response);
            HandleVoiceCommand(result.text.ToLower().Trim());
        }
    }

    void HandleVoiceCommand(string command)
    {
        Debug.Log("🎤 Heard: " + command);

        DogMovement dog = FindObjectOfType<DogMovement>();

        if (dog == null)
        {
            Debug.LogError("🐶 DogMovement not found in scene!");
            return;
        }

       if (command.Contains("sit"))
        {
            Debug.Log("🐾 Command: SIT");
            dog.TransitionToIdle();
            dog.MakeDogSit();
        }
        else if (command.Contains("run"))
        {
            Debug.Log("🐾 Command: RUN");
            dog.TransitionToIdle();
            dog.MakeDogRun();
        }
        else if (command.Contains("wag"))
        {
            Debug.Log("🐾 Command: WAG TAIL");
            dog.TransitionToIdle();
            dog.MakeDogWagTail();
        }
        else if (command.Contains("walk"))
        {
            Debug.Log("🐾 Command: WALK");
            dog.TransitionToIdle();
            dog.MakeDogWalk();
        }       
        else if (command.Contains("move left"))
        {
            Debug.Log("Dog moves left.");
            dog.TransitionToIdle();
            dog.MoveInDirection(Vector3.right);
        }
        else if (command.Contains("move right"))
        {
            Debug.Log("Dog moves right.");
            dog.TransitionToIdle();
            dog.MoveInDirection(Vector3.left);
        }
        else if (command.Contains("move forward"))
        {
            Debug.Log("Dog moves forward.");
            dog.TransitionToIdle();
            dog.MoveInDirection(Vector3.back);
        }
        else if (command.Contains("move backward"))
        {
            Debug.Log("Dog moves backward.");
            dog.TransitionToIdle();
            dog.MoveInDirection(Vector3.forward);
        }
        else
        {
            Debug.Log("❗ Unrecognized command.");
        }
    }


    [System.Serializable]
    public class WhisperResult
    {
        public string text;
    }
}
