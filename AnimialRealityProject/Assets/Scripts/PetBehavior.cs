using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetBehavior: MonoBehaviour
{
    private Animator animator;
    private int currentID = -1; //current State

    //Pet Animation Enum
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

    
    //play animation
   
    public void Play(PetAnim anim)
    {
        int id = (int)anim;

        if (currentID == id) return; 
        animator.SetInteger("AnimationID", -1); //reset animation
        animator.SetInteger("AnimationID", id);

        currentID = id;
        Debug.Log($"Playing animation: {anim} (ID: {id})");
    }

    public void Breath() => Play(PetAnim.Breath);
    public void WagTail() => Play(PetAnim.Wag);
    public void Walk() => Play(PetAnim.Walk);
    public void Run() => Play(PetAnim.Run);
    public void Eat() => Play(PetAnim.Eat);
    public void Angry() => Play(PetAnim.Angry);
    public void Sit() => Play(PetAnim.Sit);

}
