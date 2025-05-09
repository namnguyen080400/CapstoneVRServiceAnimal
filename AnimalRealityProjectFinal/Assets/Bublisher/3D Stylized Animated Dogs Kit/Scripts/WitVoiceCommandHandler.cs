using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

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

        //recording = Microphone.Start(null, true, 10, SAMPLE_RATE); // 10-second loop buffer
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
        while (GetMicVolume() < 0.01f) // threshold for voice detection
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
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("Nam11 Raw JSON Response:\n" + json);

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
                dog.MakeDogWalk();
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
            else if (json.ToLower().Contains("come here") || json.ToLower().Contains("back here")) 
            {
                Debug.Log("Nam11 Come here command recognized!");
                Vector3 playerPosition = player.position;
                Vector3 direction = (playerPosition - transform.position).normalized;
                //playerPosition -= direction * stoppingDistanceFromPlayer; // Back off a little
                playerPosition.y = 0f;
                dog.navMeshAgent.stoppingDistance = stoppingDistanceFromPlayer;
                dog.MakeDogComeHere(playerPosition); // Pass the player transform
            }       
            else if (json.ToLower().Contains("go there") || json.ToLower().Contains("move there") 
                    || json.ToLower().Contains("there") || json.ToLower().Contains("over there"))
            {
                Debug.Log("Nam11 Go there command recognized!");
                dog.MakeDogGoThere(tennisBall); // Call the new method on DogMovement
            }
            else if (json.ToLower().Contains("stop"))
            {
                Debug.Log("Nam11 Stop command recognized!");
                dog.MakeDogStopTransitionToIdle();
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
                Vector3 playerPosition = player.position;
                playerPosition.y = 0f;
                dog.MakeDogAngry(); // Pass the player transform
            }
            else if (json.ToLower().Contains("fetch") || json.ToLower().Contains("get ball")
                    || json.ToLower().Contains("go get ball") || json.ToLower().Contains("get the ball"))
            {
                Debug.Log("Nam11 Fetch command recognized!");
                dog.MakeDogFetchBall(tennisBall, player);
            }
            else if (json.ToLower().Contains("throw") || json.ToLower().Contains("throw ball")
                    || json.ToLower().Contains("move ball") || json.ToLower().Contains("get it again")
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
                Vector3 playerPosition = player.position;
                playerPosition.y = 0;
                dog.MakeDogComfortMe(playerPosition);
            }

            else 
            {
                Debug.Log("Nam11 No recognized command found.");
            }            
        }
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
