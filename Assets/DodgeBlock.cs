using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeBlock : BlockState
{
    public DodgeBlock(float _duration, State _state)
    {
        ongoingEffects = new List<Constraints.OngoingTag> { Constraints.OngoingTag.FreezeBlockInput };
        initialEffects = new List<Constraints.InitialTag> { Constraints.InitialTag.ResetForces };
        myState = _state;
        duration = _duration;
        t = 0;
    }


}
