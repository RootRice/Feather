using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlockState : Constraints
{
    public enum State { OneBlock, TwoBlock, Dodging, None }

    public State myState;
    protected float duration;
    protected float t;
    public bool blockReady;

    protected List<Constraints.OngoingTag> ongoingEffects = new List<Constraints.OngoingTag>();
    protected List<Constraints.InitialTag> initialEffects = new List<Constraints.InitialTag>();

    public virtual bool Progress(float deltaTime)
    {
        t += deltaTime;
        return t > duration;
    }
    public virtual bool CompareStates(DamageTypes.Type type)
    {
        if((int)type == (int)myState)
        {
            ongoingEffects.Remove(Constraints.OngoingTag.FreezeBlockInput);
            return true;
        }
        return false;
    }

    public virtual Constraints.InitialTag[] CheckInitialConstraints()
    {
        return initialEffects.ToArray();
    }

    public virtual Constraints.OngoingTag[] CheckOngoingConstraints()
    {
        return ongoingEffects.ToArray();
    }


}
