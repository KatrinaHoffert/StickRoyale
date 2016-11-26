using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MageAttacks : AttackBase
{
    public GameObject mageAttack1Prefab;

    private Queue<GameObject> attack1Projectiles = new Queue<GameObject>();

    public override void Attack1()
    {
        // Just a simple box in front of it, as a placeholder demonstration of doing an attack
        var attackObject = (GameObject) Instantiate(mageAttack1Prefab, transform.position + new Vector3(0.5f, 0, 0), Quaternion.identity, transform);
        attack1Projectiles.Enqueue(attackObject);

        // Remove the projectile later
        Invoke("ClearAttack1Projectile", 0.5f);
    }

    /// <summary>
    /// Clears the oldest object that is created by Attack1
    /// </summary>
    private void ClearAttack1Projectile()
    {
        Destroy(attack1Projectiles.Dequeue());
    }

    public override void Attack2()
    {
        // TODO
    }
    
    public override bool CanAttack1Hit()
    {
        // TODO: Placeholder
        return false;
    }

    public override bool CanAttack2Hit()
    {
        // TODO: Placeholder
        return false;
    }
    
    public override double GetAttack1AiWeight()
    {
        // TODO: Placeholder
        return 1.0;
    }

    public override double GetAttack2AiWeight()
    {
        // TODO: Placeholder
        return 1.0;
    }
}
