using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static int LoopIndex(int i, int resetPoint)
    {
        return (i+resetPoint) % resetPoint;
    }
}
