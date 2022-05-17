using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidAirControls : ControlScheme
{
    bool jumpCharge = false;
    bool blockCharge = false;

    public void Dodge(PlayerController player, ActiveForce.InitParams initParams)
    {
        if (jumpCharge)
            return;
        player.AddActiveForce(player.dodgeForce, initParams);
        player.AddAnimation(player.dodgeAnimation);
        jumpCharge = true;
    }

    public void Jump(PlayerController player)
    {
        if (jumpCharge)
            return;
        player.AddAnimation(player.jumpAnimation);
        player.AddActiveForce(player.jumpForce);
        jumpCharge = true;
    }

    public void LBlock(PlayerController player)
    {
        if (blockCharge)
            return;
        player.SetBlock(new OneBlock(0.3f));
        player.AddAnimation(player.jumpAnimation);
        player.AddActiveForce(player.doubleJumpForce);
        blockCharge = true;
    }

    public Vector3 Move(Vector3 axis)
    {
        return axis;
    }

    public void RBlock(PlayerController player)
    {
        if (blockCharge)
            return;
        player.SetBlock(new TwoBlock(0.3f));
        player.AddAnimation(player.jumpAnimation);
        player.AddActiveForce(player.doubleJumpForce);
        blockCharge = true;
    }
    public int ChangeControls(int i)
    {
        if(i != (int)ControlScheme.ControlType.MidAir)
        {
            blockCharge = false;
            jumpCharge = false;
        }
        return i;
    }

    public void SuccessfulBlock()
    {
        blockCharge = false;
    }
}
