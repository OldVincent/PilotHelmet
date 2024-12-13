
using UnityEngine;

public static class RosCoordinate
{
    public static Vector3 ToRosCoordinate(in this Vector3 position)
    {
        return new Vector3(position.z, -position.x, position.y);
    }

    public static Quaternion ToRosCoordinate(in this Quaternion rotation)
    {
        return new Quaternion(rotation.z, -rotation.x, rotation.y, -rotation.w);
    }
}