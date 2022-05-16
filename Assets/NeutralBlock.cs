using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralBlock : BlockState
{
    public NeutralBlock()
    {
        ongoingEffects = new List<Constraints.OngoingTag>();
        initialEffects = new List<Constraints.InitialTag>();
        myState = State.None;
    }

    public override bool CompareStates(DamageTypes.Type type)
    {
        return false;
    }

    public override bool Progress(float deltaTime)
    {
        return false;
    }
}
