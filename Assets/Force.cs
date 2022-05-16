using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force : ScriptableObject
{
    public virtual bool ShouldTerminate() { return false; }

    public virtual void Initialise() { }

    public virtual void Initialise(float _x, float _y, float _z, float _t) { }

    public virtual Vector3 ApplyForce(float deltaTime) { return Vector3.zero; }
    public virtual Vector3 ApplyForce(float deltaTime, bool reset) { return Vector3.zero; }
    public virtual Constraints.OngoingTag[] CheckConstraints() { return null; }
}
