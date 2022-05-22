using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class IdleMovementType : ScriptableObject
{    public enum DrawType { Lines, Bezier, Box }
    public float speed;
    public float rotSpeed;
    protected float tolerance;
    [HideInInspector] public Points patrol;
    [HideInInspector] public Line[] lines;
    [HideInInspector] public Transform transform;
    protected Rigidbody rigidbody;
    [HideInInspector]public DrawType drawType;
    public float turnDist;

    public virtual void init(Transform _transform, float _speed, float _rotSpeed)
    {
        tolerance = 0.05f;
        transform = _transform;
        speed = _speed;
        rotSpeed = _rotSpeed;
        rigidbody = transform.GetComponent<Rigidbody>();
        
    }
    public abstract void RequestMove(float deltaTime);
    public abstract void RemovePoint(Points p, int i);
    public abstract void AddPoint(Points p, Vector3 position);
}
