#define DEBUG
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
<<<<<<< Updated upstream

public class DogMovement : MonoBehaviour
{
    public float speed = 5f;
=======
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Mathematics;

public class DogMovement : MonoBehaviour
{
>>>>>>> Stashed changes
    public float rotationSpeed = 500f;
    public Vector3 minBounds = new Vector3(-10f, 0f, -10f);
    public Vector3 maxBounds = new Vector3(10f, 0f, 10f);
    public Transform ballHoldPoint;
    public NavMeshAgent navMeshAgent;
    public DogFollowPlayer dogFollowPlayer{ get; set; }
    public AudioSource comfortDogSound;
    public AudioSource angryDogSound;
<<<<<<< Updated upstream
    public const int WAIT_CYCLE = 40;
    public const float MOVING_TIME = 100f;
    public const float WALKING_SPEED = 5f;
    public const float JOGGING_SPEED = 7f;
    public const float RUNNING_SPEED = 10f;
    public const float WAITING_IN_MS = 0.05f;
    private Animator animator;
    private Coroutine moveCoroutine;
    private AudioSource audioSource;
=======
    public AudioSource singleBarkSoundMath;
    public AudioSource incorrectMathSorrySound;
    public bool isBusy = false;
    public bool isPlayingMath = false;
    public const int WAIT_CYCLE = 40;
    public const float MOVING_TIME = 100f;
    public const float WANDERING_SPEED = 1.0f;
    public const float WALKING_SPEED = 1f;
    public const float JOGGING_SPEED = 1f;
    public const float RUNNING_SPEED = 1f;
    public const float WAITING_IN_MS = 0.05f;
    public const float DISTANCE_TO_TARGET = 1f;
    public const int LAST_DOG_ANSWER  = 0;
    private Animator animator;
    private Coroutine moveCoroutine;
    private AudioSource audioSource;
    private Coroutine randomRoamCoroutine;
    private List<int> dogAnswers;
    private string lastMathQuestion = "";
    public Dictionary<string, int> predefinedMathExpressions = new Dictionary<string, int>()
    {
        { "one plus one", 2 }, { "one plus two", 3 }, { "one plus three", 1 },
        { "one plus four", 1 }, { "one plus five", 2 }, { "one plus six", 2 },
        { "one plus seven", 3 }, { "one plus eight", 1 }, { "one plus nine", 1 },

        { "two plus one", 3 }, { "two plus two", 1 }, { "two plus three", 7 },
        { "two plus four", 5 }, { "two plus five", 3 }, { "two plus six", 4 },
        { "two plus seven", 5 }, { "two plus eight", 10 },

        { "three plus one", 4 }, { "three plus two", 7 }, { "three plus three", 4 },
        { "three plus four", 7 }, { "three plus five", 8 }, { "three plus six", 9 },
        { "three plus seven", 10 },

        { "four plus one", 5 }, { "four plus two", 6 }, { "four plus three", 7 },
        { "four plus four", 8 }, { "four plus five", 9 }, { "four plus six", 10 },

        { "five plus one", 6 }, { "five plus two", 7 }, { "five plus three", 8 },
        { "five plus four", 9 }, { "five plus five", 10 },

        { "six plus one", 7 }, { "six plus two", 8 }, { "six plus three", 9 },
        { "six plus four", 10 },

        { "seven plus one", 8 }, { "seven plus two", 9 }, { "seven plus three", 10 },

        { "eight plus one", 9 }, { "eight plus two", 10 },

        { "nine plus one", 10 }
    };

    private Dictionary<string, int> correctAnswer = new Dictionary<string, int>()
    {
        { "one plus one", 2 }, { "one plus two", 3 }, { "one plus three", 4 },
        { "one plus four", 5 }, { "one plus five", 6 }, { "one plus six", 7 },
        { "one plus seven", 8 }, { "one plus eight", 9 }, { "one plus nine", 10 },

        { "two plus one", 3 }, { "two plus two", 4 }, { "two plus three", 5 },
        { "two plus four", 6 }, { "two plus five", 7 }, { "two plus six", 8 },
        { "two plus seven", 9 }, { "two plus eight", 10 },

        { "three plus one", 4 }, { "three plus two", 5 }, { "three plus three", 6 },
        { "three plus four", 7 }, { "three plus five", 8 }, { "three plus six", 9 },
        { "three plus seven", 10 },

        { "four plus one", 5 }, { "four plus two", 6 }, { "four plus three", 7 },
        { "four plus four", 8 }, { "four plus five", 9 }, { "four plus six", 10 },

        { "five plus one", 6 }, { "five plus two", 7 }, { "five plus three", 8 },
        { "five plus four", 9 }, { "five plus five", 10 },

        { "six plus one", 7 }, { "six plus two", 8 }, { "six plus three", 9 },
        { "six plus four", 10 },

        { "seven plus one", 8 }, { "seven plus two", 9 }, { "seven plus three", 10 },

        { "eight plus one", 9 }, { "eight plus two", 10 },

        { "nine plus one", 10 }
    };
>>>>>>> Stashed changes

