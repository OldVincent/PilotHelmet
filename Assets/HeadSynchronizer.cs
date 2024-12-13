using System;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class HeadSynchronizer : MonoBehaviour
{
    public ROSConnection connection;

    public string topicName;

    public Transform headsetTransform;

    public int maxFrequency = 120;

    public double rotationScale = 1.0;
    
    private Vector3? _initialRotationAngles;
    
    void Start()
    {
        connection.RegisterPublisher<PoseStampedMsg>(topicName);
        _lastUpdateTime = DateTime.Now;
    }
    
    void SendHeadRotation(double pan, double tilt)
    {
        
    }

    private DateTime _lastUpdateTime;
    
    // Update is called once per frame
    void Update()
    {
        _initialRotationAngles ??= headsetTransform.eulerAngles;
        
        if ((DateTime.Now - _lastUpdateTime) < TimeSpan.FromSeconds(1.0 / maxFrequency))
            return;
        _lastUpdateTime = DateTime.Now;
        
        connection.Publish(topicName, new PoseStampedMsg()
        {
            header = new HeaderMsg
            {
                seq = 0,
                frame_id = "",
                stamp = new TimeMsg
                {
                    sec = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    nanosec = 0
                }
            },
            pose = new PoseMsg()
            {
                orientation = new QuaternionMsg(
                    headsetTransform.rotation.x, 
                    headsetTransform.rotation.y, 
                    headsetTransform.rotation.z, 
                    headsetTransform.rotation.w),
                position = new PointMsg(
                    headsetTransform.position.x,
                    headsetTransform.position.y,
                    headsetTransform.position.z)
            }
        });
        
        Debug.Log($"Sent head pose.");
    }
}
