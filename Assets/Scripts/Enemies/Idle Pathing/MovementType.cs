using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class IdleMovementType : ScriptableObject
{
    protected float speed;
    protected float tolerance;
    protected Points p;
    protected Transform t;
    [HideInInspector]public IdlePathManager.DrawType drawType;

    public virtual void init(Transform _t, Points _p, float _s)
    {
        tolerance = 0.05f;
        t = _t;
        p = _p;
        speed = _s;
    }
    public abstract Vector3 GetTargetPosition(float deltaTime);
    public abstract void RemovePoint(Points p, int i);
    public abstract void AddPoint(Points p, Vector3 position);
}
