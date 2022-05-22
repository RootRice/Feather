using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PatrolPoints", menuName = "MovementTypes/Idle/PatrolPoints", order = 1)]
public class PatrolPoints : IdleMovementType
{
    
    int targetIndex = 0;
    GameObject DEBUG;
    public override void RequestMove(float deltaTime)
    {
        Vector3 position = transform.position;
        Vector3 forward = transform.forward;
        Vector3 dir = (patrol.points[targetIndex] - position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * deltaTime);
        rigidbody.AddForce(forward * speed * deltaTime * Mathf.Max(Vector3.Dot(dir, forward), 0.8f));
        DEBUG.transform.position = patrol.points[targetIndex];
        if (lines[targetIndex].HasCrossedLine(Utils.V3ToV2(position)))
        {
            targetIndex = Utils.LoopIndex(targetIndex + 1, lines.Length);
        }
    }



    public override void RemovePoint(Points p, int i)
    {
        throw new System.NotImplementedException();
    }

    public override void AddPoint(Points p, Vector3 position)
    {
        p.AddPoint(position);
    }


    public override void init(Transform _t, float _s, float _rs)
    {
        base.init(_t, _s, _rs);
        drawType = DrawType.Lines;
        targetIndex = 0;
        DEBUG = GameObject.Find("DebugCube");

        lines = new Line[patrol.NumPoints()];
        for (int i = 0; i < lines.Length; i++)
        {
            Vector2 currentPoint = Utils.V3ToV2(patrol.points[i]);
            Vector2 prevPoint = Utils.V3ToV2(patrol.points[Utils.LoopIndex(i - 1, lines.Length)]);
            Vector2 dirToCurrentPoint = (currentPoint - prevPoint).normalized;
            Vector2 turnBoundaryPoint = currentPoint - dirToCurrentPoint * turnDist;
            lines[i] = new Line(turnBoundaryPoint, prevPoint);
        }
    }


}
