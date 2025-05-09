#define DEBUG
using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class DogMovement : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 500f;
    public Vector3 minBounds = new Vector3(-10f, 0f, -10f);
    public Vector3 maxBounds = new Vector3(10f, 0f, 10f);
    public Transform ballHoldPoint;
    public NavMeshAgent navMeshAgent;
    public DogFollowPlayer dogFollowPlayer{ get; set; }
    public AudioSource comfortDogSound;
    public AudioSource angryDogSound;
    public const int WAIT_CYCLE = 40;
    public const float MOVING_TIME = 100f;
    public const float WALKING_SPEED = 5f;
    public const float JOGGING_SPEED = 7f;
    public const float RUNNING_SPEED = 10f;
    public const float WAITING_IN_MS = 0.05f;
    private Animator animator;
    private Coroutine moveCoroutine;
    private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            Debug.LogWarning("Nam11 Animator not found on root, found in child: " + animator);
        }
        audioSource = GetComponent<AudioSource>();
        moveCoroutine = null;
        dogFollowPlayer = GetComponent<DogFollowPlayer>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        // You can add Android-compatible microphone code here later
        Debug.Log("Nam11 Animator = " + animator);
        Debug.Log("Nam11 DogMovement attached to: " + gameObject.name);
    }

    public void Update()
    {
        // Reserved for movement input if needed
    }

    public void MakeDogSit() 
    {
        StartCoroutine(MakeDogSitPrivate());
    }

    public void MakeDogWagTail() 
    {
        StartCoroutine(MakeDogWagTailPrivate());
    }

    public void MakeDogAngry()
    {
        StartCoroutine(MakeDogAngryPrivate());
    }

    public void MakeDogStopTransitionToIdle() 
    {
        StartCoroutine(MakeDogStopTransitionToIdlePrivate());
    }

    public void MakeDogWalk() 
    {
        StartCoroutine(MakeDogWalkPrivate());
    } 

    public void MakeDogComeHere(Vector3 target)
    {
        StartCoroutine(MakeDogComeHerePrivate(target));
    }

    public void MakeDogMoveLeft()
    {
        StartCoroutine(MakeDogMoveLeftPrivate());
    }

    public void MakeDogMoveRight()
    {
        StartCoroutine(MakeDogMoveRightPrivate());
    }

    public void MakeDogMoveBackward()
    {
        StartCoroutine(MakeDogMoveBackwardPrivate());
    }

    public void MakeDogGoThere(Transform destination)
    {
        StartCoroutine(MakeDogGoTherePrivate(destination));
    }
    public void MakeDogGoEat(Transform destination)
    {
        StartCoroutine(MakeDogGoEatPrivate(destination));
    }

    public void MakeDogFetchBall(Transform ball, Transform player)
    {
        StartCoroutine(MakeDogFetchBallPrivate(ball, player));
    }   

    public void MakeDogFollowPlayer() 
    {
        StartCoroutine(MakeDogFollowPlayerPrivate());
    }

    public void MakeDogComfortMe(Vector3 targetPosition)
    {
        StartCoroutine(MakeDogComfortMePrivate(targetPosition));
    }

    //TODO
    // public void MakeDogHappy(Vector3 targetPosition)
    // {
    //     StartCoroutine(MakeDogHappyPrivate(targetPosition));
    // }


    private void StopMovement()
    {
        Debug.Log("Nam11 StopMovement() start " + moveCoroutine);
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
            //dogFollowPlayer.isFollowing = true;
        }
        dogFollowPlayer.isFollowing = false;
        // Stop NavMeshAgent movement
        if (navMeshAgent != null && navMeshAgent.enabled)
        {
            navMeshAgent.ResetPath();
            navMeshAgent.velocity = Vector3.zero;
        }
        Debug.Log("Nam11 StopMovement() end " + moveCoroutine);
    }

    private IEnumerator MoveToTarget(Vector3 target, string actionAtTarget, float movingSpeed)
    {
        float stoppingDistance = 0.5f; // how close is "close enough"
        transform.LookAt(target);
        Debug.Log("Nam11 MoveToTarget start " + target + "action at target = " + actionAtTarget);
        Debug.Log("Nam11 MoveToTarget transform position " + transform.position + "distance to target = " + Vector3.Distance(transform.position, target));
        Debug.Log("Nam11 MoveToTarget dogFollowPlayer.isFollowing = " + dogFollowPlayer.isFollowing);
        bool success = false;
        try
        {
            while (Vector3.Distance(transform.position, target) > stoppingDistance)
            {
                Vector3 direction = (target - transform.position).normalized;
                transform.Translate(direction * movingSpeed * Time.deltaTime, Space.World);

                if (direction != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
                }
                Debug.Log("Nam11 Move to target loop. Distance to target " + Vector3.Distance(transform.position, target));
                yield return null;
            }
            success = true;
            Debug.Log("Nam11 MoveToTarget end loop " + target + " actionAtTarget " + actionAtTarget + "dogFollowPlayer.isFollowing = " + dogFollowPlayer.isFollowing);
        }
        finally
        {
            if (!success)
            {
                Debug.LogError("Nam11 MoveToTarget Exception: exited unexpectedly!");
            }
        }

        yield return TransitionToIdle();
        if (actionAtTarget != null) 
        {
            Debug.Log("Nam11 MoveToTarget transition to next state " + " actionAtTarget " + actionAtTarget);
            yield return StartCoroutine(TriggerAndWaitForTransitionToTarget(actionAtTarget));
        }
    }

    private IEnumerator TransitionToIdle()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log("Nam11 TransitionToIdle() start.");
        if (stateInfo.IsName("Idle")) 
        {
            Debug.Log("Nam11 Current animation: " + stateInfo.shortNameHash);
            Debug.Log("Nam11 Dog is already in Idle State. No need to transition.");
        } 
        else if (stateInfo.IsName("Running")) 
        {
            Debug.Log("Nam11 Dog transition from running to idle");
            yield return StartCoroutine(WaitAndMoveToIdle("RunningToIdle"));
            // animator.SetTrigger("RunningToIdle");
        }
        else if (stateInfo.IsName("WalkingNormal")) 
        {
            Debug.Log("Nam11 Dog transition from walking to idle");
            Debug.Log("Nam11 Current animation: " + stateInfo.shortNameHash);
            yield return StartCoroutine(WaitAndMoveToIdle("WalkingNormalToIdle"));

            //WaitAndMoveToIdle("WalkingNormalToIdle");
            // animator.SetTrigger("WalkingNormalToIdle");
        }
        else if (stateInfo.IsName("SittingCycle")) 
        {
            Debug.Log("Nam11 Dog transition from SittingCycle to idle");            
            Debug.Log("Nam11 Current animation: " + stateInfo.shortNameHash);
            yield return StartCoroutine(WaitAndMoveToIdle("SittingCycleToIdle"));
            // animator.SetTrigger("SittingCycleToIdle");
        } 
        else if (stateInfo.IsName("Breathing")) 
        {
            Debug.Log("Nam11 Dog is in Breathing cycle to idle.");
            yield return StartCoroutine(WaitAndMoveToIdle("BreathingToIdle"));
            // animator.SetTrigger("BreathingToIdle");
        }
        else if (stateInfo.IsName("AngryCycle")) 
        {
            Debug.Log("Nam11 Dog is in AngryCycle to idle.");
            yield return StartCoroutine(WaitAndMoveToIdle("AngryCycleToIdle"));
            // animator.SetTrigger("AngryCycleToIdle");
        }
        else if (stateInfo.IsName("WigglingTail")) 
        {
            Debug.Log("Nam11 Dog transition from WigglingTail to idle");
            Debug.Log("Nam11 Current animation: " + stateInfo.shortNameHash);
            yield return StartCoroutine(WaitAndMoveToIdle("WigglingTailToIdle"));
            // animator.SetTrigger("WigglingTailToIdle");
        } 
        else if (stateInfo.IsName("EatingCycle") || stateInfo.IsName("EatingStart")) 
        {
            Debug.Log("Nam11 Dog is in EatingCycle or EatingStart to idle.");
            yield return StartCoroutine(WaitAndMoveToIdle("EatingCycleToIdle"));
            // animator.SetTrigger("EatingCycleToIdle");
        } 
        else if (stateInfo.IsName("SittingStart")) 
        {
            Debug.Log("Nam11 Dog is in SittingStart state.");
        }
        else if (stateInfo.IsName("AngryStart")) 
        {
            Debug.Log("Nam11 Dog is in AngryStart state.");
        }  
        else if (stateInfo.IsName("EatingStart")) 
        {
            Debug.Log("Nam11 Dog is in EatingStart state.");
        }        
        else 
        {           
            Debug.Log("Nam11 Unknown state. No transition to idle");
        }
    }

    private IEnumerator WaitAndMoveToIdle(string triggerString) 
    {
        int counter = 0;
        bool transitioned = false;
        //animator.ResetTrigger(triggerString);
        animator.SetTrigger(triggerString);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        while (counter < WAIT_CYCLE) 
        {
            if (!stateInfo.IsName("Idle")) 
            {

                yield return new WaitForSeconds(WAITING_IN_MS);
            }
            
            stateInfo = animator.GetCurrentAnimatorStateInfo(0); 
            if (stateInfo.IsName("Idle")) // Any state other than Idle is okay
            {
                transitioned = true;
                break;
            }
            counter++;   
        }
        if (transitioned) 
        {
            Debug.Log("Nam11 Successfully transition to Idle counter = " + counter);
        }
        else 
        {
            Debug.Log("Nam11 Fail transition to Idle. Current State " + stateInfo.shortNameHash + " counter = " + counter);
        }
        
    }

    private IEnumerator TriggerAndWaitForTransitionToTarget(string triggerString) 
    {
        int counter = 0;
        bool transitioned = false;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log("Nam11 Start TriggerAndWaitForTransitionToTarget() start current state " + stateInfo.shortNameHash + " " + triggerString);
        //animator.ResetTrigger(triggerString); // Prevent accidental double triggering
        animator.SetTrigger(triggerString);
        while (counter < WAIT_CYCLE) 
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (!stateInfo.IsName("Idle")) // Any state other than Idle is okay
            {
                transitioned = true;
                break;
            }
            yield return new WaitForSeconds(WAITING_IN_MS);
            counter++;     
        }

        if (!transitioned) 
        {
            Debug.Log("Nam11 TriggerAndWaitForTransitionToTarget() End Fail to transition out of Idle state counter = " + counter);
        }
        else 
        {
            Debug.Log("Nam11 TriggerAndWaitForTransitionToTarget() End Successfully transition to " + stateInfo.shortNameHash + " counter = " + counter);
        }
    }

    private void MoveInDirection(Vector3 direction, float currentSpeed)
    {
        Debug.Log("Nam11 MoveInDirection() called with direction: " + direction);
        moveCoroutine = StartCoroutine(MoveOverTime(direction, MOVING_TIME, currentSpeed));
    }

    private IEnumerator MoveOverTime(Vector3 direction, float duration, float currentSpeed)
    {
        float timeElapsed = 0f;
        Debug.Log("Nam11 MoveOverTime() start direction = " + direction);
        while (timeElapsed < duration)
        {
            transform.Translate(direction * currentSpeed * Time.deltaTime, Space.World);
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Nam11 MoveOverTime() end");           
        yield return StartCoroutine(TransitionToIdle());
    }

    private IEnumerator MakeDogStopTransitionToIdlePrivate() {
        if (moveCoroutine != null) 
        {
            StopMovement();
        }
        dogFollowPlayer.isFollowing = false;
        yield return StartCoroutine(TransitionToIdle());
    }

    private IEnumerator MakeDogSitPrivate() 
    {
        Debug.Log("Nam11 MakeDogSitPrivate() start animator = " + animator);
        
        if (moveCoroutine != null) // if dog is moving
        {
            StopMovement(); // stop the moving routine
        }
        dogFollowPlayer.isFollowing = false;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("SittingCycle")) // check if the dog is not sitting
        {
            yield return StartCoroutine(TransitionToIdle()); // transition to idle  
            // move the dog to sitting state       
            yield return StartCoroutine(TriggerAndWaitForTransitionToTarget("SittingStart")); 
            Debug.Log("Nam11 Dog is sucessfully transtion to sitting.");
        }
        else // dog is alaready sitting. leave it alone
        {
            Debug.Log("Nam11 Dog is already in sitting state.");
        }  
        Debug.Log("Nam11 MakeDogSitPrivate() end isFollowing " + dogFollowPlayer.isFollowing);
    }

    private IEnumerator MakeDogWalkPrivate() 
    {
        Debug.Log("Nam11 MakeDogWalkPrivate() start");

        dogFollowPlayer.isFollowing = false;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        string actionAtTarget = "WalkingNormal";
        if (!stateInfo.IsName("WalkingNormal")) 
        {
            if (moveCoroutine != null) 
            {
                StopMovement();
            }
            yield return StartCoroutine(TransitionToIdle());
            yield return StartCoroutine(TriggerAndWaitForTransitionToTarget(actionAtTarget));
            Debug.Log("Nam11 Dog is sucessfully transtion to WalkingNormal.");
        }
        else 
        {
            Debug.Log("Nam11 Dog is walking.");
        }
        
        MoveInDirection(Vector3.forward, WALKING_SPEED); 
        yield return new WaitForSeconds(2f);

        Debug.Log("Nam11 MakeDogWalkPrivate() end isFollowing " + dogFollowPlayer.isFollowing);
    }

    private IEnumerator MakeDogWagTailPrivate() 
    {
        Debug.Log("Nam11 MakeDogWagTailPrivate() start");       
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        string actionAtTarget = "WigglingTail";
        if (moveCoroutine != null) 
        {
            StopMovement();
        }
        if (!stateInfo.IsName("WigglingTail")) 
        {
            yield return StartCoroutine(TransitionToIdle());
            yield return StartCoroutine(TriggerAndWaitForTransitionToTarget(actionAtTarget));
        }
        else 
        {
            Debug.Log("Nam11 Dog is wagging tail.");
        }        

        Debug.Log("Nam11 MakeDogWagTailPrivate() end isFollowing " + dogFollowPlayer.isFollowing);
    }

    private IEnumerator MakeDogAngryPrivate() 
    {
        Debug.Log("Nam11 MakeDogAngryPrivate() start");
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        string actionAtTarget = "AngryStart";
        if (moveCoroutine != null) 
        {
            StopMovement();
        }
        if (!stateInfo.IsName("AngryCycle")) 
        {
            yield return StartCoroutine(TransitionToIdle());
            yield return StartCoroutine(TriggerAndWaitForTransitionToTarget(actionAtTarget));
            if (angryDogSound != null && angryDogSound.clip != null)
            {
                Debug.Log("Nam11: Playing angry sound.");
                angryDogSound.Play();
            }
            else
            {
                Debug.LogWarning("Nam11: angryDogSound or its clip is missing.");
            }
        }
        else 
        {
            Debug.Log("Nam11 Dog is already in angry state.");
        }        

        Debug.Log("Nam11 MakeDogAngryPrivate() end isFollowing " + dogFollowPlayer.isFollowing);
    }

    private IEnumerator MakeDogComeHerePrivate(Vector3 target) 
    {
        Debug.Log("Nam11 MakeDogComeHerePrivate() start");
        string actionAtTarget = "WigglingTail";
        if (moveCoroutine != null) 
        {
            StopMovement();
        }
        dogFollowPlayer.isFollowing = false;
        yield return StartCoroutine(TransitionToIdle());
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget("Running"));
        //moveCoroutine = StartCoroutine(MoveToTarget(target, actionAtTarget, RUNNING_SPEED));
        navMeshAgent.enabled = true;
        //navMeshAgent.stoppingDistance = 0.05f;
        navMeshAgent.SetDestination(target);
        while (!navMeshAgent.pathPending && navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            yield return null;
        }
        yield return StartCoroutine(TransitionToIdle());
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget(actionAtTarget));           
        Debug.Log("Nam11 MakeDogComeHerePrivate() end isFollowing " + dogFollowPlayer.isFollowing);
    }

    private IEnumerator MakeDogMoveLeftPrivate() 
    {
        Debug.Log("Nam11 MakeDogMoveLeftPrivate() start");
        string actionAtTarget = "WalkingNormal";
        if (moveCoroutine != null) 
        {
            StopMovement();
        }
        dogFollowPlayer.isFollowing = false;
        yield return StartCoroutine(TransitionToIdle());
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget(actionAtTarget));
        MoveInDirection(Vector3.left, WALKING_SPEED); // Global left

        Debug.Log("Nam11 MakeDogMoveLeftPrivate() end isFollowing " + dogFollowPlayer.isFollowing);
    }

    private IEnumerator MakeDogMoveRightPrivate() 
    {
        Debug.Log("Nam11 MakeDogMoveRightPrivate() start");
        string actionAtTarget = "WalkingNormal";
        if (moveCoroutine != null) 
        {
            StopMovement();
        }
        dogFollowPlayer.isFollowing = false;
        yield return StartCoroutine(TransitionToIdle());
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget(actionAtTarget));
        MoveInDirection(Vector3.right, WALKING_SPEED); // Global left

        Debug.Log("Nam11 MakeDogMoveRightPrivate() end isFollowing " + dogFollowPlayer.isFollowing);
    }

    private IEnumerator MakeDogMoveBackwardPrivate() 
    {
        Debug.Log("Nam11 MakeDogMoveBackwardPrivate() start");
        string actionAtTarget = "WalkingNormal";
        if (moveCoroutine != null) 
        {
            StopMovement();
        }
        dogFollowPlayer.isFollowing = false;
        yield return StartCoroutine(TransitionToIdle());
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget(actionAtTarget));
        MoveInDirection(Vector3.back, WALKING_SPEED); // Global left

        Debug.Log("Nam11 MakeDogMoveBackwardPrivate() end isFollowing " + dogFollowPlayer.isFollowing);
    }

    private IEnumerator MakeDogGoTherePrivate(Transform destination) 
    {
        Debug.Log("Nam11 MakeDogGoTherePrivate() start");
        string actionAtTarget = "WigglingTail";
        if (moveCoroutine != null) 
        {
            StopMovement();
        }
        dogFollowPlayer.isFollowing = false;
        yield return StartCoroutine(TransitionToIdle());
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget("WalkingNormal"));
        moveCoroutine = StartCoroutine(MoveToTarget(destination.position, actionAtTarget, RUNNING_SPEED));

        Debug.Log("Nam11 MakeDogGoTherePrivate() end isFollowing " + dogFollowPlayer.isFollowing);
    }

    private IEnumerator MakeDogGoEatPrivate(Transform destination) 
    {
        Debug.Log("Nam11 MakeDogGoEatPrivate() start");
        string actionAtTarget = "EatingStart";
        if (TryGetComponent<NavMeshAgent>(out var agent))
        {
            agent.enabled = false;
        }

        if (moveCoroutine != null) 
        {
            StopMovement();
        }
        dogFollowPlayer.isFollowing = false;
        yield return StartCoroutine(TransitionToIdle());
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget("WalkingNormal"));
        moveCoroutine = StartCoroutine(MoveToTarget(destination.position, actionAtTarget, JOGGING_SPEED));
        if (agent != null) agent.enabled = true;

        Debug.Log("Nam11 MakeDogGoEatPrivate() end. isFollowing " + dogFollowPlayer.isFollowing);
    }

    private IEnumerator MakeDogFetchBallPrivate(Transform ball, Transform player) 
    {
        Debug.Log("Nam11 MakeDogFetchBallPrivate() start");
        if (moveCoroutine != null)
        {
            StopMovement();
        }
        
        if (ball == null)
        {
            Debug.LogError("Nam11 ERROR1: Ball is null in MakeDogFetchBallPrivate!");
            yield break;
        }
        dogFollowPlayer.isFollowing = false;
        // Step 1: Transition to idle
        yield return StartCoroutine(TransitionToIdle());

        // Step 2: Run to the ball          
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget("Running"));
        yield return StartCoroutine(MoveToTarget(ball.position, null, RUNNING_SPEED));

        // Step 3: Pretend to fetch
        yield return StartCoroutine(TransitionToIdle());
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget("EatingStart"));
        //yield return new WaitForSeconds(1.0f); // Simulate grabbing delay
        if (ball == null)
        {
            Debug.LogError("Nam11 ERROR2: Ball is null in MakeDogFetchBallPrivate!");
            yield break;
        }
        // Step 4: Attach the ball to the dog's mouth
        ball.SetParent(ballHoldPoint);
        ball.localPosition = Vector3.zero;
        ball.localRotation = Quaternion.identity;

        // Step 5: Return to player
        yield return StartCoroutine(TransitionToIdle());
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget("Running"));
        Vector3 playerPosition = player.position;
        playerPosition.y = 0f;
        yield return StartCoroutine(MoveToTarget(playerPosition, null, RUNNING_SPEED));

        yield return StartCoroutine(TransitionToIdle());
        // Step 6: Drop the ball in front of the player
        ball.SetParent(null);
        Vector3 dropPosition = transform.position + transform.forward * 0.5f;
        dropPosition.y = 0f;
        ball.position = dropPosition;
        //dogFollowPlayer.isFollowing = true;
        Debug.Log("Nam11 MakeDogFetchBallPrivate() end ball position " + ball.position);
    }

    private IEnumerator MakeDogFollowPlayerPrivate() 
    {
        Debug.Log("Nam11 MakeDogFollowPlayerPrivate() start");
        if (moveCoroutine != null)
        {
            StopMovement();
        }
        // Step 1: Transition to idle
        yield return StartCoroutine(TransitionToIdle());
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget("WalkingNormal"));
        dogFollowPlayer.isFollowing = true;
        
    }

    public void ThrowBallAndFetch(Transform ball, Transform player)
    {
        StartCoroutine(ThrowBallAndFetchPrivate(ball, player));
    }

    private IEnumerator ThrowBallAndFetchPrivate(Transform ball, Transform player)
    {
        Debug.Log("Nam11 ThrowBallAndFetchPrivate() start");
        dogFollowPlayer.isFollowing = false;
        if (moveCoroutine != null) 
        {
            StopMovement();
        }
        
        yield return StartCoroutine(TransitionToIdle());

        // 1. Simulate ball throw: pick a random spot in front of player
        Vector3 throwDirection = player.forward + player.right * Random.Range(-0.5f, 0.5f);
        Vector3 throwTarget = player.position + throwDirection.normalized * Random.Range(3f, 6f);
        throwTarget.y = 0f;

        Debug.Log("Nam11 ThrowBall target = " + throwTarget);
        ball.SetParent(null);
        ball.position = throwTarget;

        yield return new WaitForSeconds(0.5f); // small pause to simulate throw time

        // 2. Command dog to fetch again
        yield return StartCoroutine(MakeDogFetchBallPrivate(ball, player));
        moveCoroutine = null;
        Debug.Log("Nam11 ThrowBallAndFetchPrivate() end");
    }
    private bool ShouldStartFollowingAfterAction(string action)
    {
        return action != "EatingStart";
    }

    private IEnumerator MakeDogComfortMePrivate(Vector3 target)
    {
        Debug.Log("Nam11 MakeDogComfortMePrivate() start");
        
        if (moveCoroutine != null)
        {
            StopMovement();
        }

        dogFollowPlayer.isFollowing = false;

        yield return StartCoroutine(TransitionToIdle());

        // Walk or run to the player
        string approachAnimation = "WalkingNormal";
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget(approachAnimation));
        moveCoroutine = StartCoroutine(MoveToTarget(target, "WigglingTail", WALKING_SPEED)); // or use RUNNING_SPEED if you want it faster
        if (comfortDogSound != null && comfortDogSound.clip != null)
        {
            Debug.Log("Nam11: Playing comfort sound.");
            comfortDogSound.Play();
        }
        else
        {
            Debug.LogWarning("Nam11: comfortDogSound or its clip is missing.");
        }
        yield return new WaitForSeconds(audioSource.clip.length);
        moveCoroutine = null;
        Debug.Log("Nam11 MakeDogComfortMePrivate() end");
    }

    //TODO
    // private IEnumerable MakeDogHappyPrivate(Vector3 target) 
    // {
        
    // }
}
