using System.Collections;
using UnityEngine;

public class PetBehavior : MonoBehaviour
{
    private Animator animator;
    private int currentID = -1;
    private bool animationLocked = false;

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
    }

  
    //basic animation play method

    public void Play(PetAnim anim)
    {
        if (animationLocked) return; // don't change animation if locked

        int id = (int)anim;

        if (currentID == id) return;

        animator.SetInteger("AnimationID", -1); // Reset first
        animator.SetInteger("AnimationID", id);
        currentID = id;

        Debug.Log($"[PetBehavior] Playing animation: {anim} (ID: {id})");
    }


    //play animation and lock it for a duration to prevent override
    public void PlayLocked(PetAnim anim, float lockDuration)
    {
        int id = (int)anim;

        animator.SetInteger("AnimationID", -1); // Reset first
        animator.SetInteger("AnimationID", id);
        currentID = id;

        Debug.Log($"[PetBehavior] Playing LOCKED animation: {anim} (ID: {id}) for {lockDuration}s");
        animationLocked = true;
        StartCoroutine(UnlockAfter(lockDuration));
    }

    private IEnumerator UnlockAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        animationLocked = false;
        Debug.Log("[PetBehavior] Animation lock released");
    }
    public bool IsAnimationLocked => animationLocked;

    //shortcut methods

    public void Breath() => Play(PetAnim.Breath);
    public void Walk() => Play(PetAnim.Walk);
    public void Run() => Play(PetAnim.Run);

    public void WagTail() => PlayLocked(PetAnim.Wag, 2f);
    public void Eat() => PlayLocked(PetAnim.Eat, 3f);
    public void Angry() => PlayLocked(PetAnim.Angry, 2.5f);
    public void Sit() => PlayLocked(PetAnim.Sit, 4f);
}
