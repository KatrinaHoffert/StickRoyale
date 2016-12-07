using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KnightAttack2Trigger : AttackTriggerBase
{
    protected override int GetDamage()
    {
        return 19;
    }

    protected override Vector2 GetDamageForce()
    {
        return new Vector2(1.0f, 0.5f) * 205;
    }
}
