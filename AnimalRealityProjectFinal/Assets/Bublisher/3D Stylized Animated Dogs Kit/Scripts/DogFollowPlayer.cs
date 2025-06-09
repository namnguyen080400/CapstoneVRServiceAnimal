using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DogFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform player; // drag OVRCameraRig.CenterEyeAnchor
    public bool isFollowing = false;
    public DogMovement dog;
    private NavMeshAgent agent;
    //private Animator animator;

    public float rotationSpeed = 5f;
    public float stoppingDistanceFromPlayer = .1f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //animator = GetComponent<Animator>();
        /*         if (player == null)
                {
                    Debug.LogWarning("Nam11 DogFollowPlayer: Player not set, trying to find Main Camera...");
                    GameObject cam = GameObject.FindWithTag("MainCamera");

                    if (cam != null)
                    {
                        player = cam.transform;
                        Debug.Log("DogFollowPlayer: Found MainCamera at runtime.");
                    }
                    else
                    {
                        Debug.LogError("DogFollowPlayer: Could not find MainCamera. Assign player manually.");
                    }
                } */
        if (player == null)
        {
            Debug.Log("Nam11 cannot find camera at run time.");
        }
        isFollowing = false;
        StartCoroutine(DelayedWarpToNavMesh());
    }

    private IEnumerator DelayedWarpToNavMesh()
    {
        yield return new WaitForEndOfFrame();  // Let Unity complete initial NavMesh setup

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
            Debug.Log("Nam11 dog warped to NavMesh at " + hit.position);
        }
        else
        {
            Debug.LogError("Nam11 Dog could not find nearby NavMesh!");
        }
    }

    void Update()
    {
        //Debug.Log($"Nam11 isFollowing = {isFollowing}");
        // Debug.Log("Nam11 Camera localPosition = " + player.localPosition);
        // Debug.Log("Nam11 Camera worldPosition = " + player.position);
        if (agent == null || !isFollowing || player == null || dog.isBusy) 
        {
            return;
        }

        //Vector3 targetPosition = Camera.main.transform.position;
        Vector3 targetPosition = player.transform.position;
        targetPosition.y = transform.position.y;
        float distance = Vector3.Distance(transform.position, targetPosition);

        //Debug.Log($"Nam11 HEADSET POS: {targetPosition}");
        Debug.Log($"Nam11 distance = {distance} transform.position = {transform.position} targetPosition = {targetPosition}");
        
        //Debug.Log("Nam11 XR Origin position: " + GameObject.Find("XR Origin").transform.position);        
        if (agent.isOnNavMesh && distance > stoppingDistanceFromPlayer) 
        {
            Debug.Log($"Nam11 Set SetDestination = {targetPosition}");
            agent.speed = 1.0f;
            agent.SetDestination(targetPosition);
        }
            

        // Rotate to face player
        Vector3 lookDirection = targetPosition - transform.position;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            //Debug.Log("Nam11 dog rotation");
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
    }

}
