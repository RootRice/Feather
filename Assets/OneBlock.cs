using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneBlock : BlockState
{
    public OneBlock(float _duration)
    {
        ongoingEffects = new List<Constraints.OngoingTag> { Constraints.OngoingTag.FreezeMovementInput, Constraints.OngoingTag.FreezeJumpInput, Constraints.OngoingTag.FreezeBlockInput, Constraints.OngoingTag.FreezeDodgeInput };
        myState = State.OneBlock;
        duration = _duration;
        t = 0;
    }


}