    void Start()
    {
        animator = GetComponent<Animator>();
<<<<<<< Updated upstream
=======
        dogAnswers = new List<int>();
        lastMathQuestion = null;
>>>>>>> Stashed changes
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            Debug.LogWarning("Nam11 Animator not found on root, found in child: " + animator);
        }
        audioSource = GetComponent<AudioSource>();
        moveCoroutine = null;
        dogFollowPlayer = GetComponent<DogFollowPlayer>();
        navMeshAgent = GetComponent<NavMeshAgent>();
<<<<<<< Updated upstream
=======
        StartRandomRoaming();
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
=======
        StopRandomRoaming();
>>>>>>> Stashed changes
        StartCoroutine(MakeDogSitPrivate());
    }

    public void MakeDogWagTail() 
    {
<<<<<<< Updated upstream
=======
        StopRandomRoaming();
>>>>>>> Stashed changes
        StartCoroutine(MakeDogWagTailPrivate());
    }

    public void MakeDogAngry()
    {
<<<<<<< Updated upstream
=======
        StopRandomRoaming();
>>>>>>> Stashed changes
        StartCoroutine(MakeDogAngryPrivate());
    }

    public void MakeDogStopTransitionToIdle() 
    {
<<<<<<< Updated upstream
=======
        StopRandomRoaming();
>>>>>>> Stashed changes
        StartCoroutine(MakeDogStopTransitionToIdlePrivate());
    }

    public void MakeDogWalk() 
    {
<<<<<<< Updated upstream
=======
        StopRandomRoaming();
>>>>>>> Stashed changes
        StartCoroutine(MakeDogWalkPrivate());
    } 

    public void MakeDogComeHere(Vector3 target)
    {
<<<<<<< Updated upstream
=======
        StopRandomRoaming();
>>>>>>> Stashed changes
        StartCoroutine(MakeDogComeHerePrivate(target));
    }

    public void MakeDogMoveLeft()
    {
<<<<<<< Updated upstream
=======
        StopRandomRoaming();
>>>>>>> Stashed changes
        StartCoroutine(MakeDogMoveLeftPrivate());
    }

    public void MakeDogMoveRight()
    {
<<<<<<< Updated upstream
=======
        StopRandomRoaming();
>>>>>>> Stashed changes
        StartCoroutine(MakeDogMoveRightPrivate());
    }

    public void MakeDogMoveBackward()
    {
<<<<<<< Updated upstream
=======
        StopRandomRoaming();
>>>>>>> Stashed changes
        StartCoroutine(MakeDogMoveBackwardPrivate());
    }

    public void MakeDogGoThere(Transform destination)
    {
<<<<<<< Updated upstream
=======
        StopRandomRoaming();
>>>>>>> Stashed changes
        StartCoroutine(MakeDogGoTherePrivate(destination));
    }
    public void MakeDogGoEat(Transform destination)
    {
<<<<<<< Updated upstream
=======
        StopRandomRoaming();
>>>>>>> Stashed changes
        StartCoroutine(MakeDogGoEatPrivate(destination));
    }

    public void MakeDogFetchBall(Transform ball, Transform player)
    {
<<<<<<< Updated upstream
=======
        StopRandomRoaming();
>>>>>>> Stashed changes
        StartCoroutine(MakeDogFetchBallPrivate(ball, player));
    }   

    public void MakeDogFollowPlayer() 
    {
<<<<<<< Updated upstream
=======
        StopRandomRoaming();
>>>>>>> Stashed changes
        StartCoroutine(MakeDogFollowPlayerPrivate());
    }

    public void MakeDogComfortMe(Vector3 targetPosition)
    {
<<<<<<< Updated upstream
        StartCoroutine(MakeDogComfortMePrivate(targetPosition));
    }

=======
        StopRandomRoaming();
        StartCoroutine(MakeDogComfortMePrivate(targetPosition));
    }

    public void MakeDogWander() 
    {
        StartCoroutine(MakeDogWanderPrivate());
    }

    public void MakeDogStop() 
    {
        Debug.Log($"Nam11 MakeDogStop() start isBusy = {isBusy} isPlayingMath = {isPlayingMath}");
        isBusy = false;
        isPlayingMath = false;
        dogAnswers.Clear();
        lastMathQuestion = "";
        MakeDogStopTransitionToIdle();
    }

    public void MakeDogDoMath(string mathExpression) 
    {
        StartCoroutine(MakeDogDoMathPrivate(mathExpression));
    }

    public void DogCorrectMathRespond() 
    {
        StartCoroutine(DogCorrectMathRespondPrivate());
    }

    public void DogIncorrectMathRespond() 
    {
        StartCoroutine(DogIncorrectMathRespondPrivate());
    }

    public void DogTryPreviousMathProblemAgain() 
    {
        StartCoroutine(DogTryPreviousMathProblemAgainPrivate());
    } 

    public void DogEndMathSection() 
    {
        StartCoroutine(DogEndMathSectionPrivate());
        StartRandomRoaming();
    }


    public void MathGameSetup(Vector3 playerPostion, string setupString) 
    {
        StartCoroutine(MathGameSetupPrivate(playerPostion, setupString));
    }

