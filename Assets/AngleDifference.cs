
using System;

public static class AngleDifference
{
    public static double ToSmallerThan180(double angleDifference)
    {
        if (Math.Abs(angleDifference) <= 180.0) 
            return angleDifference;
        if (angleDifference > 0)
            angleDifference = -360.0f + angleDifference;
        else 
            angleDifference = 360.0f + angleDifference;
        return angleDifference;
    }
    
    public static double ToSmallerThan180(double angle1, double angle2)
    {
        var difference = angle1 - angle2;
        if (!(Math.Abs(difference) > 180.0)) 
            return difference;
        if (difference > 0)
            difference = -360.0f + difference;
        else 
            difference = 360.0f + difference;
        return difference;
    }
}