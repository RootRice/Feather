using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ControlScheme
{
    public enum ControlType{ Grounded, MidAir};
    public  Vector3 Move(Vector3 axis);
    public  void Jump(PlayerController player);
    public  void LBlock(PlayerController player);
    public  void RBlock(PlayerController player);
    public  void Dodge(PlayerController player, ActiveForce.InitParams initParams);
    public int ChangeControls(int i);

}
