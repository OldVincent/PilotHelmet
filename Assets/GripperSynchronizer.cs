using System;
using RosMessageTypes.Actionlib;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Control;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class GripperSynchronizer : MonoBehaviour
{
    public ROSConnection connection;

    public string topicName;

    public double gripperOpenStrength = 20.0;

    public double gripperCloseStrength = 60.0;
    
    void Start()
    {
        connection.RegisterPublisher<GripperCommandActionGoal>(topicName);
    }

    public void OpenGripper()
        => SendGripperCommand(0.09, gripperOpenStrength);

    public void CloseGripper()
        => SendGripperCommand(0.0, gripperCloseStrength);
    
    
    public void SendGripperCommand(double position, double strength)
    {
        Debug.Log($"{gameObject.name} Command sent: {position}, {strength}.");
        
        var timestamp = new TimeMsg
        {
            sec = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            nanosec = 0
        };
        
        connection.Publish(topicName, new GripperCommandActionGoal
        {
            goal = new GripperCommandGoal
            {
                command = new GripperCommandMsg
                {
                    position = position,
                    max_effort = strength
                }
            },
            goal_id = new GoalIDMsg
            {
                id = "",
                stamp = timestamp
            },
            header = new HeaderMsg
            {
                frame_id = "",
                seq = 0,
                stamp = timestamp
            }
        });
    }
}