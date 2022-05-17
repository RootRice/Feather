using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundControls : ControlScheme
{

    public void Dodge(PlayerController player, ActiveForce.InitParams initParams)
    {
        player.transform.position += new Vector3(0f, 0.1f, 0f);
        player.AddActiveForce(player.dodgeForce, initParams);
        player.AddAnimation(player.dodgeAnimation);
    }

    public void Jump(PlayerController player)
    {
        player.AddActiveForce(player.jumpForce);
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
