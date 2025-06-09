#define DEBUG
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Mathematics;
using Meta.XR.MRUtilityKit;
using Unity.AI.Navigation;
using Meta.XR.MRUtilityKit.SceneDecorator;
using Meta.XR.MRUtilityKitSamples.BouncingBall;

public class DogMovement : MonoBehaviour
{
    public float rotationSpeed = 500f;
    public Vector3 minBounds = new Vector3(-10f, 0f, -10f);
    public Vector3 maxBounds = new Vector3(10f, 0f, 10f);
    public Transform ballHoldPoint;
    public NavMeshAgent navMeshAgent;
    public DogFollowPlayer dogFollowPlayer { get; set; }
    public AudioSource comfortDogSound;
    public AudioSource angryDogSound;
    public AudioSource singleBarkSoundMath;
    public AudioSource incorrectMathSorrySound;
    public bool isBusy = false;
    public bool isPlayingMath = false;
    public const int WAIT_CYCLE = 40;
    public const float MOVING_TIME = 20.0f;
    public const float WANDERING_SPEED = 0.4f;
    public const float WALKING_SPEED = 1f;
    public const float JOGGING_SPEED = 1f;
    public const float RUNNING_SPEED = 2f;
    public const float WAITING_IN_MS = 0.5f;
    public const float DISTANCE_TO_TARGET = 0.2f;
    public const int LAST_DOG_ANSWER = 0;
    public const float BALL_DROPPING_TO_PLAYER_DISTANCE = 0.3f;
    public const float BALL_FETCHING_TIMEOUT = 10.0f;
    private const float WANDERING_TIMEOUT = 15.0f;
    public Transform player;
    private Animator animator;
    private Coroutine dogActionCoroutine = null;
    private Coroutine nesteddogActionCoroutine = null;
    private AudioSource audioSource;
    private Coroutine randomRoamCoroutine = null;
    private Coroutine nestedrandomRoamCoroutine = null;
    private Coroutine nestedTransitionToIdleCoroutine = null;
    private List<int> dogAnswers;
    private string lastMathQuestion = "";
    private Queue<Transform> ballDoneQueue = new();

    BouncingcBallMgr bouncingcBallMgr; 

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

