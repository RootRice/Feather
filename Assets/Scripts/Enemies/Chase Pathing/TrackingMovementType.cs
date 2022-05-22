using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrackingMovementType : ScriptableObject
{
    protected float maxSpeed;
    protected float rotSpeed;
    protected float tolerance;
    protected float minRadius;
    protected float maxRadius;
    protected Transform transform;
    protected Transform player;

    public virtual void init(Transform _transform, Transform _player, float _speed, float _rotSpeed,float _minRadius, float _maxRadius)
    {
        tolerance = 0.05f;
        transform = _transform;
        maxSpeed = _speed;
        rotSpeed = _rotSpeed;
        player = _player;
        minRadius = _minRadius;
        maxRadius = _maxRadius;
    }
    public abstract void RequestMove(float deltaTime);
}
