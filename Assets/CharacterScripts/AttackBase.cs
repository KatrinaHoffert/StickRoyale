using UnityEngine;
using System.Collections;

public abstract class AttackBase : MonoBehaviour
{
    // Implementations for performing an attack
    public abstract void Attack1();
    public abstract void Attack2();

    // Implementations for determining if an attack can hit a player (for AI's decision making).
    public abstract bool CanAttack1Hit();
    public abstract bool CanAttack2Hit();

    // Implementations for checking how much an AI should "like" an attack. Returns a value between
    // 0 and 1. Can take into aspect damage, distance from the player that would be hit, etc. Should
    // be a "compared to the other attack" sort of thing, since this would be used to choose between
    // attacks when both could be used.
    public abstract double GetAttack1AiWeight();
    public abstract double GetAttack2AiWeight();
}