    void Start()
    {
        Debug.Log($"Nam11 DogMovement start animator = {animator}");
        animator = GetComponent<Animator>();
        dogAnswers = new List<int>();
        lastMathQuestion = null;
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            Debug.LogWarning("Nam11 Animator not found on root, found in child: " + animator);
        }
        audioSource = GetComponent<AudioSource>();
        //moveCoroutine = null;
        dogFollowPlayer = GetComponent<DogFollowPlayer>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        isBusy = false;
        bouncingcBallMgr = FindObjectOfType<BouncingcBallMgr>();
        randomRoamCoroutine = StartCoroutine(RandomRoamingLoop());
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
        StopRandomRoaming();
        bouncingcBallMgr.Reset();
        dogActionCoroutine = StartCoroutine(MakeDogSitPrivate());
    }

    public void MakeDogWagTail()
    {
        StopRandomRoaming();
        bouncingcBallMgr.Reset();
        dogActionCoroutine = StartCoroutine(MakeDogWagTailPrivate());
    }

    public void MakeDogAngry()
    {
        StopRandomRoaming();
        bouncingcBallMgr.Reset();
        dogActionCoroutine = StartCoroutine(MakeDogAngryPrivate());
    }

    public void MakeDogStopTransitionToIdle()
    {
        StopRandomRoaming();
        bouncingcBallMgr.Reset();
        StartCoroutine(MakeDogStopTransitionToIdlePrivate());
    }

    public void MakeDogWalkForward()
    {
        StopRandomRoaming();
        bouncingcBallMgr.Reset();
        dogActionCoroutine = StartCoroutine(MakeDogWalkForwardPrivate());
    }

    public void MakeDogComeHere()
    {
        StopRandomRoaming();
        bouncingcBallMgr.Reset();
        dogActionCoroutine = StartCoroutine(MakeDogComeHerePrivate());
    }

    public void MakeDogMoveLeft()
    {
        StopRandomRoaming();
        bouncingcBallMgr.Reset();
        dogActionCoroutine = StartCoroutine(MakeDogMoveLeftPrivate());
    }

    public void MakeDogMoveRight()
    {
        StopRandomRoaming();
        bouncingcBallMgr.Reset();
        dogActionCoroutine = StartCoroutine(MakeDogMoveRightPrivate());
    }

    public void MakeDogMoveBackward()
    {
        StopRandomRoaming();
        bouncingcBallMgr.Reset();
        dogActionCoroutine = StartCoroutine(MakeDogMoveBackwardPrivate());
    }

    public void MakeDogGoThere(Vector3 destination)
    {
        StopRandomRoaming();
        bouncingcBallMgr.Reset();
        dogActionCoroutine = StartCoroutine(MakeDogGoTherePrivate(destination));
    }
    public void MakeDogGoEat(Transform destination)
    {
        StopRandomRoaming();
        bouncingcBallMgr.Reset();
        dogActionCoroutine = StartCoroutine(MakeDogGoEatPrivate(destination));
    }

    public void MakeDogFetchBall(Transform ball, System.Action onComplete = null)
    {
        Debug.Log("Nam11 MakeDogFetchBall start");
        StopRandomRoaming();
        dogActionCoroutine = StartCoroutine(MakeDogFetchBallPrivate(ball, onComplete));
        Debug.Log("Nam11 MakeDogFetchBall end");
    }

    public void MakeDogFollowPlayer()
    {
        StopRandomRoaming();
        bouncingcBallMgr.Reset();
        dogActionCoroutine = StartCoroutine(MakeDogFollowPlayerPrivate());
    }

    public void MakeDogComfortMe()
    {
        StopRandomRoaming();
        bouncingcBallMgr.Reset();
        dogActionCoroutine = StartCoroutine(MakeDogComfortMePrivate());
    }

    public void MakeDogWander()
    {
        dogActionCoroutine = StartCoroutine(MakeDogWanderPrivate());
    }

    public void MakeDogStop()
    {
        Debug.Log($"Nam11 MakeDogStop() start isBusy = {isBusy} isPlayingMath = {isPlayingMath}");
        isBusy = false;
        isPlayingMath = false;
        dogAnswers.Clear();
        lastMathQuestion = "";
        bouncingcBallMgr.Reset(); 
        MakeDogStopTransitionToIdle();
    }

    public void MakeDogDoMath(string mathExpression)
    {
        bouncingcBallMgr.Reset();
        dogActionCoroutine = StartCoroutine(MakeDogDoMathPrivate(mathExpression));
    }

    public void DogCorrectMathRespond()
    {
        dogActionCoroutine = StartCoroutine(DogCorrectMathRespondPrivate());
    }

    public void DogIncorrectMathRespond()
    {
        dogActionCoroutine = StartCoroutine(DogIncorrectMathRespondPrivate());
    }

    public void DogTryPreviousMathProblemAgain()
    {
        dogActionCoroutine = StartCoroutine(DogTryPreviousMathProblemAgainPrivate());
    }

    public void DogEndMathSection()
    {
        dogActionCoroutine = StartCoroutine(DogEndMathSectionPrivate());
        randomRoamCoroutine = StartCoroutine(RandomRoamingLoop());
    }


    public void MathGameSetup(string setupString)
    {
        dogActionCoroutine = StartCoroutine(MathGameSetupPrivate(setupString));
    }

    //TODO
    // public void MakeDogHappy(Vector3 targetPosition)
    // {
    //     StartCoroutine(MakeDogHappyPrivate(targetPosition));
    // }


    private void StopMovement()
    {
        Debug.Log($"Nam11 StopMovement() start dogActionCoroutine = {dogActionCoroutine} nestedrandomRoamCoroutine = {nestedrandomRoamCoroutine} randomRoamCoroutine = {randomRoamCoroutine}");
        if (dogActionCoroutine != null)
        {
            StopCoroutine(dogActionCoroutine);
            dogActionCoroutine = null;
            //dogFollowPlayer.isFollowing = true;
        }
        if (nesteddogActionCoroutine != null)
        {
            StopCoroutine(nesteddogActionCoroutine);
            nesteddogActionCoroutine = null;
        }

        StopRandomRoaming();

        dogFollowPlayer.isFollowing = false;
        Debug.Log($"Nam11 StopMovement() end dogActionCoroutine = {dogActionCoroutine} nestedrandomRoamCoroutine = {nestedrandomRoamCoroutine} randomRoamCoroutine = {randomRoamCoroutine}");
    }

    /*     private IEnumerator MoveToTarget(Vector3 target, string actionAtTarget, float movingSpeed)
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
                            //Debug.Log("Nam11 Move to target loop. Distance to target " + Vector3.Distance(transform.position, target));
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

                    yield return StartCoroutine(TransitionToIdle());
                    if (actionAtTarget != null)
                    {
                        Debug.Log("Nam11 MoveToTarget transition to next state " + " actionAtTarget " + actionAtTarget);
                        yield return StartCoroutine(TriggerAndWaitForTransitionToTarget(actionAtTarget));
                    }
                } */

    private IEnumerator TransitionToIdle()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log("Nam11 TransitionToIdle() start.");
        IEnumerator enumerator;

        if (stateInfo.IsName("Idle"))
        {
            Debug.Log("Nam11 Current animation: " + stateInfo.shortNameHash);
            Debug.Log("Nam11 Dog is already in Idle State. No need to transition.");
        }
        else if (stateInfo.IsName("Running"))
        {
            Debug.Log("Nam11 Dog transition from running to idle");
            enumerator = WaitAndMoveToIdle("RunningToIdle");
            nestedTransitionToIdleCoroutine = StartCoroutine(enumerator);
            yield return enumerator;
        }
        else if (stateInfo.IsName("WalkingFast"))
        {
            Debug.Log("Nam11 Dog transition from walking to idle");
            Debug.Log("Nam11 Current animation: " + stateInfo.shortNameHash);
            enumerator = WaitAndMoveToIdle("WalkingFastToIdle");
            nestedTransitionToIdleCoroutine = StartCoroutine(enumerator);
            yield return enumerator;
        }
        else if (stateInfo.IsName("WalkingNormal"))
        {
            Debug.Log("Nam11 Dog transition from walking to idle");
            Debug.Log("Nam11 Current animation: " + stateInfo.shortNameHash);
            enumerator = WaitAndMoveToIdle("WalkingNormalToIdle");
            nestedTransitionToIdleCoroutine = StartCoroutine(enumerator);
            yield return enumerator;
        }
        else if (stateInfo.IsName("SittingCycle"))
        {
            Debug.Log("Nam11 Dog transition from SittingCycle to idle");
            Debug.Log("Nam11 Current animation: " + stateInfo.shortNameHash);
            enumerator = WaitAndMoveToIdle("SittingCycleToIdle");
            nestedTransitionToIdleCoroutine = StartCoroutine(enumerator);
            yield return enumerator;
        }
        else if (stateInfo.IsName("Breathing"))
        {
            Debug.Log("Nam11 Dog is in Breathing cycle to idle.");
            enumerator = WaitAndMoveToIdle("BreathingToIdle");
            nestedTransitionToIdleCoroutine = StartCoroutine(enumerator);
            yield return enumerator;
        }
        else if (stateInfo.IsName("AngryCycle"))
        {
            Debug.Log("Nam11 Dog is in AngryCycle to idle.");
            enumerator = WaitAndMoveToIdle("AngryCycleToIdle");
            nestedTransitionToIdleCoroutine = StartCoroutine(enumerator);
            yield return enumerator;
        }
        else if (stateInfo.IsName("WigglingTail"))
        {
            Debug.Log("Nam11 Dog transition from WigglingTail to idle");
            Debug.Log("Nam11 Current animation: " + stateInfo.shortNameHash);
            enumerator = WaitAndMoveToIdle("WigglingTailToIdle");
            nestedTransitionToIdleCoroutine = StartCoroutine(enumerator);
            yield return enumerator;
        }
        else if (stateInfo.IsName("EatingCycle") || stateInfo.IsName("EatingStart"))
        {
            Debug.Log("Nam11 Dog is in EatingCycle or EatingStart to idle.");
            enumerator = WaitAndMoveToIdle("EatingCycleToIdle");
            nestedTransitionToIdleCoroutine = StartCoroutine(enumerator);
            yield return enumerator;
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
                yield return StartCoroutine(WaitForSecond(WAITING_IN_MS));
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

        IEnumerator enumerator;

        enumerator = TransitionToIdle();
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

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
            yield return StartCoroutine(WaitForSecond(WAITING_IN_MS));
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

    /*     private IEnumerator MoveInDirection(Vector3 direction, float currentSpeed)
        {
            Debug.Log("Nam11 MoveInDirection() called with direction: " + direction);
            yield return StartCoroutine(MoveOverTime(direction, MOVING_TIME, currentSpeed));
        } */

    private IEnumerator MoveOverTime(Vector3 direction, float duration = MOVING_TIME, float currentSpeed = WALKING_SPEED)
    {
        Debug.Log("Nam11 MoveOverTime() start direction = " + direction);
        IEnumerator enumerator;

        enumerator = RotateTowardDirection(direction);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        float distance = 10.0f;
        Vector3 targetPosition = transform.position + direction * distance;
        targetPosition.y = 0;

        enumerator = MakeDogNavigateToTarget(targetPosition);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        enumerator = TransitionToIdle();
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        Debug.Log($"Nam11 MoveOverTime() end targetPosition = {targetPosition}");
    }

    private IEnumerator MakeDogStopTransitionToIdlePrivate()
    {
        StopMovement();
        dogFollowPlayer.isFollowing = false;
        yield return StartCoroutine(TransitionToIdle());
    }

    private IEnumerator MakeDogSitPrivate()
    {
        Debug.Log("Nam11 MakeDogSitPrivate() start animator = " + animator);
        IEnumerator enumerator;
        StopMovement(); // stop the moving routine

        dogFollowPlayer.isFollowing = false;
        isBusy = true;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("SittingCycle")) // check if the dog is not sitting
        {
            enumerator = TransitionToIdle();
            nesteddogActionCoroutine = StartCoroutine(enumerator);
            yield return enumerator;

            enumerator = TriggerAndWaitForTransitionToTarget("SittingStart");
            nesteddogActionCoroutine = StartCoroutine(enumerator);
            yield return enumerator;

            Debug.Log("Nam11 Dog is sucessfully transtion to sitting.");
        }
        else // dog is alaready sitting. leave it alone
        {
            Debug.Log("Nam11 Dog is already in sitting state.");
        }
        Debug.Log($"Nam11 MakeDogSitPrivate() end isFollowing = {dogFollowPlayer.isFollowing} isBusy = {isBusy}");
    }

    private IEnumerator MakeDogWalkForwardPrivate()
    {
        Debug.Log($"Nam11 MakeDogWalkPrivate() start isBusy = {isBusy}");
        IEnumerator enumerator;
        dogFollowPlayer.isFollowing = false;
        isBusy = true;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        string actionAtTarget = "WalkingNormal";
        if (!stateInfo.IsName("WalkingNormal"))
        {
            StopMovement();
            enumerator = TransitionToIdle();
            nesteddogActionCoroutine = StartCoroutine(enumerator);
            yield return enumerator;
            enumerator = TriggerAndWaitForTransitionToTarget(actionAtTarget);
            nesteddogActionCoroutine = StartCoroutine(enumerator);
            yield return enumerator;
            Debug.Log("Nam11 Dog is sucessfully transtion to WalkingNormal.");
        }
        else
        {
            Debug.Log("Nam11 Dog is walking.");
        }
        Vector3 forwardOfPlayer = player.forward;
        enumerator = MoveOverTime(forwardOfPlayer);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;
        yield return StartCoroutine(WaitForSecond(1f));
        isBusy = false;
        Debug.Log($"Nam11 MakeDogWalkPrivate() end isFollowing = {dogFollowPlayer.isFollowing} isBusy = {isBusy}");
    }

    private IEnumerator MakeDogWagTailPrivate()
    {
        Debug.Log($"Nam11 MakeDogWagTailPrivate() start isBusy = {isBusy}");
        IEnumerator enumerator;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        string actionAtTarget = "WigglingTail";
        StopMovement();

        if (!stateInfo.IsName("WigglingTail"))
        {
            enumerator = TransitionToIdle();
            nesteddogActionCoroutine = StartCoroutine(enumerator);
            yield return enumerator;

            enumerator = TriggerAndWaitForTransitionToTarget(actionAtTarget);
            nesteddogActionCoroutine = StartCoroutine(enumerator);
            yield return enumerator;
        }
        else
        {
            Debug.Log("Nam11 Dog is wagging tail.");
        }
        isBusy = true;
        Debug.Log($"Nam11 MakeDogWagTailPrivate() end isFollowing = dogFollowPlayer.isFollowing isBusy = {isBusy}");
    }

    private IEnumerator MakeDogAngryPrivate()
    {
        Debug.Log($"Nam11 MakeDogAngryPrivate() start isBusy = {isBusy}");
        IEnumerator enumerator;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        string actionAtTarget = "AngryStart";
        StopMovement();

        if (!stateInfo.IsName("AngryCycle"))
        {
            enumerator = TransitionToIdle();
            nesteddogActionCoroutine = StartCoroutine(enumerator);
            yield return enumerator;

            enumerator = TriggerAndWaitForTransitionToTarget(actionAtTarget);
            nesteddogActionCoroutine = StartCoroutine(enumerator);
            yield return enumerator;

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
        isBusy = true;
        Debug.Log($"Nam11 MakeDogAngryPrivate() end isFollowing = dogFollowPlayer.isFollowing isBusy = {isBusy}");
    }

    private IEnumerator MakeDogComeHerePrivate()
    {
        Debug.Log($"Nam11 MakeDogComeHerePrivate() start isBusy = {isBusy}");
        IEnumerator enumerator;
        string actionAtTarget = "WigglingTail";
        isBusy = true;
        dogFollowPlayer.isFollowing = false;
        StopMovement();

        enumerator = TransitionToIdle();
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        enumerator = TriggerAndWaitForTransitionToTarget("WalkingFast");
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;


        Vector3 playerPosition = player.position + player.forward * 0.6f;
        Vector3 direction = (player.position - transform.position).normalized;
        playerPosition.y = 0f;

        enumerator = RotateTowardDirection(direction);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        enumerator = MakeDogNavigateToTarget(playerPosition);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        direction = (player.position - transform.position).normalized;

        enumerator = RotateTowardDirection(direction);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        enumerator = TransitionToIdle();
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        enumerator = TriggerAndWaitForTransitionToTarget(actionAtTarget);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        Debug.Log($"Nam11 MakeDogComeHerePrivate() end isFollowing = dogFollowPlayer.isFollowing isBusy = {isBusy}");
    }

    private IEnumerator MakeDogMoveLeftPrivate()
    {
        Debug.Log($"Nam11 MakeDogMoveLeftPrivate() start dogActionCoroutine = {dogActionCoroutine}");
        IEnumerator enumerator;
        string actionAtTarget = "WalkingNormal";
        StopMovement();
        dogFollowPlayer.isFollowing = false;
        isBusy = true;
        enumerator = TransitionToIdle();
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        enumerator = TriggerAndWaitForTransitionToTarget(actionAtTarget);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        Vector3 leftOfPlayer = -player.right;
        enumerator = MoveOverTime(leftOfPlayer);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        isBusy = false;
        Debug.Log($"Nam11 MakeDogMoveLeftPrivate() end isFollowing = {dogFollowPlayer.isFollowing} isBusy = {isBusy}");
    }

    private IEnumerator MakeDogMoveRightPrivate()
    {
        Debug.Log($"Nam11 MakeDogMoveRightPrivate() start");
        IEnumerator enumerator;
        string actionAtTarget = "WalkingNormal";
        StopMovement();
        dogFollowPlayer.isFollowing = false;
        isBusy = true;
        enumerator = TransitionToIdle();
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        enumerator = TriggerAndWaitForTransitionToTarget(actionAtTarget);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        Vector3 rightOfPlayer = player.right;
        enumerator = MoveOverTime(rightOfPlayer);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        isBusy = false;
        Debug.Log($"Nam11 MakeDogMoveRightPrivate() end isFollowing = {dogFollowPlayer.isFollowing} isBusy = {isBusy}");
    }

    private IEnumerator MakeDogMoveBackwardPrivate()
    {
        Debug.Log($"Nam11 MakeDogMoveBackwardPrivate() start");
        IEnumerator enumerator;
        string actionAtTarget = "WalkingNormal";
        StopMovement();
        dogFollowPlayer.isFollowing = false;
        isBusy = true;
        enumerator = TransitionToIdle();
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        enumerator = TriggerAndWaitForTransitionToTarget(actionAtTarget);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        Vector3 backOfPlayer = -player.forward;
        enumerator = MoveOverTime(backOfPlayer);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;
        isBusy = false;
        Debug.Log($"Nam11 MakeDogMoveBackwardPrivate() end isFollowing = {dogFollowPlayer.isFollowing} isBusy = {isBusy}");
    }

    private IEnumerator MakeDogGoTherePrivate(Vector3 destination)
    {
        Debug.Log($"Nam11 MakeDogGoTherePrivate() start");
        IEnumerator enumerator;
        string actionAtTarget = "WalkingNormal";
        StopMovement();
        dogFollowPlayer.isFollowing = false;
        isBusy = true;
        enumerator = TransitionToIdle();
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        enumerator = TriggerAndWaitForTransitionToTarget(actionAtTarget);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        enumerator = MakeDogNavigateToTarget(destination);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        enumerator = TransitionToIdle();
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        isBusy = false;
        Debug.Log($"Nam11 MakeDogGoTherePrivate() end isFollowing = {dogFollowPlayer.isFollowing} isBusy = {isBusy}");
    }

    private IEnumerator MakeDogGoEatPrivate(Transform destination)
    {
        Debug.Log($"Nam11 MakeDogGoEatPrivate() start destination.position = {destination.position}");
        IEnumerator enumerator;
        string actionAtTarget = "EatingStart";
        if (TryGetComponent<NavMeshAgent>(out var agent))
        {
            agent.enabled = false;
        }

        StopMovement();
        dogFollowPlayer.isFollowing = false;
        isBusy = true;
        enumerator = TransitionToIdle();
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        enumerator = TriggerAndWaitForTransitionToTarget("WalkingNormal");
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        Vector3 foodPos = destination.position;
        foodPos.y = 0;
        Vector3 directionToFood = (foodPos - player.position).normalized;
        float stopOffset = 0.5f; // in meters
        Vector3 stopPosition = foodPos - directionToFood * stopOffset;

        enumerator = MakeDogNavigateToTarget(stopPosition);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        Vector3 lookDir = (destination.position - player.transform.position).normalized;
        lookDir.y = 0;
        enumerator = RotateTowardDirection(lookDir);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        enumerator = TransitionToIdle();
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        enumerator = TriggerAndWaitForTransitionToTarget(actionAtTarget);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        if (agent != null) agent.enabled = true;

        Debug.Log($"Nam11 MakeDogGoEatPrivate() end. isFollowing = {dogFollowPlayer.isFollowing} isBusy = {isBusy} destination.position = {destination.position}");
    }

    private IEnumerator MakeDogFetchBallPrivate(Transform ball, System.Action onComplete = null)
    {
        IEnumerator enumerator;
        Debug.Log($"Nam11 MakeDogFetchBallPrivate() start  dogFollowPlayer.isFollowing = {dogFollowPlayer.isFollowing}");
        StopMovement();

        if (ball == null)
        {
            Debug.LogError("Nam11 ERROR1: Ball is null in MakeDogFetchBallPrivate!");
            yield break;
        }
        dogFollowPlayer.isFollowing = false;
        isBusy = true;
        // Step 1: Transition to idle
        enumerator = TransitionToIdle();
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        // Step 2: Run to the ball          
        enumerator = TriggerAndWaitForTransitionToTarget("WalkingFast");
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        //yield return StartCoroutine(MoveToTarget(ball.position, null, RUNNING_SPEED));
        Vector3 ballPos = ball.position;
        Vector3 directionToBall = (ballPos - player.position).normalized;
        float stopOffset = 0.5f; // in meters

        Vector3 stopPosition = ballPos - directionToBall * stopOffset;
        enumerator = MakeDogNavigateToTarget(stopPosition, RUNNING_SPEED, 0.0f);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        Vector3 lookDir = (ballPos - transform.position).normalized;
        lookDir.y = 0;
        enumerator = RotateTowardDirection(lookDir);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        // Step 3: Pretend to fetch
        enumerator = TransitionToIdle();
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        Debug.Log("Nam11 MakeDogFetchBallPrivate finish idle");

        enumerator = TriggerAndWaitForTransitionToTarget("EatingStart");
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        Debug.Log("Nam11 MakeDogFetchBallPrivate Wait for 1 second start");
        //yield return new WaitForSeconds(1.0f); // Simulate grabbing delay
        Debug.Log("Nam11 MakeDogFetchBallPrivate Wait for 1 second end");
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
        enumerator = TransitionToIdle();
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        enumerator = TriggerAndWaitForTransitionToTarget("WalkingFast");
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        Vector3 playerPosition = player.position + player.forward * 0.6f;
        //playerPosition.y = navMeshAgent.transform.position.y; // keep same height as dog


        playerPosition.y = 0f;
        //yield return StartCoroutine(MoveToTarget(playerPosition, null, RUNNING_SPEED));
        enumerator = MakeDogNavigateToTarget(playerPosition, RUNNING_SPEED, BALL_DROPPING_TO_PLAYER_DISTANCE, BALL_FETCHING_TIMEOUT);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        //lookDirection.y = 0; // flatten the direction
        // make the dog rotate to face player
        lookDir = player.position - navMeshAgent.transform.position;
        lookDir.y = 0;

        enumerator = RotateTowardDirection(lookDir);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;


        enumerator = TransitionToIdle();
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;
        // Step 6: Drop the ball in front of the player
        ball.SetParent(null);
        Vector3 dropPosition = navMeshAgent.transform.position + navMeshAgent.transform.forward * 0.3f;
        //Vector3 dropPosition = playerPosition;
        dropPosition.y = 0f;
        ball.position = dropPosition;

        isBusy = false;
        onComplete?.Invoke();

        Debug.Log($"Nam11 MakeDogFetchBallPrivate() end ball position = {ball.position} isBusy = {isBusy} navMeshAgent.transform.position = {navMeshAgent.transform.position}");
        //MakeDogWander();
    }

    private IEnumerator MakeDogFollowPlayerPrivate()
    {
        Debug.Log($"Nam11 MakeDogFollowPlayerPrivate() start");
        IEnumerator enumerator;
        StopMovement();
        isBusy = false;
        // Step 1: Transition to idle
        enumerator = TransitionToIdle();
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        enumerator = TriggerAndWaitForTransitionToTarget("WalkingNormal");
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        dogFollowPlayer.isFollowing = true;
        Debug.Log($"Nam11 MakeDogFollowPlayerPrivate() end dogFollowPlayer.isFollowing = {dogFollowPlayer.isFollowing}");
    }

    private IEnumerator MakeDogWanderPrivate()
    {
        Debug.Log("Nam11 MakeDogWanderPrivate() start");
        StopMovement();
        isBusy = false;
        isPlayingMath = false;
        dogAnswers.Clear();
        lastMathQuestion = "";

        IEnumerator enumerator = RandomRoamingLoop();
        randomRoamCoroutine = StartCoroutine(enumerator);
        yield return enumerator;
        Debug.Log($"Nam11 MakeDogWanderPrivate() end isBusy = {isBusy}");
    }

    public void ThrowBallAndFetch(Transform ball, Transform player)
    {
        dogActionCoroutine = StartCoroutine(ThrowBallAndFetchPrivate(ball, player));
    }

    private IEnumerator ThrowBallAndFetchPrivate(Transform ball, Transform player)
    {
        Debug.Log($"Nam11 ThrowBallAndFetchPrivate() start");
        IEnumerator enumerator;
        dogFollowPlayer.isFollowing = false;
        StopMovement();
        isBusy = true;

        enumerator = TransitionToIdle();
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        // 1. Simulate ball throw: pick a random spot in front of player
        Vector3 throwDirection = player.forward + player.right * UnityEngine.Random.Range(-0.5f, 0.5f);
        Vector3 throwTarget = player.position + throwDirection.normalized * UnityEngine.Random.Range(3f, 6f);
        throwTarget.y = 0f;

        Debug.Log("Nam11 ThrowBall target = " + throwTarget);
        ball.SetParent(null);
        ball.position = throwTarget;
        yield return StartCoroutine(WaitForSecond(WAITING_IN_MS));

        dogActionCoroutine = null;
        isBusy = false;
        Debug.Log($"Nam11 ThrowBallAndFetchPrivate() end isBusy = {isBusy}");
    }

    private IEnumerator MakeDogComfortMePrivate()
    {
        Debug.Log($"Nam11 MakeDogComfortMePrivate() start isBusy = {isBusy}");
        IEnumerator enumerator;
        StopMovement();

        dogFollowPlayer.isFollowing = false;
        isBusy = true;
        enumerator = TransitionToIdle();
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        // Walk or run to the player
        enumerator = TriggerAndWaitForTransitionToTarget("WalkingNormal");
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        //yield return StartCoroutine(MoveToTarget(target, "WigglingTail", WALKING_SPEED)); // or use RUNNING_SPEED if you want it faster
        Vector3 playerPosition = player.position;
        Vector3 direction = (playerPosition - transform.position).normalized;
        playerPosition.y = 0f;

        enumerator = MakeDogNavigateToTarget(playerPosition);
        nesteddogActionCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        if (comfortDogSound != null && comfortDogSound.clip != null)
        {
            Debug.Log("Nam11: Playing comfort sound.");
            comfortDogSound.Play();
        }
        else
        {
            Debug.LogWarning("Nam11: comfortDogSound or its clip is missing.");
        }
        yield return StartCoroutine(WaitForSecond(audioSource.clip.length));

        isBusy = false;
        Debug.Log($"Nam11 MakeDogComfortMePrivate() end isBusy = {isBusy}");
    }

    //TODO
    // private IEnumerable MakeDogHappyPrivate(Vector3 target) 
    // {

    // }

    /*     public IEnumerator StartRandomRoaming()
        {
            yield return StartCoroutine(RandomRoamingLoop());
        } */

    public void StopRandomRoaming()
    {
        if (randomRoamCoroutine != null)
        {
            StopCoroutine(randomRoamCoroutine);
            randomRoamCoroutine = null;
        }
        if (nestedrandomRoamCoroutine != null)
        {
            StopCoroutine(nestedrandomRoamCoroutine);
            nestedrandomRoamCoroutine = null;
        }
        if (nestedTransitionToIdleCoroutine != null)
        {
            StopCoroutine(nestedTransitionToIdleCoroutine);
            nestedTransitionToIdleCoroutine = null;
        }
        if (navMeshAgent != null && navMeshAgent.enabled)
        {
            navMeshAgent.ResetPath();
            navMeshAgent.velocity = Vector3.zero;
        }
        ResetTriggerAndReturnToIdle();
    }

    private IEnumerator RandomRoamingLoop()
    {
        Debug.Log("Nam11: RandomRoamingLoop start");
        if (dogFollowPlayer)
        {
            Debug.Log($"Nam11: RandomRoamingLoop() start dogFollowPlayer.isFollowing = {dogFollowPlayer.isFollowing} isBusy = {isBusy}");
        }

        var room = MRUK.Instance?.GetCurrentRoom();
        if (!room)
        {
            Debug.Log("Nam11: RandomRoamingLoop Room is not available");
            yield return null;
        }
        yield return StartCoroutine(WaitForNavMeshReady());
        Debug.Log($"Nam11: RandomRoamingLoop Room wait end. dog position = {navMeshAgent.transform.position} dogFollowPlayer = {dogFollowPlayer}");
        while (dogFollowPlayer != null)
        {
            //Debug.Log($"Nam11: RandomRoamingLoop() start dog position = {navMeshAgent.transform.position} isBusy = {isBusy}");
            if (dogFollowPlayer.isFollowing || isBusy)
            {
                yield return null;
                continue;
            }

            /*             Vector3 randomPosition = new Vector3(
                            UnityEngine.Random.Range(minBounds.x, maxBounds.x),
                            transform.position.y,
                            UnityEngine.Random.Range(minBounds.z, maxBounds.z)
                        ); */
            //navMeshAgent.ResetPath();
            IEnumerator enumerator = TransitionToIdle();
            nestedrandomRoamCoroutine = StartCoroutine(enumerator);
            yield return enumerator;

            //navMeshAgent.stoppingDistance = DISTANCE_TO_TARGET - 0.1f;

            Vector3 randomPosition = GuardianWanderUtil.FindReachableDestinationInGuardianArc(player, navMeshAgent.transform);
            enumerator = MakeDogNavigateToTarget(randomPosition, WANDERING_SPEED, DISTANCE_TO_TARGET);
            nestedrandomRoamCoroutine = StartCoroutine(enumerator);
            yield return enumerator;
            yield return StartCoroutine(WaitForSecond(UnityEngine.Random.Range(2f, 5f)));
            //yield return StartCoroutine(TransitionToIdle());
            Debug.Log($"Nam11: RandomRoamingLoop() end dog position = {navMeshAgent.transform.position} isBusy = {isBusy}");
        }
        Debug.Log($"Nam11: RandomRoamingLoop() function end isBusy = {isBusy} dog position = {navMeshAgent.transform.position}");
    }

    private IEnumerator MakeDogDoMathPrivate(string mathExpression)
    {
        Debug.Log($"Nam11 MakeDogDoMathPrivate() start mathExpression: {mathExpression} isPlayingMath = {isPlayingMath} isBusy = {isBusy}");
        if (isPlayingMath == true)
        {
            int answer = predefinedMathExpressions[mathExpression];

            IEnumerator enumerator = TransitionToIdle();
            nestedrandomRoamCoroutine = StartCoroutine(enumerator);
            yield return enumerator;

            enumerator = TriggerAndWaitForTransitionToTarget("AngryStart");
            nestedrandomRoamCoroutine = StartCoroutine(enumerator);
            yield return enumerator;

            enumerator = BarkNTimesCoroutine(answer);
            nestedrandomRoamCoroutine = StartCoroutine(enumerator);
            yield return enumerator;

            enumerator = TransitionToIdle();
            nestedrandomRoamCoroutine = StartCoroutine(enumerator);
            yield return enumerator;

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
            if (singleBarkSoundMath != null && singleBarkSoundMath.clip != null)
            {
                singleBarkSoundMath.PlayOneShot(singleBarkSoundMath.clip);
                Debug.Log($"Nam11: Barking {i + 1}/{count} singleBarkSoundMath.clip.length = {singleBarkSoundMath.clip.length}");

                float totalDelay = singleBarkSoundMath.clip.length + 0.7f;
                float startTime = Time.realtimeSinceStartup;

                while (Time.realtimeSinceStartup - startTime < totalDelay)
                {
                    yield return null; // wait until delay is met in real-world time
                }
            }
            else
            {
                Debug.LogWarning("Nam11: Bark audio source or clip is null.");
                yield return new WaitForSeconds(1.5f);
            }
        }
        Debug.Log("Nam11 BarkNTimesCoroutine() end");
    }



    private IEnumerator DogRespondToSetupMathGame()
    {
        Debug.Log("Nam11 DogRespondToSetupMathGame() start");

        IEnumerator enumerator = TransitionToIdle();
        nestedrandomRoamCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        enumerator = TriggerAndWaitForTransitionToTarget("AngryStart");
        nestedrandomRoamCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        enumerator = BarkNTimesCoroutine(1);
        nestedrandomRoamCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        enumerator = TransitionToIdle();
        nestedrandomRoamCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

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

                    IEnumerator enumerator = TransitionToIdle();
                    nestedrandomRoamCoroutine = StartCoroutine(enumerator);
                    yield return enumerator;

                    enumerator = TriggerAndWaitForTransitionToTarget("WigglingTail");
                    nestedrandomRoamCoroutine = StartCoroutine(enumerator);
                    yield return enumerator;

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
            IEnumerator enumerator = TransitionToIdle();
            nestedrandomRoamCoroutine = StartCoroutine(enumerator);
            yield return enumerator;
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

            IEnumerator enumerator = TransitionToIdle();
            nestedrandomRoamCoroutine = StartCoroutine(enumerator);
            yield return enumerator;

            enumerator = TriggerAndWaitForTransitionToTarget("AngryStart");
            nestedrandomRoamCoroutine = StartCoroutine(enumerator);
            yield return enumerator;

            enumerator = BarkNTimesCoroutine(newAnswer);
            nestedrandomRoamCoroutine = StartCoroutine(enumerator);
            yield return enumerator;

            enumerator = TransitionToIdle();
            nestedrandomRoamCoroutine = StartCoroutine(enumerator);
            yield return enumerator;

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
            while (number != answer && dogAnswers.Contains(number))
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

        IEnumerator enumerator = TransitionToIdle();
        nestedrandomRoamCoroutine = StartCoroutine(enumerator);
        yield return enumerator;

        Debug.Log($"Nam11 DogEndMathSectionPrivate() end isPlayingMath = {isPlayingMath} isBusy = {isBusy}");
    }

    private IEnumerator MathGameSetupPrivate(string setupString)
    {
        Debug.Log($"Nam11: MathGameSetup() start. setupString = {setupString} isBusy = {isBusy} isPlayingMath = {isPlayingMath}");
        IEnumerator enumerator;
        if (setupString == "let play math game")
        {
            enumerator = MakeDogComeHerePrivate();
            nestedrandomRoamCoroutine = StartCoroutine(enumerator);
            yield return enumerator;

            isBusy = true;
        }
        else if (setupString == "are you ready" && isBusy == true)
        {
            enumerator = DogRespondToSetupMathGame();
            nestedrandomRoamCoroutine = StartCoroutine(enumerator);
            yield return enumerator;

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

    private bool SetWanderingLoation(NavMeshAgent dog)
    {
        float wanderRadius = 2.0f;  // Max distance from origin
        float maxSampleDistance = 2.0f; // How far to search for NavMesh around candidate point
        int maxAttempts = 10;

        for (int attempts = 0; attempts < maxAttempts; attempts++)
        {
            Vector2 random2D = UnityEngine.Random.insideUnitCircle * wanderRadius;
            Vector3 candidate = dog.transform.position + new Vector3(random2D.x, 0, random2D.y);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, maxSampleDistance, NavMesh.AllAreas))
            {
                float dist = Vector3.Distance(dog.transform.position, hit.position);
                if (dist >= 1.0f)
                {
                    NavMeshPath path = new NavMeshPath();
                    if (dog.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                    {
                        dog.SetDestination(hit.position);
                        return true;
                    }
                    else
                    {
                        Debug.Log("Nam11 Sampled point is on NavMesh but not reachable from current position.");
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator WaitForNavMeshReady()
    {
        // Optional: Wait until NavMeshSurface is present
        yield return new WaitForSeconds(2.0f);
        yield return new WaitUntil(() =>
            FindObjectOfType<NavMeshSurface>() != null &&
            NavMesh.CalculateTriangulation().vertices.Length > 0);

        Debug.Log("Nam11: NavMesh is ready, enabling dog wander.");

    }

    private IEnumerator MakeDogNavigateToTarget(Vector3 targetLocation, float agentSpeed = WALKING_SPEED, float stoppingDistance = 0.3f, float navigationTimeoutTime = 10.0f)
    {
        Debug.Log($"Nam11: MakeDogNavigateToTarget start targetLocation = {targetLocation} dog position = {navMeshAgent.transform.position} stoppingDistance = {stoppingDistance}");

        navMeshAgent.enabled = true;
        navMeshAgent.stoppingDistance = stoppingDistance;
        navMeshAgent.speed = agentSpeed;
        navMeshAgent.autoBraking = true;
        navMeshAgent.SetDestination(targetLocation);
        animator.SetTrigger("WalkingNormal");
        while (navMeshAgent.pathPending)
        {
            yield return null;
        }

        float travelTime = 0;
        //float distanceToTarget = Vector3.Distance(transform.position, randomPosition);
        Debug.Log($"Nam11: navMeshAgent.remainingDistance = {navMeshAgent.remainingDistance} targetLocation = {targetLocation} dog position = {navMeshAgent.transform.position} navMeshAgent.stoppingDistance = {navMeshAgent.stoppingDistance}");
        while (navMeshAgent.remainingDistance > DISTANCE_TO_TARGET && travelTime < navigationTimeoutTime)
        {
            //if (dogFollowPlayer.isFollowing || isBusy) yield break;
            yield return null;
            float distanceToTarget = Vector3.Distance(transform.position, targetLocation);
            travelTime += Time.deltaTime;
            //Debug.Log($"Nam11: navMeshAgent.transform.position = {navMeshAgent.transform.position} distanceToTarget = {navMeshAgent.remainingDistance} targetLocation = {targetLocation} navMeshAgent.stoppingDistance = {navMeshAgent.stoppingDistance}");
        }
        if (travelTime > navigationTimeoutTime)
        {

            Debug.Log($"Nam11: travel timeout navMeshAgent = {navMeshAgent.transform.position} distanceToTarget = {navMeshAgent.remainingDistance}");
        }
        navMeshAgent.ResetPath();
        ResetTriggerAndReturnToIdle();
        Debug.Log($"Nam11: MakeDogNavigateToTarget end travelTime = {travelTime} navigationTimeoutTime = {navigationTimeoutTime} dog position = {navMeshAgent.transform.position} player position = {player.transform.position} dogFollowPlayer.isFollowing = {dogFollowPlayer.isFollowing} isBusy = {isBusy}");
    }

    private IEnumerator RotateTowardDirection(Vector3 direction)
    {
        Debug.Log($"Nam11: RotateTowardDirection start, direction = {direction}");

        if (direction == Vector3.zero)
        {
            Debug.LogWarning("Nam11 RotateTowardDirection: Zero direction vector.");
            yield break;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float tolerance = 1f;
        float rotationSpeed = 540f;
        float timeout = 2.5f; // seconds
        float timer = 0f;

        navMeshAgent.updateRotation = false;

        while (Quaternion.Angle(transform.rotation, targetRotation) > tolerance)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            timer += Time.deltaTime;
            if (timer > timeout)
            {
                Debug.LogWarning("Nam11 RotateTowardDirection: Timed out.");
                break;
            }

            yield return null;
        }

        navMeshAgent.updateRotation = true;
        Debug.Log("Nam11: RotateTowardDirection end");
    }

    private void ResetTriggerAndReturnToIdle()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log($"Nam11: ResetTriggerAndReturnToIdle start stateInfo.shortNameHash = {stateInfo.shortNameHash}");

        animator.Play("Idle", 0, 0f);
        animator.Update(0f);
        animator.ResetTrigger("WalkingNormal");
        animator.ResetTrigger("SittingStart");
        animator.ResetTrigger("Breathing");
        animator.ResetTrigger("AngryStart");
        animator.ResetTrigger("WigglingTail");
        animator.ResetTrigger("EatingStart");
        animator.ResetTrigger("RunningToIdle");
        animator.ResetTrigger("WalkingFastToIdle");
        animator.ResetTrigger("SittingCycleToIdle");
        animator.ResetTrigger("BreathingToIdle");
        animator.ResetTrigger("WalkingFast");
        animator.ResetTrigger("AngryCycleToIdle");
        animator.ResetTrigger("WigglingTailToIdle");
        animator.ResetTrigger("EatingCycleToIdle");
        animator.ResetTrigger("WalkingNormalToIdle");
        animator.ResetTrigger("EatingCycle");
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log($"Nam11: ResetTriggerAndReturnToIdle end stateInfo.shortNameHash = {stateInfo.shortNameHash}");
    }

    private IEnumerator WaitForSecond(float numSec)
    {
        float startTime = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup - startTime < numSec)
        {
            yield return null; // wait until delay is met in real-world time
        }   
    }
    

}
