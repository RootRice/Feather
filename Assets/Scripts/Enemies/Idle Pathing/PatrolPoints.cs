using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PatrolPoints", menuName = "MovementTypes/Idle/PatrolPoints", order = 1)]
public class PatrolPoints : IdleMovementType
{
    
    int targetIndex = 0;
    
    public override Vector3 GetTargetPosition(float deltaTime)
    {
        Vector3 r = Vector3.MoveTowards(t.position, p.points[targetIndex], speed * deltaTime);
        if (Vector3.SqrMagnitude(r - p.points[targetIndex]) < tolerance)
        {
            targetIndex = Utils.LoopIndex(targetIndex+1, p.points.Count);
            r = Vector3.MoveTowards(t.position, p.points[targetIndex], speed * deltaTime);
        }
        return r;
    }



    public override void RemovePoint(Points p, int i)
    {
        throw new System.NotImplementedException();
    }

    public override void AddPoint(Points p, Vector3 position)
    {
        p.AddPoint(position);
    }


    public override void init(Transform _t, Points _p, float _s)
    {
        base.init(_t, _p, _s);
        drawType = IdlePathManager.DrawType.Lines;
    }


}
