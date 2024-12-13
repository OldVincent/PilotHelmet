
using RosMessageTypes.Geometry;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class BodySynchronizer : MonoBehaviour
{
    public string topicName;

    public float linearVelocityScale = 1.0f;

    public float angularVelocityScale = 1.0f;
    
    public ROSConnection connection;
    
    // Start is called before the first frame update
    void Start()
    {
        connection.RegisterPublisher<TwistMsg>(topicName);
    }

    public void SendVelocity(Vector3 angular, Vector3 linear)
    {
        connection.Publish(topicName, new TwistMsg
        {
            angular = new Vector3Msg(angular.x, angular.y, angular.z),
            linear = new Vector3Msg(linear.x, linear.y, linear.z)
        });
    }

    // Update is called once per frame
    void Update()
    {
        OVRInput.Update();
        
        var inputLinearVelocity = new Vector2(0.0f, 0.0f);
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) > 0.9)
            inputLinearVelocity = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        var linearVelocity = new Vector3
        {
            x = inputLinearVelocity.y * linearVelocityScale,
            y = -inputLinearVelocity.x * linearVelocityScale,
            z = 0.0f
        };
        
        var inputAngularVelocity = new Vector2(0.0f, 0.0f);
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch) > 0.9)
            inputAngularVelocity = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
        var angularVelocity = new Vector3
        {
            x = 0.0f,
            y = 0.0f,
            z = -inputAngularVelocity.x * angularVelocityScale
        };

        SendVelocity(angularVelocity, linearVelocity);
    }
}
