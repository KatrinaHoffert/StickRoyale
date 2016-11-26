using UnityEngine;
using System.Collections;

public class KnightAttack : AttackBase
{
    // a lock for attacking
    bool isAttacking;
    public float attackCooldown=2;
    //refrences the attack prefabs. used for their colliders
    GameObject Attack1prefab;
    GameObject Attack2prefab;
    public int attackDamage1 = 5;
    public int attackDamage2 = 5;


    // Use this for initialization
    void Start()
    {
        //assign the colliders in the attack prefabs so they can be called upon
        isAttacking = false;
        Transform[] stuff = GetComponentsInChildren<Transform>();
        foreach (Transform thing in stuff)
        {
            if (thing.CompareTag("Attack1"))
            {
                Attack1prefab = thing.gameObject;
            }
            if (thing.CompareTag("Attack2"))
            {
                Attack2prefab = thing.gameObject;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAttacking)
        {
            if (Input.GetAxis("PrimaryAttack") > 0)
            {
                Attack1();
            }
            if (Input.GetAxis("SecondaryAttack") > 0)
            {
                Attack2();
            }
        }
    }

    //first attack variation
    public override void Attack1()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            Attack1prefab.GetComponent<BoxCollider2D>().enabled = true;
            gameObject.GetComponent<PlayerMovement>().setDirectionLocked(true);
            Invoke("resetAttack", 1);
            Invoke("attackCooldownReset", attackCooldown);
        }

    }
    //second attack variation
    
    public override void Attack2()
    {
        if (!isAttacking)
        {
            isAttacking = true;

            Attack2prefab.GetComponent<BoxCollider2D>().enabled = true;
            gameObject.GetComponent<PlayerMovement>().setDirectionLocked(true);
            Invoke("resetAttack", 1f);
            Invoke("attackCooldownReset", attackCooldown);
        }
    }
    
    public void resetAttack()
    {
        Attack1prefab.GetComponent<BoxCollider2D>().enabled = false;
        Attack2prefab.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<PlayerMovement>().setDirectionLocked(false);
    }

    void attackCooldownReset()
    {
        isAttacking = false;
    }


    public override float GetAttack1Delay()
    {
        return 0.5f;
    }

    public override float GetAttack2Delay()
    {
        return 0.5f;
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
