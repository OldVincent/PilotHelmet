using RosMessageTypes.Geometry;
using UnityEngine;

public static class GeometryMsgExtensions
{
    public static Vector3Msg ToVector3Msg(this Vector3 vector)
    {
        return new Vector3Msg()
        {
            x = vector.x,
            y = vector.y,
            z = vector.z
        };
    }
    
    public static QuaternionMsg ToQuaternionMsg(this Quaternion vector)
    {
        return new QuaternionMsg()
        {
            x = vector.x,
            y = vector.y,
            z = vector.z,
            w = vector.w
        };
    }

    public static TransformMsg ToTransformMsg(this Transform transform)
    {
        return new TransformMsg()
        {
            translation = transform.position.ToVector3Msg(),
            rotation = transform.rotation.ToQuaternionMsg()
        };
    }
    
    public static Vector3 Vector3(this Vector3Msg vector)
    {
        return new Vector3((float)vector.x, (float)vector.y, (float)vector.z);
    }
}