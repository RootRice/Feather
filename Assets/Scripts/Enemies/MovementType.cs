using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class MovementType : ScriptableObject
{
    [SerializeField] protected float speed;
    protected Points p;
    protected Transform t;
    public IdlePathManager.DrawType drawType;

    public virtual void init(Transform _t, Points _p)
    {
        t = _t;
        p = _p;
    }
    public abstract Vector3 GetTargetPosition(float deltaTime);
    public abstract void SetAggressionTarget();
    public abstract void RemovePoint(Points p, int i);
    public abstract void AddPoint(Points p, Vector3 position);
}