>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
=======
        else if (stateInfo.IsName("WalkingFast")) 
        {
            Debug.Log("Nam11 Dog transition from walking to idle");
            Debug.Log("Nam11 Current animation: " + stateInfo.shortNameHash);
            yield return StartCoroutine(WaitAndMoveToIdle("WalkingFastToIdle"));

            //WaitAndMoveToIdle("WalkingNormalToIdle");
            // animator.SetTrigger("WalkingNormalToIdle");
        }
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
=======
        isBusy = true;
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        Debug.Log("Nam11 MakeDogSitPrivate() end isFollowing " + dogFollowPlayer.isFollowing);
=======
        Debug.Log($"Nam11 MakeDogSitPrivate() end isFollowing = {dogFollowPlayer.isFollowing} isBusy = {isBusy}");
>>>>>>> Stashed changes
    }

    private IEnumerator MakeDogWalkPrivate() 
    {
<<<<<<< Updated upstream
        Debug.Log("Nam11 MakeDogWalkPrivate() start");

        dogFollowPlayer.isFollowing = false;

=======
        Debug.Log($"Nam11 MakeDogWalkPrivate() start isBusy = {isBusy}");

        dogFollowPlayer.isFollowing = false;
        isBusy = true;
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
        Debug.Log("Nam11 MakeDogWalkPrivate() end isFollowing " + dogFollowPlayer.isFollowing);
=======
        Debug.Log($"Nam11 MakeDogWalkPrivate() end isFollowing = {dogFollowPlayer.isFollowing} isBusy = {isBusy}");
>>>>>>> Stashed changes
    }

    private IEnumerator MakeDogWagTailPrivate() 
    {
<<<<<<< Updated upstream
        Debug.Log("Nam11 MakeDogWagTailPrivate() start");       
=======
        Debug.Log($"Nam11 MakeDogWagTailPrivate() start isBusy = {isBusy}");       
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream

        Debug.Log("Nam11 MakeDogWagTailPrivate() end isFollowing " + dogFollowPlayer.isFollowing);
=======
        isBusy = true;
        Debug.Log($"Nam11 MakeDogWagTailPrivate() end isFollowing = dogFollowPlayer.isFollowing isBusy = {isBusy}");
>>>>>>> Stashed changes
    }

    private IEnumerator MakeDogAngryPrivate() 
    {
<<<<<<< Updated upstream
        Debug.Log("Nam11 MakeDogAngryPrivate() start");
=======
        Debug.Log($"Nam11 MakeDogAngryPrivate() start isBusy = {isBusy}");
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream

        Debug.Log("Nam11 MakeDogAngryPrivate() end isFollowing " + dogFollowPlayer.isFollowing);
=======
        isBusy = true;
        Debug.Log($"Nam11 MakeDogAngryPrivate() end isFollowing = dogFollowPlayer.isFollowing isBusy = {isBusy}");
>>>>>>> Stashed changes
    }

    private IEnumerator MakeDogComeHerePrivate(Vector3 target) 
    {
<<<<<<< Updated upstream
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
=======
        Debug.Log($"Nam11 MakeDogComeHerePrivate() start isBusy = {isBusy}");
        string actionAtTarget = "WigglingTail";
        isBusy = true;
        dogFollowPlayer.isFollowing = false;
        if (moveCoroutine != null || navMeshAgent.enabled) 
        {
            StopMovement();
        }

        yield return StartCoroutine(TransitionToIdle());
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget("WalkingFast"));
        //moveCoroutine = StartCoroutine(MoveToTarget(target, actionAtTarget, RUNNING_SPEED));
        navMeshAgent.enabled = true;
        navMeshAgent.stoppingDistance = 1f;
        navMeshAgent.speed = WALKING_SPEED;
        navMeshAgent.SetDestination(target);
        Debug.Log($"Nam11 navMeshAgent.pathPending = {navMeshAgent.pathPending} navMeshAgent.remainingDistance = {navMeshAgent.remainingDistance}");
        while (navMeshAgent.pathPending)
        {
            yield return null;
        }
        while (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance) 
        {
            Debug.Log($"Nam11 navMeshAgent.pathPending = {navMeshAgent.pathPending} navMeshAgent.remainingDistance = {navMeshAgent.remainingDistance} navMeshAgent.velocity.sqrMagnitude = {navMeshAgent.velocity.sqrMagnitude} navMeshAgent.hasPath = {navMeshAgent.hasPath}");
            yield return null;
        }
        yield return StartCoroutine(TransitionToIdle());
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget(actionAtTarget));    
             
        dogFollowPlayer.isFollowing = false;
        StopMovement();
        isBusy = false;  
        Debug.Log($"Nam11 MakeDogComeHerePrivate() end isFollowing = dogFollowPlayer.isFollowing isBusy = {isBusy}");
