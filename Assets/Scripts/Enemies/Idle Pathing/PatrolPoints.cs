using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PatrolPoints", menuName = "MovementTypes/Idle/PatrolPoints", order = 1)]
public class PatrolPoints : IdleMovementType
{
    
    int targetIndex = 0;
    
    public override Vector3 GetTargetPosition(float deltaTime)
    {
        //transform.rotation = Quaternion.RotateTowards()
        return Vector3.zero;
    }



    public override void RemovePoint(Points p, int i)
    {
        throw new System.NotImplementedException();
    }

    public override void AddPoint(Points p, Vector3 position)
    {
        p.AddPoint(position);
    }


    public override void init(Transform _t, Points _p, float _s, float _rs)
    {
        base.init(_t, _p, _s, _rs);
        drawType = DrawType.Lines;
    }


}
