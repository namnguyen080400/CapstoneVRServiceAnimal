using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class DogMovement : MonoBehaviour
{
    public float speed = 3f;
    public float rotationSpeed = 500f;
    private Animator animator;

     private static KeywordRecognizer keywordRecognizer;
    private static Dictionary<string, System.Action> commands = new Dictionary<string, System.Action>();
    
    private static bool voiceInitialized = false;

    private static DictationRecognizer dictationRecognizer;

    public Vector3 minBounds = new Vector3(-10f, 0f, -10f);
    public Vector3 maxBounds = new Vector3(10f, 0f, 10f);


    void Start()
    {
        animator = GetComponent<Animator>();
       /*  if (!voiceInitialized) 
        {
            animator = GetComponent<Animator>(); // Get the Animator component
            DefineVoiceCommand();
            Debug.Log("dog start");
            if (keywordRecognizer == null) {
                // Initialize keyword recognizer
                keywordRecognizer = new KeywordRecognizer(commands.Keys.ToArray());
                //keywordRecognizer.OnPhraseRecognized += OnCommandRecognized;
                keywordRecognizer.OnPhraseRecognized += (args) =>
                {
                    Debug.Log("Heard: " + args.text);
                };
                keywordRecognizer.Start();
            }


    
            Debug.Log("Commands: " + string.Join(", ", commands.Keys));
            Debug.Log("Recognizer status: " + (keywordRecognizer.IsRunning ? "Running" : "Not Running"));
            
            foreach (var device in Microphone.devices)
            {
                Debug.Log("Mic device: " + device);
            }

            voiceInitialized = true;
        } */

            dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.DictationResult += (text, confidence) =>
        {
            Debug.Log("🎤 Dictation heard: " + text);
        };
        dictationRecognizer.DictationHypothesis += (text) =>
        {
            Debug.Log("...listening: " + text);
        };
        dictationRecognizer.DictationComplete += (completionCause) =>
        {
            Debug.Log("Dictation completed: " + completionCause);
        };
        dictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogError("Dictation error: " + error);
        };

        dictationRecognizer.Start();
        Debug.Log("🎧 DictationRecognizer started...");


    }

    public void OnCommandRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log("Command Recognized: " + args.text);
        commands[args.text].Invoke();
    }


    public void Update()
    {
        /* float moveX = Input.GetAxis("Horizontal"); // A (-1) / D (+1)
        float moveZ = Input.GetAxis("Vertical");   // W (+1) / S (-1)
        Vector3 moveDirection = new Vector3((-1) * moveX, 0, (-1) * moveZ).normalized;
        Move(moveDirection); */
    }

    public void Move(Vector3 moveDirection)
    {
        if (moveDirection.magnitude > 0)
        {
            // Calculate new position
            Vector3 newPosition = transform.position + moveDirection * speed * Time.deltaTime;

            // Clamp position within boundaries
            newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
            newPosition.z = Mathf.Clamp(newPosition.z, minBounds.z, maxBounds.z);

            // Apply movement
            transform.position = newPosition;

            // Rotate towards movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Play walking animation
            //animator.SetBool("isWalking", true);
        }
        else
        {
            // Stop walking animation
            //animator.SetBool("isWalking", false);
        }
    }

    public void MoveInDirection(Vector3 direction)
    {
        if (animator != null)
        {
            animator.SetTrigger("WalkingNormal"); // Play walking animation
        }

        StartCoroutine(MoveOverTime(direction));
    }

    IEnumerator MoveOverTime(Vector3 direction)
    {
        float duration = 1f; // Move for 1 seconds
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            // Rotate to face movement direction
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        TransitionToIdle();
    }


    public void MakeDogRun() 
    {
        if (animator != null)
        {
            animator.SetTrigger("Running");
        }
    }

    public void MakeDogTransitionRunToIdle() 
    {
        if (animator != null)
        {
            animator.SetTrigger("RunningToIdle");
        }
    }

    public void MakeDogWalk() 
    {
        if (animator != null)
        {
            animator.SetTrigger("WalkingNormal");
        }
    }

    public void MakeDogTransitionWalkingNormalToIdle() 
    {
        Debug.Log("Transition from walking normal to idle");
        if (animator != null)
        {
            animator.SetTrigger("WalkingNormalToIdle");
        }
    } 

    public void MakeDogSit()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Idle")) 
        {
            Debug.Log("dog in Idle state");
        }
        else 
        {
            Debug.Log("dog not in Idle state");
        }
        Debug.Log("Execute MakeDogSit");
        if (animator != null)
        {        
            animator.SetTrigger("SittingStart");
        }
    }

    public void MakeDogTransitionSitToIdle()
    {
        if (animator != null)
        {
            animator.SetTrigger("SittingCycleToIdle");
        }
    }

    public void MakeDogBreathing()
    {
        if (animator != null)
        {
            animator.SetTrigger("Breathing");
        }
    }

    public void MakeDogTransitionBreathingToIdle()
    {
        if (animator != null)
        {
            animator.SetTrigger("BreathingToIdle");
        }
    }

    public void MakeDogAngry()
    {
        if (animator != null)
        {
            animator.SetTrigger("AngryStart");
        }
    }

    public void MakeDogTransitionAngryToIdle()
    {
        if (animator != null)
        {
            animator.SetTrigger("AngryCycleToIdle");
        }
    }

    public void MakeDogWagTail()
    {
        if (animator != null)
        {
            animator.SetTrigger("WigglingTail");
        }
    }

    public void MakeDogTransitionWagTailToIdle()
    {
        if (animator != null)
        {
            animator.SetTrigger("WigglingTailToIdle");
        }
    }

    public void MakeDogEating()
    {
        if (animator != null)
        {
            animator.SetTrigger("EatingStart");
        }
    }

    public void MakeDogTransitionEatingToIdle()
    {
        if (animator != null)
        {
            animator.SetTrigger("WigglingTailToIdle");
        }
    }


    public void TransitionToIdle() 
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Running"))
        {
            MakeDogTransitionRunToIdle();
        }
        else if (stateInfo.IsName("WalkingNormal"))
        {
            MakeDogTransitionWalkingNormalToIdle();
        }
        else if (stateInfo.IsName("SittingCycle"))
        {
            MakeDogTransitionSitToIdle();
        }  
        else if (stateInfo.IsName("Breathing"))
        {
            MakeDogTransitionBreathingToIdle();
        }         
        else if (stateInfo.IsName("AngryCycle"))
        {
            MakeDogTransitionAngryToIdle();
        }   
        else if (stateInfo.IsName("WigglingTail"))
        {
            MakeDogTransitionWagTailToIdle();
        }
        else if (stateInfo.IsName("EatingCycle"))
        {
           MakeDogTransitionEatingToIdle();
        }          
    }

    public void DefineVoiceCommand() 
    {
        Debug.Log("DefineVoiceCommand call");
        // Define voice commands
        // commands.Add("sit", MakeDogSit);
        // commands.Add("wag tail", MakeDogWagTail);
        // commands.Add("stop", TransitionToIdle);
        commands.Add("sit", () => Debug.Log("Voice: Sit command received"));
        commands.Add("wag tail", () => Debug.Log("Voice: Wag tail command received"));
        commands.Add("stop", () => Debug.Log("Voice: Stop command received"));

    }
    
}
