using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using Meta.XR.MRUtilityKitSamples.BouncingBall;
using System;
using Newtonsoft.Json.Linq;



public class WitVoiceCommandHandler : MonoBehaviour
{
    public string witAccessToken = "QREEJDRRAQ4PZSUEOBDDDCVU6GGT4XX5"; // replace this with your copied token
    public DogMovement dog; // Assign the dog GameObject that has the DogMovement script
    public Transform player; // Assign this in the inspector (like Main Camera or XR Rig)
    public Transform tennisBall;
    public Transform curryPlate;
    public float stoppingDistanceFromPlayer = 0.2f;
    private string witApiUrl = "https://api.wit.ai/speech?v=20230202";
    private const float TRESHOLD_FOR_VOICE_DETECTION = 0.01f;
    private AudioClip recording;
    private const int SAMPLE_RATE = 16000;
    private string micName;
    private BouncingcBallMgr bouncingcBallMgr;

    public SpeechBubble speechBubble;
    private ChatHandler chatHandler;

    void Start()
    {
        //StartCoroutine(LoopRecording());
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("Nam11: No microphone detected.");
            return;
        }

        micName = Microphone.devices[0];
        Debug.Log("Nam11: Using mic: " + micName);
        recording = Microphone.Start(micName, true, 10, SAMPLE_RATE);
        bouncingcBallMgr = FindObjectOfType<BouncingcBallMgr>();

        //recording = Microphone.Start(null, true, 10, SAMPLE_RATE); // 10-second loop buffer

        if (speechBubble == null)
            speechBubble = FindObjectOfType<SpeechBubble>();

        chatHandler = FindObjectOfType<ChatHandler>();

