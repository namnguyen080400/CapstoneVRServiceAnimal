using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetFollow : MonoBehaviour
{
    [Header("Player Reference")]
    // Assign Main Camera from XR Origin
    public Transform playerCamera; 

    [Header("Follow Settings")]
    public float followSpeed = 1.5f;
    public float rotationSpeed = 5f;
    public float stopDistance = 1.5f;

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

        Vector3 targetPosition = playerCamera.position;
        targetPosition.y = transform.position.y; // Lock Y position (stay on ground)

        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance > stopDistance)
        {
            // Move toward player
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * followSpeed * Time.deltaTime;

            // Rotate to face the player
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            petBehavior.Play(PetBehavior.PetAnim.Walk);
        }
        else
        {
            // Stop and idle when close enough
            petBehavior.Play(PetBehavior.PetAnim.Breath);
        }
    }
}

