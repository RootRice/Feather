using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidAirControls : ControlScheme
{
    bool jumpCharge = false;
    bool blockCharge = false;

    int airDashHash = Animator.StringToHash("Air Dash");
    int dashHash = Animator.StringToHash("Dash");
    int jumpHash = Animator.StringToHash("Jump");
    int doubleJumpHash = Animator.StringToHash("Double Jump");
    int startMovingHash = Animator.StringToHash("StartMoving");

    public void Dodge(PlayerController player, ActiveForce.InitParams initParams)
    {
        if (jumpCharge)
            return;
        player.AddActiveForce(player.dodgeForce, initParams);
        player.AddTransformAnimation(player.dodgeAnimation);
        player.PlayAnimation(airDashHash, 1.0f);
        jumpCharge = true;
    }

    public void Jump(PlayerController player, ActiveForce.InitParams initParams)
    {
        if (jumpCharge)
            return;
        player.AddTransformAnimation(player.jumpAnimation);
        player.PlayAnimation(doubleJumpHash, 1.0f);
        player.AddActiveForce(player.doubleJumpForce);
        jumpCharge = true;
    }

    public void LBlock(PlayerController player)
    {
        if (blockCharge)
            return;
        player.SetBlock(new OneBlock(0.3f));
        player.AddTransformAnimation(player.jumpAnimation);
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
        player.AddTransformAnimation(player.jumpAnimation);
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
