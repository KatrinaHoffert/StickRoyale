using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RogueAttack1Trigger : AttackTriggerBase
{
    protected override int GetDamage()
    {
        return 17;
    }

    protected override Vector2 GetDamageForce()
    {
        return Vector3.right * 50;
    }
}