>>>>>>> Stashed changes
    }

    private IEnumerator MakeDogMoveLeftPrivate() 
    {
<<<<<<< Updated upstream
        Debug.Log("Nam11 MakeDogMoveLeftPrivate() start");
        string actionAtTarget = "WalkingNormal";
        if (moveCoroutine != null) 
=======
        Debug.Log($"Nam11 MakeDogMoveLeftPrivate() start moveCoroutine = {moveCoroutine}");
        string actionAtTarget = "WalkingNormal";
        if (moveCoroutine != null || navMeshAgent.enabled) 
>>>>>>> Stashed changes
        {
            StopMovement();
        }
        dogFollowPlayer.isFollowing = false;
<<<<<<< Updated upstream
=======
        isBusy = true;
>>>>>>> Stashed changes
        yield return StartCoroutine(TransitionToIdle());
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget(actionAtTarget));
        MoveInDirection(Vector3.left, WALKING_SPEED); // Global left

<<<<<<< Updated upstream
        Debug.Log("Nam11 MakeDogMoveLeftPrivate() end isFollowing " + dogFollowPlayer.isFollowing);
=======
        Debug.Log($"Nam11 MakeDogMoveLeftPrivate() end isFollowing = {dogFollowPlayer.isFollowing} isBusy = {isBusy}");
>>>>>>> Stashed changes
    }

    private IEnumerator MakeDogMoveRightPrivate() 
    {
<<<<<<< Updated upstream
        Debug.Log("Nam11 MakeDogMoveRightPrivate() start");
        string actionAtTarget = "WalkingNormal";
        if (moveCoroutine != null) 
=======
        Debug.Log($"Nam11 MakeDogMoveRightPrivate() start");
        string actionAtTarget = "WalkingNormal";
        if (moveCoroutine != null || navMeshAgent.enabled) 
>>>>>>> Stashed changes
        {
            StopMovement();
        }
        dogFollowPlayer.isFollowing = false;
        yield return StartCoroutine(TransitionToIdle());
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget(actionAtTarget));
        MoveInDirection(Vector3.right, WALKING_SPEED); // Global left
<<<<<<< Updated upstream

        Debug.Log("Nam11 MakeDogMoveRightPrivate() end isFollowing " + dogFollowPlayer.isFollowing);
=======
        isBusy = true;
        Debug.Log($"Nam11 MakeDogMoveRightPrivate() end isFollowing = {dogFollowPlayer.isFollowing} isBusy = {isBusy}");
>>>>>>> Stashed changes
    }

    private IEnumerator MakeDogMoveBackwardPrivate() 
    {
<<<<<<< Updated upstream
        Debug.Log("Nam11 MakeDogMoveBackwardPrivate() start");
        string actionAtTarget = "WalkingNormal";
        if (moveCoroutine != null) 
=======
        Debug.Log($"Nam11 MakeDogMoveBackwardPrivate() start");
        string actionAtTarget = "WalkingNormal";
        if (moveCoroutine != null || navMeshAgent.enabled) 
>>>>>>> Stashed changes
        {
            StopMovement();
        }
        dogFollowPlayer.isFollowing = false;
<<<<<<< Updated upstream
=======
        isBusy = true;
>>>>>>> Stashed changes
        yield return StartCoroutine(TransitionToIdle());
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget(actionAtTarget));
        MoveInDirection(Vector3.back, WALKING_SPEED); // Global left

<<<<<<< Updated upstream
        Debug.Log("Nam11 MakeDogMoveBackwardPrivate() end isFollowing " + dogFollowPlayer.isFollowing);
=======
        Debug.Log($"Nam11 MakeDogMoveBackwardPrivate() end isFollowing = {dogFollowPlayer.isFollowing} isBusy = {isBusy}");
