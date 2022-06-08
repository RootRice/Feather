using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static int LoopIndex(int i, int resetPoint)
    {
        return (i+resetPoint) % resetPoint;
    }

    public static Vector2 V3ToV2(Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }

    public static float LoopAngle(float angle)
    {
        return (angle + 360) % 360;
    }

}
