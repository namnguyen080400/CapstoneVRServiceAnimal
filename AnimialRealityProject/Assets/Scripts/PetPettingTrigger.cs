using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class PetPettingTrigger : MonoBehaviour
{
    private PetBehavior petBehavior;
    private AudioSource audioSource;

    [Header("Touch Sound")]
    public AudioClip whineSound;

    private void Start()
    {
        petBehavior = GetComponent<PetBehavior>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            Debug.Log("Dog was petted!");
            petBehavior.WagTail();

            if (whineSound != null)
            {
                audioSource.PlayOneShot(whineSound);
            }
        }
    }
}
