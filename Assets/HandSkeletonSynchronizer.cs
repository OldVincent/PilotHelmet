using System;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Geometry;
using RosMessageTypes.PilotHelmet;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class HandSkeletonSynchronizer : MonoBehaviour
{
    public ROSConnection connection;

    public string topicName;
    
    public OVRHand hand;

    public OVRSkeleton handSkeleton;
    
    public Transform headsetTransform;
    
    public int maxFrequency = 120;
    
    void Start()
    {
        connection.RegisterPublisher<HandSkeletonMsg>(topicName);
        _lastUpdateTime = DateTime.Now;
    }

    private DateTime _lastUpdateTime;
    
    void Update()
    {
        if (DateTime.Now - _lastUpdateTime < TimeSpan.FromSeconds(1.0 / maxFrequency))
            return;
        _lastUpdateTime = DateTime.Now;
        
        if (!hand.IsDataHighConfidence)
        {
            Debug.Log($"[CyberDiver] {gameObject.name} Hand is not active.");
            return;
        }
        
        connection.Publish(topicName, new HandSkeletonMsg()
        {
            timestamp = new TimeMsg()
            {
                sec = (uint)(DateTimeOffset.Now.ToUnixTimeSeconds()),
                nanosec = 0
            },
            head = headsetTransform.ToTransformMsg(),
            wrist_root = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_WristRoot].Transform.ToTransformMsg(),
            forearm_stub = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_ForearmStub].Transform.ToTransformMsg(),
            thumb0 = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_Thumb0].Transform.ToTransformMsg(),
            thumb1 = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_Thumb1].Transform.ToTransformMsg(),
            thumb2 = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_Thumb2].Transform.ToTransformMsg(),
            thumb3 = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_Thumb3].Transform.ToTransformMsg(),
            thumb_tip = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_ThumbTip].Transform.ToTransformMsg(),
            index1 = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_Index1].Transform.ToTransformMsg(),
            index2 = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_Index2].Transform.ToTransformMsg(),
            index3 = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_Index3].Transform.ToTransformMsg(),
            index_tip = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_IndexTip].Transform.ToTransformMsg(),
            middle1 = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_Middle1].Transform.ToTransformMsg(),
            middle2 = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_Middle2].Transform.ToTransformMsg(),
            middle3 = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_Middle3].Transform.ToTransformMsg(),
            middle_tip = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_MiddleTip].Transform.ToTransformMsg(),
            ring1 = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_Ring1].Transform.ToTransformMsg(),
            ring2 = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_Ring2].Transform.ToTransformMsg(),
            ring3 = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_Ring3].Transform.ToTransformMsg(),
            ring_tip = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_RingTip].Transform.ToTransformMsg(),
            pinky0 = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_Pinky0].Transform.ToTransformMsg(),
            pinky1 = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_Pinky1].Transform.ToTransformMsg(),
            pinky2 = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_Pinky2].Transform.ToTransformMsg(),
            pinky3 = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_Pinky3].Transform.ToTransformMsg(),
            pinky_tip = handSkeleton.Bones[(int) OVRSkeleton.BoneId.Hand_PinkyTip].Transform.ToTransformMsg(),
        });
        
        Debug.Log($"[CyberDiver] {gameObject.name} " +
                  $"Hand pose is sent.");
    }
}
