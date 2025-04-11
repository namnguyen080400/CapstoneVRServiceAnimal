using UnityEngine;

public class PetFollow : MonoBehaviour
{
    [Header("Player Reference")]
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

        //animation lock
        if (petBehavior.IsAnimationLocked) return;

        Vector3 targetPosition = playerCamera.position;
        targetPosition.y = transform.position.y;

        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance > stopDistance)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * followSpeed * Time.deltaTime;

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            petBehavior.Run();
        }
        else
        {
            petBehavior.Breath();
        }
    }
}