        StartCoroutine(LoopListening());
        
    }

    IEnumerator LoopListening()
    {
        while (true)
        {
            // Wait until user starts speaking
            yield return StartCoroutine(WaitForSpeech());

            // Wait until user finishes speaking
            yield return StartCoroutine(WaitForSilence());

            // Send the audio (you can extract recent audio clip segment)
            yield return StartCoroutine(CaptureAndSend());

            // Small optional pause
            yield return new WaitForSeconds(0.2f);
        }
    }

    float GetMicVolume()
    {
        int micPos = Microphone.GetPosition(null) - 256;
        if (micPos < 0) return 0;

        float[] samples = new float[256];
        recording.GetData(samples, micPos);

        float sum = 0; 
        foreach (var sample in samples)
            sum += sample * sample;

        return Mathf.Sqrt(sum / samples.Length);
    }


    IEnumerator WaitForSpeech()
    {
        Debug.Log("Waiting for user to start speaking...");
        while (GetMicVolume() < 0.05f) // threshold for voice detection
        {
            yield return null;
        }
        Debug.Log("Voice detected");
    }

    IEnumerator WaitForSilence()
    {
        Debug.Log("Waiting for user to stop speaking...");
        float silenceTimer = 0f;

        while (silenceTimer < 1f) // require 1 second of silence
        {
            if (GetMicVolume() < 0.005f)
                silenceTimer += Time.deltaTime;
            else
                silenceTimer = 0f; // reset if voice is heard again

            yield return null;
        }

        Debug.Log("Silence detected");
    }

    IEnumerator LoopRecording()
    {
        while (true)
        {
            yield return StartCoroutine(CaptureAndSend());
            yield return new WaitForSeconds(1); // Small pause before next listen
        }
    }

    // public void StartRecording()
    // {
    //     StartCoroutine(CaptureAndSend());
    //     Debug.Log("Nam11 Microphone devices: " + string.Join(", ", Microphone.devices));

    // }

    IEnumerator CaptureAndSend()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("Nam11 No microphone devices found!");
            yield break;
        }  

        //byte[] audio = WavUtility.FromAudioClip(recording); // Convert clip to WAV byte[]

        int sampleLength = SAMPLE_RATE * 3; // 3 seconds
        int micPos = Microphone.GetPosition(micName); // current mic position in samples
        int startSample = micPos - sampleLength;

        if (startSample < 0)
            startSample += recording.samples; // wrap around if needed

        float[] samples = new float[sampleLength];
        recording.GetData(samples, startSample);

        AudioClip trimmed = AudioClip.Create("trimmed", sampleLength, 1, SAMPLE_RATE, false);
        trimmed.SetData(samples, 0);

        // Convert trimmed clip to WAV
        byte[] audio = WavUtility.FromAudioClip(trimmed);


        UnityWebRequest request = UnityWebRequest.PostWwwForm(witApiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(audio);
        request.SetRequestHeader("Authorization", "Bearer " + witAccessToken);
        request.SetRequestHeader("Content-Type", "audio/wav");
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        Debug.Log("Nam11 Sending audio to Wit...");
        Debug.Log("Nam11 Recording done, audio length: " + recording.length);


        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Nam11 Wit.ai Error: " + request.error);
            yield break;
        }

        string json = request.downloadHandler.text;
        Debug.Log("Nam11 Raw JSON Response:\n" + json);

        string recognizedText = json.ToLower();

        if (json.ToLower().Contains("stop chat") || json.ToLower().Contains("stop talking") ||
            json.ToLower().Contains("end chat") || json.ToLower().Contains("bye"))
        {
            Debug.Log("Nam11 Detected stop chat command.");
            chatHandler?.EndChat();
            dog?.MakeDogWander();
            speechBubble?.ShowMessage("Chat mode ended. You can talk to me again later!");
            yield break;
        }

        if (chatHandler != null && chatHandler.IsChatMode())
        {
            yield return StartCoroutine(chatHandler.SendMessageToChatGPT(recognizedText));
            yield break;
        }

        if (json.ToLower().Contains("let's talk") || json.ToLower().Contains("lets talk") ||
            json.ToLower().Contains("let's chat") || json.ToLower().Contains("lets chat"))
        {
            chatHandler?.StartChat();
            dog.MakeDogComeHere(); 
            dog.MakeDogSit();
            speechBubble?.ShowMessage("Chat mode started. You can now talk to me!");
            yield break;
        }

        


        //behavior commands
        if (json.ToLower().Contains("sit") || json.ToLower().Contains("set down") || json.ToLower().Contains("down"))
        {
            Debug.Log("Nam11 Dog sit command recognized!");
            dog.MakeDogSit();
        }
        else if (json.ToLower().Contains("wag"))
        {
            Debug.Log("Nam11 Wag tail command recognized!");
            dog.MakeDogWagTail();
        }
        else if (json.ToLower().Contains("walk") || json.ToLower().Contains("go forward"))
        {
            Debug.Log("Nam11 Walk command recognized!");
            dog.MakeDogWalkForward();
        }
        else if (json.ToLower().Contains("go back") || json.ToLower().Contains("go backward"))
        {
            Debug.Log("Nam11 Move back command recognized!");
            dog.MakeDogMoveBackward();
        }
        else if (json.ToLower().Contains("go left"))
        {
            Debug.Log("Nam11 Move left command recognized!");
            dog.MakeDogMoveLeft();
        }
        else if (json.ToLower().Contains("go right"))
        {
            Debug.Log("Nam11 Move right command recognized!");
            dog.MakeDogMoveRight();
        }
        else if (json.ToLower().Contains("come here") || json.ToLower().Contains("back here") || json.ToLower().Contains("hey silver") || json.ToLower().Contains("silver"))
        {
            Debug.Log("Nam11 Come here command recognized!");
            dog.MakeDogComeHere(); // Pass the player transform
        }
        else if (json.ToLower().Contains("go there") || json.ToLower().Contains("move there")
                || json.ToLower().Contains("there") || json.ToLower().Contains("over there"))
        {
            Debug.Log("Nam11 Go there command recognized!");
            if (bouncingcBallMgr != null)
            {
                dog.MakeDogGoThere(bouncingcBallMgr.currentLaserTarget); // Call the new method on DogMovement
            }
            else
            {
                Debug.LogError("Nam11 bouncingcBallMgr is null");
            }
        }
        else if (json.ToLower().Contains("stop"))
        {
            Debug.Log("Nam11 Stop command recognized!");
            dog.MakeDogStop();

        }
        else if (json.ToLower().Contains("eat") || json.ToLower().Contains("go eat")
                || json.ToLower().Contains("eating") || json.ToLower().Contains("time to eat"))
        {
            Debug.Log("Nam11 Eat command recognized!");
            dog.MakeDogGoEat(curryPlate);
        }
        else if (json.ToLower().Contains("bad boy") || json.ToLower().Contains("stupid dog")
                || json.ToLower().Contains("bad dog"))
        {
            Debug.Log("Nam11 MakeDogAngry command recognized!");
            dog.MakeDogAngry(); // Pass the player transform
        }
        else if (json.ToLower().Contains("fetch") || json.ToLower().Contains("get ball")
                || json.ToLower().Contains("go get ball") || json.ToLower().Contains("get the ball"))
        {
            Debug.Log("Nam11 Fetch command recognized!");
            dog.MakeDogFetchBall(tennisBall);
        }
        else if (json.ToLower().Contains("move ball") || json.ToLower().Contains("get it again")
                || json.ToLower().Contains("get ball again") || json.ToLower().Contains("get the ball again"))
        {
            Debug.Log("Nam11 Throw ball command recognized!");
            dog.ThrowBallAndFetch(tennisBall, player);
        }
        else if (json.ToLower().Contains("follow me") || json.ToLower().Contains("go with me"))
        {
            Debug.Log("Nam11 Follow me command recognized!");
            dog.MakeDogFollowPlayer();
        }
        else if (json.ToLower().Contains("i feel sad") || json.ToLower().Contains("i'm sad")
                || json.ToLower().Contains("i feel bad") || json.ToLower().Contains("i have a bad day"))
        {
            Debug.Log("Nam11 Comfort me command recognized!");
            dog.MakeDogComfortMe();
        }
        else if (json.ToLower().Contains("go play") || json.ToLower().Contains("go playing"))
        {
            Debug.Log("Nam11 Go playing command recognized!");
            dog.MakeDogWander();
        }
        else if (json.ToLower().Contains("let play math game") || json.ToLower().Contains("let play game") ||
            json.ToLower().Contains("let's play math game") || json.ToLower().Contains("let's play game")
            || json.ToLower().Contains("time to play"))
        {
            dog.MathGameSetup("let play math game");
        }
        else if (json.ToLower().Contains("are you ready"))
        {
            dog.MathGameSetup("are you ready");
        }
        else if (json.ToLower().Contains("incorrect") || json.ToLower().Contains("wrong")
        || json.ToLower().Contains("very close") || json.ToLower().Contains("still wrong")
        || json.ToLower().Contains("it's close") || json.ToLower().Contains("its close")
        || json.ToLower().Contains("it close") || json.ToLower().Contains("oh boy"))
        {
            dog.DogIncorrectMathRespond();
        }
        else if (json.ToLower().Contains("correct") || json.ToLower().Contains("good boy")
                || json.ToLower().Contains("good job") || json.ToLower().Contains("smart"))
        {
            dog.DogCorrectMathRespond();
        }
        else if (json.ToLower().Contains("try again") || json.ToLower().Contains("try it again") ||
                json.ToLower().Contains("one more time") || json.ToLower().Contains("another shot"))
        {
            dog.DogTryPreviousMathProblemAgain();
        }
        else if (json.ToLower().Contains("let take a break") || json.ToLower().Contains("take a break")
                || json.ToLower().Contains("break time"))
        {
            dog.DogEndMathSection();
        }
        else
        {
            string cleanedText = json.ToLower().Trim();
            Debug.Log($"Nam11: Math expression cleanedText = {cleanedText}");
            string expression = ExtractMathExpression(cleanedText);
            if (expression != "")
            {
                int result = Mathf.Clamp(dog.predefinedMathExpressions[expression], 0, 10); // limit barking
                Debug.Log($"Nam11: Found predefined math expression '{expression}' = {result}");
                dog.MakeDogDoMath(expression);
                yield break;
            }
            else
            {
                Debug.Log("Nam11 No recognized command found.");
            }

        }
    }

    private string ExtractMathExpression(string jsonString) 
    {
        string result = "";
        foreach (string key in dog.predefinedMathExpressions.Keys) 
        {
            if (jsonString.Contains(key)) 
            {
                Debug.Log($"Nam11: ExtractMathExpression() = {key}");
                return key;
            }
        } 
        return result;
    } 

    bool IsLoudEnough(AudioClip clip, float threshold = 0.01f)
    {
        float[] samples = new float[clip.samples];
        clip.GetData(samples, 0);
        // foreach (float sample in samples)
        // {
        //     if (Mathf.Abs(sample) > threshold)
        //         return true;
        // }
        // return false;
        float rms = 0f;
        float peak = 0f;
        foreach (float sample in samples)
        {
            float sq = sample * sample;
            rms += sq;
            peak = Mathf.Max(peak, Mathf.Abs(sample));
        }
        rms = Mathf.Sqrt(rms / samples.Length);

        return rms > 0.005f && peak > 0.02f;

    }

}
