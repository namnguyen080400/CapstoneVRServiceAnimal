using UnityEngine;

public class PetFollow : MonoBehaviour
{
    [Header("Player Reference")]
    public Transform playerCamera; // Assign Main Camera here (e.g., XR Origin > Main Camera)

    [Header("Follow Settings")]
    public float followSpeed = 1f;
    public float rotationSpeed = 5f;
    public float stopDistance = 0.5f;

    [Header("State")]
    [SerializeField] private bool isFollowing = false;

    private PetBehavior petBehavior;

    void Start()
    {
        petBehavior = GetComponent<PetBehavior>();

        if (!playerCamera)
        {
            Debug.LogWarning("PetFollow: Please assign the XR Main Camera to 'playerCamera'.");
        }
    }

    void Update()
    {
        if (!playerCamera || petBehavior == null) return;

        // Prevent movement during locked animations like Sit, Spin, etc.
        if (petBehavior.IsAnimationLocked) return;

        if (isFollowing)
        {
            Vector3 targetPosition = playerCamera.position;
            targetPosition.y = transform.position.y; // Stay grounded

            float distance = Vector3.Distance(transform.position, targetPosition);

            if (distance > stopDistance)
            {
                // Move toward the player
                Vector3 direction = (targetPosition - transform.position).normalized;
                transform.position += direction * followSpeed * Time.deltaTime;

                // Face the player
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

                petBehavior.Run(); // Run animation
            }
            else
            {
                // Close enough, stay still and look at the player
                FacePlayer();
                petBehavior.Breath(); // Idle animation
            }
        }
        //else
        //{
        //    // Optional: even if not following, keep looking at player
        //    FacePlayer();
        //}
    }

    private void FacePlayer()
    {
        Vector3 dir = playerCamera.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotationSpeed);
        }
    }

    // Call this method to make pet start following
    public void FollowPlayer()
    {
        isFollowing = true;
        Debug.Log("Pet starts following player.");
    }

    // Call this method to stop following
    public void StopFollowing()
    {
        isFollowing = false;
        petBehavior.Breath();
        Debug.Log("Pet stops following.");
    }

    public bool IsFollowing => isFollowing;
}
