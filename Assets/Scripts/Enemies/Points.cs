using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Points
{
    [SerializeField, HideInInspector]
    public List<Vector3> points;

    public Points(Transform t)
    {
        points = new List<Vector3>();
        points.Add(t.position + Vector3.left);
        points.Add(t.position + Vector3.right);
    }

    public int NumPoints()
    {
        return points.Count;
    }

    public void AddPoint(Vector3 pos)
    {
        points.Add(pos);
    }

    public void MovePoint(int index, Vector3 position)
    {
        points[index] = position;
    }
}
