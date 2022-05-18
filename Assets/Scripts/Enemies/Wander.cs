using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wander", menuName = "MovementTypes/Wander", order = 1)]
public class Wander : MovementType
{
    Vector3 targetPos = Vector3.zero;
    [SerializeField] float waitTime;
    float elapsedTime;
    public override void AddPoint(Points p, Vector3 position)
    {
        
    }

    public override Vector3 GetTargetPosition(float deltaTime)
    {
        if (Time.timeSinceLevelLoad-elapsedTime < waitTime)
            return t.position;
        Vector3 r = Vector3.MoveTowards(t.position, targetPos, speed * deltaTime);
        if(t.position == targetPos)
        {
            elapsedTime = Time.timeSinceLevelLoad;
            Vector3 rand = new Vector3(Random.Range(0.0f, 1.0f), 1, Random.Range(0.0f, 1.0f));
            targetPos = p.points[0] + Vector3.Scale((p.points[1]-p.points[0]), rand);
            r = Vector3.MoveTowards(t.position, targetPos, speed * deltaTime);
        }
        return r;
    }

    public override void init(Transform _t, Points _p)
    {
        base.init(_t, _p);
        targetPos = t.position;
    }

    public override void RemovePoint(Points p, int i)
    {
        throw new System.NotImplementedException();
    }

    public override void SetAggressionTarget()
    {
        throw new System.NotImplementedException();
    }
}
