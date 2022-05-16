using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Constraints
{
    public enum OngoingTag {FreezeGrav, FreezeMovementInput, FreezeJumpInput, FreezeDodgeInput, FreezeBlockInput};
    public enum InitialTag {ResetForces, ResetAnimations};

    public OngoingTag[] CheckOngoingConstraints();
    public InitialTag[] CheckInitialConstraints();


}