>>>>>>> Stashed changes
    }

    private IEnumerator MakeDogGoTherePrivate(Transform destination) 
    {
<<<<<<< Updated upstream
        Debug.Log("Nam11 MakeDogGoTherePrivate() start");
        string actionAtTarget = "WigglingTail";
        if (moveCoroutine != null) 
=======
        Debug.Log($"Nam11 MakeDogGoTherePrivate() start");
        string actionAtTarget = "WigglingTail";
        if (moveCoroutine != null || navMeshAgent.enabled) 
>>>>>>> Stashed changes
        {
            StopMovement();
        }
        dogFollowPlayer.isFollowing = false;
<<<<<<< Updated upstream
        yield return StartCoroutine(TransitionToIdle());
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget("WalkingNormal"));
        moveCoroutine = StartCoroutine(MoveToTarget(destination.position, actionAtTarget, RUNNING_SPEED));

        Debug.Log("Nam11 MakeDogGoTherePrivate() end isFollowing " + dogFollowPlayer.isFollowing);
=======
        isBusy = true;
        yield return StartCoroutine(TransitionToIdle());
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget("WalkingNormal"));
        moveCoroutine = StartCoroutine(MoveToTarget(destination.position, actionAtTarget, WALKING_SPEED));
        isBusy = false;
        Debug.Log($"Nam11 MakeDogGoTherePrivate() end isFollowing = {dogFollowPlayer.isFollowing} isBusy = {isBusy}");
>>>>>>> Stashed changes
    }

    private IEnumerator MakeDogGoEatPrivate(Transform destination) 
    {
<<<<<<< Updated upstream
        Debug.Log("Nam11 MakeDogGoEatPrivate() start");
=======
        Debug.Log($"Nam11 MakeDogGoEatPrivate() start");
>>>>>>> Stashed changes
        string actionAtTarget = "EatingStart";
        if (TryGetComponent<NavMeshAgent>(out var agent))
        {
            agent.enabled = false;
        }

<<<<<<< Updated upstream
        if (moveCoroutine != null) 
=======
        if (moveCoroutine != null || navMeshAgent.enabled) 
>>>>>>> Stashed changes
        {
            StopMovement();
        }
        dogFollowPlayer.isFollowing = false;
<<<<<<< Updated upstream
        yield return StartCoroutine(TransitionToIdle());
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget("WalkingNormal"));
        moveCoroutine = StartCoroutine(MoveToTarget(destination.position, actionAtTarget, JOGGING_SPEED));
        if (agent != null) agent.enabled = true;

        Debug.Log("Nam11 MakeDogGoEatPrivate() end. isFollowing " + dogFollowPlayer.isFollowing);
=======
        isBusy = true;
        yield return StartCoroutine(TransitionToIdle());
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget("WalkingNormal"));
        moveCoroutine = StartCoroutine(MoveToTarget(destination.position, actionAtTarget, WALKING_SPEED));
        if (agent != null) agent.enabled = true;

        Debug.Log($"Nam11 MakeDogGoEatPrivate() end. isFollowing = {dogFollowPlayer.isFollowing} isBusy = {isBusy}");
>>>>>>> Stashed changes
    }

    private IEnumerator MakeDogFetchBallPrivate(Transform ball, Transform player) 
    {
<<<<<<< Updated upstream
        Debug.Log("Nam11 MakeDogFetchBallPrivate() start");
        if (moveCoroutine != null)
=======
        Debug.Log($"Nam11 MakeDogFetchBallPrivate() start  dogFollowPlayer.isFollowing = {dogFollowPlayer.isFollowing}");
        if (moveCoroutine != null || navMeshAgent.enabled)
>>>>>>> Stashed changes
        {
            StopMovement();
        }
        
        if (ball == null)
        {
            Debug.LogError("Nam11 ERROR1: Ball is null in MakeDogFetchBallPrivate!");
            yield break;
        }
        dogFollowPlayer.isFollowing = false;
<<<<<<< Updated upstream
=======
        isBusy = true;
>>>>>>> Stashed changes
        // Step 1: Transition to idle
        yield return StartCoroutine(TransitionToIdle());

        // Step 2: Run to the ball          
<<<<<<< Updated upstream
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget("Running"));
=======
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget("WalkingFast"));
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget("Running"));
=======
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget("WalkingFast"));
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        Debug.Log("Nam11 MakeDogFetchBallPrivate() end ball position " + ball.position);
=======
        isBusy = false;
        Debug.Log($"Nam11 MakeDogFetchBallPrivate() end ball position = {ball.position} isBusy = {isBusy}");
>>>>>>> Stashed changes
    }

    private IEnumerator MakeDogFollowPlayerPrivate() 
    {
<<<<<<< Updated upstream
        Debug.Log("Nam11 MakeDogFollowPlayerPrivate() start");
        if (moveCoroutine != null)
=======
        Debug.Log($"Nam11 MakeDogFollowPlayerPrivate() start");
        if (moveCoroutine != null || navMeshAgent.enabled)
>>>>>>> Stashed changes
        {
            StopMovement();
        }
        // Step 1: Transition to idle
        yield return StartCoroutine(TransitionToIdle());
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget("WalkingNormal"));
        dogFollowPlayer.isFollowing = true;
