using UnityEngine;
// remove script later
public class DogGestureHandler : MonoBehaviour
{
    public DogMovement dogMovement;
    public Transform player;

    private bool hasTriggered = false;

    public void OnGestureRecognized()
    {
        if (!hasTriggered)
        {
            dogMovement.MakeDogComeHere();
            Debug.Log("Nam11 Dog is coming to you!");
            hasTriggered = true;
        }
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}
