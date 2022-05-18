using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PatrolPoints", menuName = "MovementTypes/PatrolPoints", order = 1)]
public class PatrolPoints : MovementType
{
    
    
    int targetIndex = 0;
    
    public override Vector3 GetTargetPosition(float deltaTime)
    {
        Vector3 r = Vector3.MoveTowards(t.position, p.points[targetIndex], speed * deltaTime);
        if(r == p.points[targetIndex])
        {
            targetIndex = (targetIndex+1)%p.points.Count;
            r = Vector3.MoveTowards(r, p.points[targetIndex], speed * deltaTime);
        }
        return r;
    }

    public override void SetAggressionTarget()
    {
        throw new System.NotImplementedException();
    }

    public override void RemovePoint(Points p, int i)
    {
        throw new System.NotImplementedException();
    }

    public override void AddPoint(Points p, Vector3 position)
    {
        p.AddPoint(position);
    }

    
}
