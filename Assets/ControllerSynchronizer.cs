using System;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class ControllerSynchronizer : MonoBehaviour
{
    public string buttonXTopicName = "/pilot_helmet/controller/button_x";
    public string buttonYTopicName = "/pilot_helmet/controller/button_y";
    public string buttonATopicName = "/pilot_helmet/controller/button_a";
    public string buttonBTopicName = "/pilot_helmet/controller/button_b";
    public string handTriggerLTopicName = "/pilot_helmet/controller/hand_trigger_l";
    public string handTriggerRTopicName = "/pilot_helmet/controller/hand_trigger_r";
    public string indexTriggerLTopicName = "/pilot_helmet/controller/index_trigger_l";
    public string indexTriggerRTopicName = "/pilot_helmet/controller/index_trigger_r";
    public string stickLTopicName = "/pilot_helmet/controller/stick_l";
    public string stickRTopicName = "/pilot_helmet/controller/stick_r";
    
    public ROSConnection connection;

    public int maxFrequency = 120;
    
    // Start is called before the first frame update
    void Start()
    {
        connection.RegisterPublisher<BoolMsg>(buttonXTopicName);
        connection.RegisterPublisher<BoolMsg>(buttonYTopicName);
        connection.RegisterPublisher<BoolMsg>(buttonATopicName);
        connection.RegisterPublisher<BoolMsg>(buttonBTopicName);
        connection.RegisterPublisher<Float32Msg>(handTriggerLTopicName);
        connection.RegisterPublisher<Float32Msg>(handTriggerRTopicName);
        connection.RegisterPublisher<Float32Msg>(indexTriggerLTopicName);
        connection.RegisterPublisher<Float32Msg>(indexTriggerRTopicName);
        connection.RegisterPublisher<Vector3Msg>(stickLTopicName);
        connection.RegisterPublisher<Vector3Msg>(stickRTopicName);
    }

    private DateTime _lastUpdateTime;

    void UpdateButton(string topicName, OVRInput.RawButton buttonMask, OVRInput.Controller controllerMask)
    {
        connection.Publish(topicName,
            OVRInput.Get(buttonMask, controllerMask) ? new BoolMsg(true) : new BoolMsg(false));
    }
    
    void UpdateTrigger(string topicName, OVRInput.Axis1D triggerMask, OVRInput.Controller controllerMask)
    {
        connection.Publish(topicName, new Float32Msg(OVRInput.Get(triggerMask, controllerMask)));
    }
    
    void UpdateStick(string topicName, OVRInput.Controller controllerMask)
    {
        var value = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controllerMask);
        connection.Publish(topicName, new Vector3Msg
        {
            x = value.x,
            y = value.y,
            z = 0.0f
        });
    }
    
    // Update is called once per frame
    void Update()
    {
        OVRInput.Update();
        
        if (DateTime.Now - _lastUpdateTime < TimeSpan.FromSeconds(1.0 / maxFrequency))
            return;
        _lastUpdateTime = DateTime.Now;

        UpdateButton(buttonATopicName, OVRInput.RawButton.A, OVRInput.Controller.RTouch);
        UpdateButton(buttonBTopicName, OVRInput.RawButton.B, OVRInput.Controller.RTouch);
        UpdateButton(buttonXTopicName, OVRInput.RawButton.X, OVRInput.Controller.LTouch);
        UpdateButton(buttonYTopicName, OVRInput.RawButton.Y, OVRInput.Controller.LTouch);
        UpdateTrigger(handTriggerRTopicName, OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch);
        UpdateTrigger(handTriggerLTopicName, OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch);
        UpdateTrigger(indexTriggerRTopicName, OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        UpdateTrigger(indexTriggerLTopicName, OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
        UpdateStick(stickLTopicName, OVRInput.Controller.LTouch);
        UpdateStick(stickRTopicName, OVRInput.Controller.RTouch);
    }
}