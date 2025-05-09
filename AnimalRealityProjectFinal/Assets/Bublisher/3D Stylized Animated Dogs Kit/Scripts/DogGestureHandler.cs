using UnityEngine;

public class DogGestureHandler : MonoBehaviour
{
    public DogMovement dogMovement;
    public Transform player;

    private bool hasTriggered = false;

    public void OnGestureRecognized()
    {
        if (!hasTriggered)
        {
            Vector3 target = player.position;
            target.y = 0f;
            dogMovement.MakeDogComeHere(target);
            Debug.Log("Nam11 Dog is coming to you!");
            hasTriggered = true;
        }
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}
