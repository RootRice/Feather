using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControlScheme
{
    public abstract Vector3 Move(Vector3 axis);

    public abstract ActiveForce Jump();

    public abstract void LBlock(PlayerController player);
    public abstract void RBlock(PlayerController player);

}
