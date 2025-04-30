using UnityEngine;
using Meta.WitAi;
using Meta.WitAi.Json;
using System.Collections;

public class VoiceController : MonoBehaviour
{
    public PetBehavior dog;
    public VoiceService voiceService;

    void Start()
    {
        Debug.Log("VoiceController Start()");

        if (voiceService != null)
        {
            Debug.Log("VoiceService found.");
            voiceService.VoiceEvents.OnResponse.AddListener(OnWitResponse);
            StartCoroutine(StartListeningDelayed());
        }
        else
        {
            Debug.LogError("VoiceService is NULL");
        }
    }

    IEnumerator StartListeningDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log("VoiceService activating...");
        voiceService.Activate();
    }

    void OnWitResponse(WitResponseNode response)
    {
        string text = response["text"];
        string intent = response["intents"]?[0]?["name"];

        Debug.Log("Recognized: " + text);
        Debug.Log("Detected Intent: " + intent);

        switch (intent)
        {
            case "sit_intent":
                dog.Sit();
                break;

            case "come_intent":
                dog.FollowPlayer();
                break;

            case "stop_intent":
                dog.StopFollowing();
                break;

            case "bark_intent":
                dog.Angry();
                break;

            case "wagtail_intent":
                dog.WagTail();
                break;

            case "spin_intent":
                dog.Spin();
                break;

            default:
                Debug.Log("Unknown intent: " + intent);
                break;
        }
        voiceService.Activate(); // Keep listening
    }
}
