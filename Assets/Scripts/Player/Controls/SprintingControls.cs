using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintingControls : ControlScheme
{
    int dashHash = Animator.StringToHash("Dash");
    int jumpHash = Animator.StringToHash("Jump");

    public void Dodge(PlayerController player, ActiveForce.InitParams initParams)
    {
        Debug.Log("Sprint Dash");
        player.SetSpeedTier(2, false);
        player.AddActiveForce(player.dashDodgeForce, initParams);
        player.PlayAnimation(dashHash, 1.0f);
    }

    public void Jump(PlayerController player, ActiveForce.InitParams initParams)
    {
        player.AddActiveForce(player.dashJumpForce, initParams);
        player.PlayAnimation(jumpHash, 1.0f);
    }

    public void LBlock(PlayerController player)
    {
        player.SetBlock(new OneBlock(0.3f));
        player.AddTransformAnimation(player.jumpAnimation);
        player.AddActiveForce(player.doubleJumpForce);
    }

    public Vector3 Move(Vector3 axis)
    {
        return axis;
    }

    public void RBlock(PlayerController player)
    {
        player.SetBlock(new TwoBlock(0.3f));
        player.AddTransformAnimation(player.jumpAnimation);
        player.AddActiveForce(player.doubleJumpForce);
    }
    public int ChangeControls(int i)
    {
        return i;
    }

    public void SuccessfulBlock()
    {
        
    }
}
