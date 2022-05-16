using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface ControlScheme
{
    public enum ControlType { Grounded, MidAir };

    public abstract Vector3 Move(Vector3 axis);
    public abstract void Jump(PlayerController player);
    public abstract void LBlock(PlayerController player);
    public abstract void RBlock(PlayerController player);
    public abstract void Dodge(PlayerController player, ActiveForce.InitParams initParams);
    public abstract int ChangeControls(int i);

}
