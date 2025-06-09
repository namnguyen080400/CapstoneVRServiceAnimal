using System.Collections;
using UnityEngine;

public class DogTouchTrigger : MonoBehaviour
{
    private DogMovement dogMovement;

    private float triggerCooldown = 0f;
    private float cooldownDuration = 2f;

    private void Start()
    {
        dogMovement = GetComponent<DogMovement>();
        if (dogMovement == null)
        {
            Debug.LogError("DogTouchTrigger error: DogMovement not found.");
        }
    }

    private void Update()
    {
        triggerCooldown += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerHand")) return;
        if (triggerCooldown < cooldownDuration) return;

        if (dogMovement != null)
        {
            Debug.Log("Dog triggered by touch.");

            dogMovement.MakeDogWagTail();
            TriggerHapticsByName(other.name);
            ShowSpeechBubbleFromScript();

            triggerCooldown = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            Debug.Log("Player hand exited trigger.");
            dogMovement.isBusy = false;
        }
    }

    private void TriggerHapticsByName(string objectName)
    {
        if (objectName.Contains("Right"))
        {
            OVRInput.SetControllerVibration(1f, 1f, OVRInput.Controller.RTouch);
            StartCoroutine(StopVibrationAfterDelay(OVRInput.Controller.RTouch));
        }
        else if (objectName.Contains("Left"))
        {
            OVRInput.SetControllerVibration(1f, 1f, OVRInput.Controller.LTouch);
            StartCoroutine(StopVibrationAfterDelay(OVRInput.Controller.LTouch));
        }
        else
        {
            Debug.LogWarning("Controller name not recognized for haptic feedback: " + objectName);
        }
    }

    private IEnumerator StopVibrationAfterDelay(OVRInput.Controller controller)
    {
        yield return new WaitForSeconds(0.2f);
        OVRInput.SetControllerVibration(0, 0, controller);
    }

    private void ShowSpeechBubbleFromScript()
    {
        var speech = FindObjectOfType<SpeechBubble>();
        if (speech != null && dogMovement != null)
        {
            speech.ShowPettingMessage(dogMovement.comfortDogSound.clip);
        }
    }
}
