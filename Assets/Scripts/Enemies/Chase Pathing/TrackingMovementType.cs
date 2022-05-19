using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrackingMovementType : ScriptableObject
{
    protected float speed;
    protected float tolerance;
    protected float minRadius;
    protected float maxRadius;
    protected Transform t;
    protected Transform p;

    public virtual void init(Transform _t, Transform _p, float _s, float _min, float _max)
    {
        tolerance = 0.05f;
        t = _t;
        speed = _s;
        p = _p;
        minRadius = _min;
        maxRadius = _max;
    }
    public abstract Vector3 GetTargetPosition(float deltaTime);
}
