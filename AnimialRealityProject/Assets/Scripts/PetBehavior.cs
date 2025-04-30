using System.Collections;
using UnityEngine;

public class PetBehavior : MonoBehaviour
{
    private Animator animator;
    private int currentID = -1;
    private bool animationLocked = false;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip barkClip;
    public AudioClip whinningClip;

    [Header("Movement Settings (optional)")]
    public float moveSpeed = 1f;
    public float rotateSpeed = 180f;

    private float lastActionTime = 0f;
    private float cooldown = 1.5f;

    [Header("Follow Settings")]
    //pet follow settings
    public Transform followHand; // Assign left or right hand transform
    public float followSpeed = 1f;
    public float followDistance = 0.5f;
    public bool followHandMode = false;


    public enum PetAnim
    {
        Breath = 0,
        Wag = 1,
        Walk = 2,
        Run = 4,
        Eat = 5,
        Angry = 6,
        Sit = 7
    }

    void Start()
    {
        animator = GetComponent<Animator>();

        if (followHand == null && Camera.main != null)
        {
            // If no follow hand is assigned, set the pet to follow the camera
            Vector3 startPosition = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;
            startPosition.y = transform.position.y; // keep the pet at the same height
            transform.position = startPosition;

            Vector3 lookDir = Camera.main.transform.position - transform.position;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }
    }

    void Update()
    {
        if (followHandMode && followHand != null && !animationLocked)
        {
            Vector3 targetPosition = followHand.position + followHand.forward * followDistance;
            targetPosition.y = transform.position.y;

            float distance = Vector3.Distance(transform.position, targetPosition);

            if (distance > 0.1f)
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;
                transform.position += moveDir * followSpeed * Time.deltaTime;

                if (moveDir != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(moveDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotateSpeed);
                }

                Run();
            }
            else
            {
                Breath();
            }
        }
    }



    private void PlayBark()
    {
        if (audioSource && barkClip)
        {
            audioSource.PlayOneShot(barkClip);
        }
    }

    private void PlayWhinning()
    {
        if (audioSource && whinningClip)
        {
            audioSource.PlayOneShot(whinningClip);
        }
    }

    public void Play(PetAnim anim)
    {
        if (animationLocked || Time.time - lastActionTime < cooldown) return;

        int id = (int)anim;
        if (currentID == id) return;

        animator.SetInteger("AnimationID", -1); // reset
        animator.SetInteger("AnimationID", id);
        currentID = id;
        lastActionTime = Time.time;

        Debug.Log($"[PetBehavior] Playing animation: {anim}");
    }

    public void PlayLocked(PetAnim anim, float lockDuration)
    {
        if (animationLocked || Time.time - lastActionTime < cooldown) return;

        int id = (int)anim;
        if (currentID == id) return; // prevent re-locking same animation

        animator.SetInteger("AnimationID", -1);
        animator.SetInteger("AnimationID", id);
        currentID = id;
        animationLocked = true;
        lastActionTime = Time.time;

        Debug.Log($"[PetBehavior] Playing LOCKED animation: {anim} for {lockDuration}s");

        StartCoroutine(UnlockAfter(lockDuration));
    }

    private IEnumerator UnlockAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        animationLocked = false;
        Debug.Log("[PetBehavior] Animation lock released");
    }

    public bool IsAnimationLocked => animationLocked;
    public int CurrentAnimationID => currentID;

    // Public API - callable from voice/gesture/etc.
    public void Breath() => Play(PetAnim.Breath);
    public void Walk() => Play(PetAnim.Walk);
    public void Run() => Play(PetAnim.Run);

    public void WagTail()
    {
        if (animationLocked || Time.time - lastActionTime < cooldown) return;

        PlayWhinning();
        PlayLocked(PetAnim.Wag, 2f);
    }

    public void Eat()
    {
        if (animationLocked || Time.time - lastActionTime < cooldown) return;
        PlayLocked(PetAnim.Eat, 3f);
    }

    public void Angry()
    {
        if (animationLocked || Time.time - lastActionTime < cooldown) return;

        PlayBark();
        PlayLocked(PetAnim.Angry, 1.2f);
    }

    public void Sit()
    {
        if (animationLocked || Time.time - lastActionTime < cooldown) return;
        PlayLocked(PetAnim.Sit, 4f); // Sit
    }

    public void Spin(float radius = 2.0f, float duration = 2.5f)
    {
        if (!animationLocked && Time.time - lastActionTime > cooldown)
        {
            StartCoroutine(SpinRoutine(radius, duration));
        }
    }

    private IEnumerator SpinRoutine(float radius, float duration)
    {
        animationLocked = true;
        lastActionTime = Time.time;

        Run();

        Vector3 center = transform.position;
        float angle = 0f;
        float anglePerSecond = 360f / duration; 

        while (angle < 360f)
        {
            float deltaAngle = anglePerSecond * Time.deltaTime;
            angle += deltaAngle;

            float rad = angle * Mathf.Deg2Rad;
            Vector3 targetPos = center + new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad)) * radius;

            Vector3 moveDirection = (targetPos - transform.position).normalized;

            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            if (moveDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(moveDirection);
            }

            center += moveDirection * moveSpeed * Time.deltaTime * 0.3f;

            yield return null;
        }

        Breath();
        animationLocked = false;
    }

    public void FollowPlayer()
    {
        followHandMode = true;
        Walk();
        Debug.Log("Pet is now following you.");
    }

    public void StopFollowPlayer()
    {
        followHandMode = false;
        Breath();
        Debug.Log("Pet stopped following you.");
    }

}
