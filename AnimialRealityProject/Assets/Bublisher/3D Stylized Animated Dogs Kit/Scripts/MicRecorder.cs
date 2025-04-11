using UnityEngine;
using System.IO;
using System.Collections;

public class MicRecorder : MonoBehaviour
{
    public string fileName = "voice_command.wav";
    private AudioClip recording;
    private bool isRecording = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartRecording();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            StopRecording();
        }
    }

    void StartRecording()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone detected!");
            return;
        }

        Debug.Log("🎙 Recording started...");
        recording = Microphone.Start(null, false, 5, 16000); // 5 seconds max, 16kHz
        isRecording = true;
    }

    void StopRecording()
    {
        if (!isRecording) return;

        Microphone.End(null);
        Debug.Log("🛑 Recording stopped.");

        string path = Path.Combine(Application.persistentDataPath, fileName);
        SaveWavFile(path, recording);
        Debug.Log($"📁 Saved to: {path}");

        // TODO: Call the Whisper Python server here
        GetComponent<VoiceCommandSender>().SendToWhisper();

        isRecording = false;
    }

    void SaveWavFile(string filepath, AudioClip clip)
    {
        string filename = Path.GetFileName(filepath);
        SavWav.Save(filename, clip);
    }
}
