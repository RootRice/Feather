using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoBlock : BlockState
{
    public TwoBlock(float _duration)
    {
        ongoingEffects = new List<Constraints.OngoingTag> { Constraints.OngoingTag.FreezeMovementInput, Constraints.OngoingTag.FreezeJumpInput, Constraints.OngoingTag.FreezeBlockInput, Constraints.OngoingTag.FreezeDodgeInput };
        initialEffects = new List<Constraints.InitialTag> { Constraints.InitialTag.ResetForces, Constraints.InitialTag.ResetAnimations };
        myState = State.TwoBlock;
        duration = _duration;
        t = 0;
    }


}
