using System;
using System.Collections.Generic;
using System.Linq;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class HandSynchronizer : MonoBehaviour
{
    public ROSConnection connection;

    public string topicName;
    
    public OVRHand hand;

    public OVRSkeleton handSkeleton;
    
    public Transform headsetTransform;
    
    public int maxFrequency = 120;

    public string frameId;

    public double timeOffsetSecond = -1.0;

    public Vector3 positionOffset = new Vector3(0.0f, 0.0f, 0.0f);

    public float rotationScale = 1.0f;

    private Vector3? _initialHeadsetOrientation = null;
    
    void Start()
    {
        connection.RegisterPublisher<PoseStampedMsg>(topicName);
        _lastUpdateTime = DateTime.Now;
    }
    
    void SendHandPose(Quaternion orientation, Vector3 position)
    {
        connection.Publish(topicName, new PoseStampedMsg()
        {
            header = new HeaderMsg
            {
                seq = 0,
                frame_id = frameId,
                stamp = new TimeMsg
                {
                    sec = (uint)(DateTimeOffset.Now.ToUnixTimeSeconds() + timeOffsetSecond),
                    nanosec = 0
                }
            },
            pose = new PoseMsg
            {
                orientation = new QuaternionMsg(orientation.x, orientation.y, orientation.z, orientation.w),
                position = new PointMsg(position.x, position.y, position.z)
            }
        });
    }

    private DateTime _lastUpdateTime;
    
    private static readonly OVRSkeleton.BoneId[] PalmPositionKeyBoneIds = new[]
    {
        // First two of them are used for calculating palm normal.
        OVRSkeleton.BoneId.Hand_Index1,
        OVRSkeleton.BoneId.Hand_Pinky0,

        OVRSkeleton.BoneId.Hand_Middle1,
        OVRSkeleton.BoneId.Hand_Ring1,
        OVRSkeleton.BoneId.Hand_Pinky1,
        OVRSkeleton.BoneId.Hand_Thumb0,
    };

    private static readonly OVRSkeleton.BoneId[] PalmNormalKeyBoneIds = new[]
    {
        OVRSkeleton.BoneId.Hand_Index1,
        OVRSkeleton.BoneId.Hand_Pinky0
    };
    
    private static (Vector3 Position, Vector3 Normal) GetPositionAndNormal(IList<OVRBone> bones)
    {
        var center = PalmPositionKeyBoneIds.Select(id => bones[(int)id])
            .Aggregate(Vector3.zero, (current, bone) => current + bone.Transform.position);
        
        center /= PalmPositionKeyBoneIds.Length;
        
        var edge0 = (bones[(int)PalmNormalKeyBoneIds[0]]).Transform.position - center;
        var edge1 = (bones[(int)PalmNormalKeyBoneIds[1]]).Transform.position - center;

        var normal = Vector3.Cross(edge0, edge1).normalized;

        return (center, normal);
    }
    
    void Update()
    {
        _initialHeadsetOrientation ??= headsetTransform.eulerAngles;
        
        if (DateTime.Now - _lastUpdateTime < TimeSpan.FromSeconds(1.0 / maxFrequency))
            return;
        _lastUpdateTime = DateTime.Now;
        
        if (!hand.IsDataHighConfidence)
        {
            Debug.Log($"[CyberDiver] {gameObject.name} Hand is not active.");
            return;
        }
        
        var (position, normal) = GetPositionAndNormal(handSkeleton.Bones);

        normal = -normal;
        
        // Todo: Hand position calculation is still not natural. Need adjustment accroding to the hand position in ROS.
        
        position = (position - headsetTransform.position).ToRosCoordinate();

        position += positionOffset;
        
        var rotationAngles = 
            Quaternion.LookRotation(normal).eulerAngles - _initialHeadsetOrientation.Value;
        rotationAngles *= rotationScale;

        var rotation = Quaternion.Euler(rotationAngles).ToRosCoordinate();
        
        SendHandPose(rotation, position);
        
        Debug.Log($"[CyberDiver] {gameObject.name} " +
                  $"Hand command is sent, position: {position}, rotation: {rotation} .");
    }
}
