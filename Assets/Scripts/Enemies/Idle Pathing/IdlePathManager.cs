using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdlePathManager : MonoBehaviour
{
    [SerializeField] IdleMovementType movementType;
    [SerializeField] float speed;
    [HideInInspector] public Points points;
    [HideInInspector] public DrawType drawType;
    public enum DrawType { Lines, Bezier, Box}

    private void Start()
    {
        movementType.init(transform, points, speed);
    }
    public Vector3 GetNewPosition(float deltaTime)
    {
        return movementType.GetTargetPosition(deltaTime);
    }

    public void CreatePoints()
    {
        points = new Points(transform);
        drawType = movementType.drawType;
    }

    public void AddPoint(Vector3 position)
    {
        movementType.AddPoint(points, position);
    }

    public void MovePoint(int i, Vector3 position)
    {
        points.MovePoint(i, new Vector3(position.x, transform.position.y, position.z));
    }

    public Points GetPoints()
    {
        return points;
    }
}
