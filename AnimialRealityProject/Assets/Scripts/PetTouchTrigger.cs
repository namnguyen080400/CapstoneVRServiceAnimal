using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;

public class PetTouchTrigger : MonoBehaviour
{
    [Header("References")]
    public PetBehavior petBehavior;    
    public Collider dogCollider;       

    [Header("Touch Settings")]
    public float cooldown = 2f;        

    private XRHandSubsystem handSubsystem;
    private bool recentlyTriggered = false;
    private float timer = 0f;

    void Start()
    {
 
        var subsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetInstances(subsystems);
        if (subsystems.Count > 0)
            handSubsystem = subsystems[0];
    }

    void Update()
    {
        if (handSubsystem == null || petBehavior == null || dogCollider == null)
            return;

        if (recentlyTriggered)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
                recentlyTriggered = false;
        }

        XRHand rightHand = handSubsystem.rightHand;
        if (!rightHand.isTracked) return;

        XRHandJoint joint = rightHand.GetJoint(XRHandJointID.IndexTip);

        if (joint.TryGetPose(out Pose pose))
        {
            Vector3 fingerPos = pose.position;

            if (dogCollider.bounds.Contains(fingerPos) && !recentlyTriggered)
            {
                Debug.Log("Dog touched");
                petBehavior.WagTail();   

                recentlyTriggered = true;
                timer = cooldown;
            }
        }
    }
}
