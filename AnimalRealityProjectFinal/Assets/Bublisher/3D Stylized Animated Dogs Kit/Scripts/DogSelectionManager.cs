using UnityEngine;
using UnityEngine.UI;

public class DogSelectionManager : MonoBehaviour
{
    [Header("Dog Options")]
    public GameObject[] dogPrefabs; // Assign German Shepherd, Chihuahua, etc.

    [Header("Setup References")]
    public Transform spawnPoint; // Where to spawn the dog
    public GameObject selectionUI; // UI panel containing the buttons
    public WitVoiceCommandHandler voiceHandler; // Hook voice commands

    private GameObject currentDog;

    public void SelectDog(int index)
    {
        if (currentDog != null)
        {
            Destroy(currentDog);
        }

        currentDog = Instantiate(dogPrefabs[index], spawnPoint.position, Quaternion.identity);

        DogMovement movement = currentDog.GetComponent<DogMovement>();
        DogFollowPlayer follow = currentDog.GetComponent<DogFollowPlayer>();

        if (voiceHandler != null) voiceHandler.dog = movement;
        if (follow != null) follow.dog = movement;

        selectionUI.SetActive(false); // Hide the menu after selection
        Debug.Log($"Nam11: Selected dog = {currentDog.name}");
    }

    public void ShowSelectionMenu()
    {
        selectionUI.SetActive(true);
    }
}
