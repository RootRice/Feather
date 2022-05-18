using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wander", menuName = "MovementTypes/Wander", order = 1)]
public class Wander : IdleMovementType
{
    Vector3 targetPos = Vector3.zero;
    Vector3 prevPoint;
    [SerializeField] float waitTime;
    float elapsedTime;
    [SerializeField] [Range(0, 1f)] float acceleration;
    public override void AddPoint(Points p, Vector3 position)
    {
        
    }

    public override Vector3 GetTargetPosition(float deltaTime)
    {
        if (Time.timeSinceLevelLoad-elapsedTime < waitTime)
            return t.position;
        float tempSpeed = GetDistanceFromMiddle01(targetPos, prevPoint, t.position) * speed;
        Vector3 r = Vector3.MoveTowards(t.position, targetPos, tempSpeed * deltaTime);
        if (Vector3.SqrMagnitude(r - targetPos) < tolerance)
        {
            elapsedTime = Time.timeSinceLevelLoad;
            Vector3 rand = new Vector3(Random.Range(0.0f, 1.0f), 1, Random.Range(0.0f, 1.0f));
            prevPoint = targetPos;
            targetPos = p.points[0] + Vector3.Scale((p.points[1]-p.points[0]), rand);
            tempSpeed = GetDistanceFromMiddle01(targetPos, prevPoint, t.position) * speed;
            r = Vector3.MoveTowards(t.position, targetPos, tempSpeed * deltaTime);
        }
        return r;
    }

    public override void init(Transform _t, Points _p, float _s)
    {
        base.init(_t, _p, _s);
        drawType = IdlePathManager.DrawType.Box;
        prevPoint = t.position-Vector3.back;
        targetPos = t.position;
        elapsedTime = 0;
    }

    public override void RemovePoint(Points p, int i)
    {
        throw new System.NotImplementedException();
    }


    float GetDistanceFromMiddle01(Vector3 a, Vector3 b, Vector3 pos)
    {
        Vector3 maxDistFromMiddle = (b - a) / 2;
        Vector3 middle = a + maxDistFromMiddle;
        float distModifier = (Vector3.SqrMagnitude(maxDistFromMiddle) / Vector3.SqrMagnitude((p.points[1] - p.points[0])/2));
        Debug.Log(distModifier);
        return Mathf.Max((1 - Vector3.SqrMagnitude(pos - middle) / Vector3.SqrMagnitude(maxDistFromMiddle)) * distModifier, acceleration*(1-distModifier));
    }
}
