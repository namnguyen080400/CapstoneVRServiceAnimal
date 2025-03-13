using UnityEngine;

public class DogMovement : MonoBehaviour
{
    public float speed = 2f; // Movement speed
    
    public float turnSpeed = 2f; // Turning speed for smooth rotation
    public float changeDirectionTime = 3f; // Time to change direction
    public Vector3 minBounds = new Vector3(-10f, 0f, -10f); // Minimum movement boundary
    public Vector3 maxBounds = new Vector3(10f, 0f, 10f); // Maximum movement boundary
    public float sittingDuration = 10f; // Sitting duration in seconds

    private float timer; // Timer for direction change
    private float sittingTimer; // Timer for sitting
    private Vector3 moveDirection; // Current movement direction
    private Animator animator; // Reference to Animator for animations
    private bool isSitting = true; // Starts in the sitting state
    private bool isAngry = false; // Indicates whether the dog is angry

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            // Start with sitting animation
            animator.SetTrigger("germanshepherd_SittingStart");
            animator.SetBool("isWalking", false); // Ensure walking is disabled
        }

        sittingTimer = sittingDuration; // Initialize sitting timer
        ChangeDirection(); // Initialize the first direction (won't matter until walking begins)
    }

    void Update()
    {
        if (isSitting)
        {
            // Countdown sitting timer
            sittingTimer -= Time.deltaTime;
            if (sittingTimer <= 0)
            {
                MakeDogWalk(); // Transition to walking after sitting duration
            }
            return; // Prevent movement while sitting
        }

        if (isAngry)
        {
            // Prevent movement while angry
            return;
        }

        // Move the dog forward in its current direction
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        // Check boundaries and change direction if needed
        if (transform.position.x <= minBounds.x || transform.position.x >= maxBounds.x ||
            transform.position.z <= minBounds.z || transform.position.z >= maxBounds.z)
        {
            ChangeDirectionOnBoundary(); // Change direction immediately
        }

        // Gradually rotate toward the movement direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }

        // Countdown to change direction at regular intervals
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ChangeDirection();
        }
    }

    void ChangeDirection()
    {
        // Reset the timer
        timer = changeDirectionTime;

        // Generate a random direction for the dog to move
        float randomAngle = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0, randomAngle, 0);

        // Set movement direction based on the forward direction of the dog
        moveDirection = transform.forward;
    }

    void ChangeDirectionOnBoundary()
    {
        // Pick a new random direction away from the boundary
        if (transform.position.x <= minBounds.x) moveDirection = Vector3.right; // Move right
        else if (transform.position.x >= maxBounds.x) moveDirection = Vector3.left; // Move left

        if (transform.position.z <= minBounds.z) moveDirection = Vector3.forward; // Move forward
        else if (transform.position.z >= maxBounds.z) moveDirection = Vector3.back; // Move backward

        // Update rotation to face the new direction
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = targetRotation;
    }

    public void MakeDogSit()
    {
        if (animator != null && !isSitting && !isAngry)
        {
            animator.SetTrigger("germanshepherd_SittingStart"); // Trigger sitting animation
            animator.SetBool("isWalking", false); // Stop walking animation
            isSitting = true; // Mark the dog as sitting
            sittingTimer = sittingDuration; // Reset the sitting timer
        }
    }

    public void MakeDogWalk()
    {
        if (animator != null && isSitting)
        {
            animator.SetBool("isWalking", true); // Start walking animation
            isSitting = false; // Mark the dog as walking
        }
    }

    public void MakeDogAngry()
    {
        if (animator != null && !isAngry)
        {
            animator.SetTrigger("germanshepherd_AngryCycle"); // Trigger angry animation
            animator.SetBool("isWalking", false); // Stop walking animation
            isAngry = true; // Mark the dog as angry
        }
    }

    public void CalmDog()
    {
        if (animator != null && isAngry)
        {
            animator.SetBool("isWalking", true); // Resume walking animation
            isAngry = false; // Mark the dog as calm
        }
    }
}
