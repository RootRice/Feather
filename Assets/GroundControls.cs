using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundControls : ControlScheme
{
    public void Dodge(PlayerController player, ActiveForce.InitParams initParams)
    {
        player.AddAnimation(player.dodgeAnimation);
        player.AddActiveForce(player.dodgeForce, initParams); 
    }

    public void Jump(PlayerController player)
    {
        player.AddActiveForce(player.jumpForce);
    }

    public void LBlock(PlayerController player)
    {
        
    }
    public void RBlock(PlayerController player)
    {
        
    }
    public Vector3 Move(Vector3 axis)
    {
        return axis;
    }

    public int ChangeControls(int i)
    {
        return i;
    }
}
