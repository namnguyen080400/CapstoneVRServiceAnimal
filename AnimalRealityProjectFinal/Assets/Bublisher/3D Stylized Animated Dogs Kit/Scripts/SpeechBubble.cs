using UnityEngine;
using TMPro;
using System.Collections;

public class SpeechBubble : MonoBehaviour
{
    public GameObject bubbleUI;
    public TMP_Text bubbleText;

    [Header("Bark Settings")]
    public AudioSource barkAudioSource;
    public AudioClip[] barkClips;
    void Start()
    {
        bubbleUI.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        bubbleText.text = message;
        bubbleUI.SetActive(true);

        PlayBarkSound();

        StopAllCoroutines();
        StartCoroutine(HideAfterSeconds(10f)); // show for 10 seconds
    }

    private void PlayBarkSound()
    {
        if (barkAudioSource != null && barkClips != null && barkClips.Length > 0)
        {
            var clip = barkClips[Random.Range(0, barkClips.Length)];
            barkAudioSource.PlayOneShot(clip);
        }
    }
    public void ShowPettingMessage(AudioClip comfortClip)
    {
        string[] petLines = {
        "I'm your happy doggo~", "More pets please!", "You pet good!", "You're the best hooman!!!","Can we do this forever?"
    };

        string line = petLines[Random.Range(0, petLines.Length)];
        bubbleText.text = line;
        bubbleUI.SetActive(true);

        if (comfortClip != null && barkAudioSource != null)
        {
            // Stop any currently playing audio before playing the new one
            if (barkAudioSource.isPlaying)
                barkAudioSource.Stop();

            barkAudioSource.clip = comfortClip;
            barkAudioSource.Play();
        }

        StopAllCoroutines();
        StartCoroutine(HideAfterSeconds(10f));
    }

    IEnumerator HideAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        bubbleUI.SetActive(false);
    }
}
