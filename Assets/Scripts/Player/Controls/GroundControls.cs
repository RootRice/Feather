using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundControls : ControlScheme
{
    int dashHash = Animator.StringToHash("Dash");
    int jumpHash = Animator.StringToHash("Jump");

    public void Dodge(PlayerController player, ActiveForce.InitParams initParams)
    {
        Debug.Log("Dash");
        player.transform.position += new Vector3(0f, 0.1f, 0f);
        player.AddActiveForce(player.dodgeForce, initParams);
        player.PlayAnimation(dashHash,1.0f);
    }

    public void Jump(PlayerController player, ActiveForce.InitParams initParams)
    {
        player.AddActiveForce(player.jumpForce);
        player.PlayAnimation(jumpHash, 1.0f);
    }

    public void LBlock(PlayerController player)
    {
        player.SetBlock(new OneBlock(0.3f));
    }
    public void RBlock(PlayerController player)
    {
        player.SetBlock(new TwoBlock(0.3f));
    }
    public Vector3 Move(Vector3 axis)
    {
        return axis;
    }

    public int ChangeControls(int i)
    {
        return i;
    }

    public void SuccessfulBlock()
    {
        
    }
}