<<<<<<< Updated upstream
        
=======
        Debug.Log($"Nam11 MakeDogFollowPlayerPrivate() end dogFollowPlayer.isFollowing = {dogFollowPlayer.isFollowing}");
    }

    private IEnumerator MakeDogWanderPrivate() 
    {
        Debug.Log("Nam11 MakeDogWanderPrivate() start");
        if (moveCoroutine != null || navMeshAgent.enabled)
        {
            StopMovement();
        }
        isBusy = false;
        isPlayingMath = false;
        dogAnswers.Clear();
        lastMathQuestion = "";
        yield return StartCoroutine(TransitionToIdle());
        StartRandomRoaming();
        Debug.Log($"Nam11 MakeDogWanderPrivate() end isBusy = {isBusy}");
>>>>>>> Stashed changes
    }

    public void ThrowBallAndFetch(Transform ball, Transform player)
    {
        StartCoroutine(ThrowBallAndFetchPrivate(ball, player));
    }

    private IEnumerator ThrowBallAndFetchPrivate(Transform ball, Transform player)
    {
<<<<<<< Updated upstream
        Debug.Log("Nam11 ThrowBallAndFetchPrivate() start");
        dogFollowPlayer.isFollowing = false;
        if (moveCoroutine != null) 
        {
            StopMovement();
        }
=======
        Debug.Log($"Nam11 ThrowBallAndFetchPrivate() start");
        dogFollowPlayer.isFollowing = false;
        if (moveCoroutine != null || navMeshAgent.enabled) 
        {
            StopMovement();
        }
        isBusy = true;
>>>>>>> Stashed changes
        
        yield return StartCoroutine(TransitionToIdle());

        // 1. Simulate ball throw: pick a random spot in front of player
<<<<<<< Updated upstream
        Vector3 throwDirection = player.forward + player.right * Random.Range(-0.5f, 0.5f);
        Vector3 throwTarget = player.position + throwDirection.normalized * Random.Range(3f, 6f);
=======
        Vector3 throwDirection = player.forward + player.right * UnityEngine.Random.Range(-0.5f, 0.5f);
        Vector3 throwTarget = player.position + throwDirection.normalized * UnityEngine.Random.Range(3f, 6f);
>>>>>>> Stashed changes
        throwTarget.y = 0f;

        Debug.Log("Nam11 ThrowBall target = " + throwTarget);
        ball.SetParent(null);
        ball.position = throwTarget;

        yield return new WaitForSeconds(0.5f); // small pause to simulate throw time

        // 2. Command dog to fetch again
        yield return StartCoroutine(MakeDogFetchBallPrivate(ball, player));
        moveCoroutine = null;
<<<<<<< Updated upstream
        Debug.Log("Nam11 ThrowBallAndFetchPrivate() end");
    }
    private bool ShouldStartFollowingAfterAction(string action)
    {
        return action != "EatingStart";
=======
        isBusy = false;
        Debug.Log($"Nam11 ThrowBallAndFetchPrivate() end isBusy = {isBusy}");
>>>>>>> Stashed changes
    }

    private IEnumerator MakeDogComfortMePrivate(Vector3 target)
    {
<<<<<<< Updated upstream
        Debug.Log("Nam11 MakeDogComfortMePrivate() start");
        
        if (moveCoroutine != null)
=======
        Debug.Log($"Nam11 MakeDogComfortMePrivate() start isBusy = {isBusy}");
        
        if (moveCoroutine != null || navMeshAgent.enabled)
>>>>>>> Stashed changes
        {
            StopMovement();
        }

        dogFollowPlayer.isFollowing = false;
<<<<<<< Updated upstream

=======
        isBusy = true;
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        Debug.Log("Nam11 MakeDogComfortMePrivate() end");
=======
        isBusy = false;
        Debug.Log($"Nam11 MakeDogComfortMePrivate() end isBusy = {isBusy}");
>>>>>>> Stashed changes
    }

    //TODO
    // private IEnumerable MakeDogHappyPrivate(Vector3 target) 
    // {
        
    // }
<<<<<<< Updated upstream
=======

    public void StartRandomRoaming()
    {
        if (randomRoamCoroutine == null)
        {
            randomRoamCoroutine = StartCoroutine(RandomRoamingLoop());
        }
    }

    public void StopRandomRoaming()
    {
        if (randomRoamCoroutine != null)
        {
            StopCoroutine(randomRoamCoroutine);
            randomRoamCoroutine = null;
        }
    }

    private IEnumerator RandomRoamingLoop()
    {
        if (dogFollowPlayer)
        {
            Debug.Log($"Nam11: RandomRoamingLoop() start dogFollowPlayer.isFollowing = {dogFollowPlayer.isFollowing} isBusy = {isBusy}");
        }
        while (true && dogFollowPlayer != null)
        {
            //Debug.Log($"Nam11: RandomRoamingLoop() start dogFollowPlayer.isFollowing = {dogFollowPlayer.isFollowing} isBusy = {isBusy}");
            if (dogFollowPlayer.isFollowing || isBusy)
            {
                yield return null;
                continue;
            }

            Vector3 randomPosition = new Vector3(
                UnityEngine.Random.Range(minBounds.x, maxBounds.x),
                transform.position.y,
                UnityEngine.Random.Range(minBounds.z, maxBounds.z)
            );

            yield return StartCoroutine(TransitionToIdle());
            navMeshAgent.enabled = true;
            navMeshAgent.stoppingDistance = 0.1f;
            navMeshAgent.speed = WANDERING_SPEED;
            navMeshAgent.SetDestination(randomPosition);
            animator.SetTrigger("WalkingNormal");
            //float distanceToTarget = Vector3.Distance(transform.position, randomPosition);
            //Debug.Log($"Nam11: navMeshAgent.remainingDistance = {navMeshAgent.remainingDistance} randomPosition = {randomPosition}");
            while (navMeshAgent.remainingDistance > DISTANCE_TO_TARGET)
            {
                if (dogFollowPlayer.isFollowing || isBusy) yield break;
                yield return null;
                //distanceToTarget = Vector3.Distance(transform.position, randomPosition);
                //Debug.Log($"Nam11: Moving to random target dogFollowPlayer.isFollowing = {dogFollowPlayer.isFollowing} isBusy = {isBusy} distanceToTarget = {navMeshAgent.remainingDistance}");
            }

            yield return StartCoroutine(TransitionToIdle());
            yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 5f));
            //Debug.Log($"Nam11: RandomRoamingLoop() end dogFollowPlayer.isFollowing = {dogFollowPlayer.isFollowing} isBusy = {isBusy}");
        }
        Debug.Log("Nam11: RandomRoamingLoop() end isBusy = {isBusy}");
    }

    private IEnumerator MakeDogDoMathPrivate(string mathExpression)
    {
        Debug.Log($"Nam11 MakeDogDoMathPrivate() start mathExpression: {mathExpression} isPlayingMath = {isPlayingMath} isBusy = {isBusy}");
        if (isPlayingMath == true) 
        {
            int answer = predefinedMathExpressions[mathExpression];
            yield return StartCoroutine(TransitionToIdle());
            yield return StartCoroutine(TriggerAndWaitForTransitionToTarget("AngryStart"));
            yield return StartCoroutine(BarkNTimesCoroutine(answer));
            yield return StartCoroutine(TransitionToIdle());
            dogAnswers.Insert(0, answer);
            lastMathQuestion = mathExpression;
        }
        Debug.Log($"Nam11 MakeDogDoMathPrivate() end isPlayingMath = {isPlayingMath} isBusy = {isBusy}");
    }

    private IEnumerator BarkNTimesCoroutine(int count)
    {
        Debug.Log($"Nam11 BarkNTimesCoroutine() start count = {count}");
        for (int i = 0; i < count; i++)
        {
            if (singleBarkSoundMath != null)
            {
                singleBarkSoundMath.Play();
                Debug.Log($"Nam11: Barking {i + 1}/{count}");
            }
            yield return new WaitForSeconds(singleBarkSoundMath.clip.length + 0.1f); // slight delay between barks
        }
        Debug.Log("Nam11 BarkNTimesCoroutine() end");
        //isBusy = false;
        //StartRandomRoaming();
    }

    private IEnumerator DogRespondToSetupMathGame() 
    {
        Debug.Log("Nam11 DogRespondToSetupMathGame() start");
        string animation = "AngryStart";
        yield return StartCoroutine(TransitionToIdle());
        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget(animation));
        yield return StartCoroutine(BarkNTimesCoroutine(1));
        yield return StartCoroutine(TransitionToIdle());
        Debug.Log("Nam11 DogRespondToSetupMathGame() end");
    }

    private IEnumerator DogCorrectMathRespondPrivate() 
    {
        Debug.Log($"Nam11 DogCorrectMathRespondPrivate() start isPlayingMath = {isPlayingMath} dogAnswers.Count = {dogAnswers.Count} lastMathQuestion = {lastMathQuestion} isBusy = {isBusy}");
        if (isPlayingMath == true) 
        {
            if (dogAnswers.Count != 0) 
            {
                if (lastMathQuestion != "") 
                {                  
                    Debug.Log($"Nam11 Update the dictonary with correct answer. lastMathQuestion = {lastMathQuestion} dogAnswers[LAST_DOG_ANSWER] = {dogAnswers[LAST_DOG_ANSWER]}");
                    predefinedMathExpressions[lastMathQuestion] = dogAnswers[LAST_DOG_ANSWER];
                    yield return StartCoroutine(TransitionToIdle());
                    yield return StartCoroutine(TriggerAndWaitForTransitionToTarget("WigglingTail"));
                    lastMathQuestion = "";
                    dogAnswers.Clear();
                }
                else 
                {
                    Debug.Log("Nam11 Not suppose to be here.");
                }
            }
        }
        Debug.Log($"Nam11 DogCorrectMathRespondPrivate() end isPlayingMath = {isPlayingMath} isBusy = {isBusy}");
    }

    private IEnumerator DogIncorrectMathRespondPrivate() 
    {
        Debug.Log($"Nam11 DogIncorrectMathRespondPrivate() start isPlayingMath = {isPlayingMath} isBusy = {isBusy}");
        if (isPlayingMath == true) 
        {
            yield return StartCoroutine(TransitionToIdle());
            if (incorrectMathSorrySound != null && incorrectMathSorrySound.clip != null)
            {
                Debug.Log("Nam11: DogIncorrectMathRespondPrivate() playing sad sound.");
                incorrectMathSorrySound.Play();
            }
            else
            {
                Debug.LogWarning("Nam11: DogIncorrectMathRespondPrivate() sad sound is missing.");
            }
        }
        Debug.Log($"Nam11 DogIncorrectMathRespondPrivate() end isPlayingMath = {isPlayingMath} isBusy = {isBusy}");
    }

    private IEnumerator DogTryPreviousMathProblemAgainPrivate() 
    {
        Debug.Log($"Nam11 DogIncorrectMathRespondPrivate() start isPlayingMath = {isPlayingMath} isBusy = {isBusy}");
        if (isPlayingMath == true) 
        {
            int newAnswer = RandomGeneratorAnswer();
            yield return StartCoroutine(TransitionToIdle());
            yield return StartCoroutine(TriggerAndWaitForTransitionToTarget("AngryStart"));
            yield return StartCoroutine(BarkNTimesCoroutine(newAnswer));
            yield return StartCoroutine(TransitionToIdle());
            dogAnswers.Insert(0, newAnswer);
        }
        Debug.Log($"Nam11 DogTryPreviousMathProblemAgainPrivate() end isBusy = {isBusy}");
    }

    private int RandomGeneratorAnswer() 
    {
        int number = 1;
        if (lastMathQuestion != "") 
        {
            int answer = correctAnswer[lastMathQuestion]; 
            number = UnityEngine.Random.Range(answer - 1, answer + 2);
            while (dogAnswers.Contains(number)) 
            {
                number = UnityEngine.Random.Range(answer - 1, answer + 2);
            }
            Debug.Log($"Nam11 RandomGeneratorAnswer() end number = {number}");
        }
        return number;
    }

    private IEnumerator DogEndMathSectionPrivate() 
    {
        Debug.Log($"Nam11 DogEndMathSectionPrivate() start isPlayingMath = {isPlayingMath} isBusy = {isBusy}");
        isBusy = false;
        isPlayingMath = false;
        dogAnswers.Clear();
        lastMathQuestion = "";
        yield return StartCoroutine(TransitionToIdle());
        Debug.Log($"Nam11 DogEndMathSectionPrivate() end isPlayingMath = {isPlayingMath} isBusy = {isBusy}");
    }

    private IEnumerator MathGameSetupPrivate(Vector3 playerPostion, string setupString) 
    {
        Debug.Log($"Nam11: MathGameSetup() start. setupString = {setupString} isBusy = {isBusy} isPlayingMath = {isPlayingMath}");
        if (setupString == "let play math game") 
        {
            yield return StartCoroutine(MakeDogComeHerePrivate(playerPostion));
            isBusy = true;  
        }
        else if (setupString == "are you ready" && isBusy == true) 
        {         
            yield return StartCoroutine(DogRespondToSetupMathGame());
            isPlayingMath = true;
        }
        else if (setupString == "let go" && isBusy == true) 
        {
            yield return StartCoroutine(DogRespondToSetupMathGame());
            isPlayingMath = true;
            dogAnswers.Clear();
            lastMathQuestion = "";
        }
        else 
        {
            Debug.Log("Nam11: MathGameSetup() Not suppose to be here.");
        }
        Debug.Log($"Nam11: MathGameSetup() end. isBusy = {isBusy} isPlayingMath = {isPlayingMath}");
    }
>>>>>>> Stashed changes
}
