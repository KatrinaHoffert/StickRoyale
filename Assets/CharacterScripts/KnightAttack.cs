using UnityEngine;
using System.Collections;

public class KnightAttack : MonoBehaviour
{
    // a lock for attacking
    bool isAttacking;
    public float attackCooldown=2;
    //refrences the attack prefabs. used for their colliders
    GameObject attack1prefab;
    GameObject attack2prefab;
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
                attack1prefab = thing.gameObject;
            }
            if (thing.CompareTag("Attack2"))
            {
                attack2prefab = thing.gameObject;
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
                attack1();
            }
            if (Input.GetAxis("SecondaryAttack") > 0)
            {
                attack2();
            }
        }
    }

    //first attack variation
    void attack1()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            attack1prefab.GetComponent<BoxCollider2D>().enabled = true;
            gameObject.GetComponent<PlayerMovement>().setDirectionLocked(true);
            Invoke("resetAttack", 1);
            Invoke("attackCooldownReset", attackCooldown);
        }

    }
    //second attack variation
    
    void attack2()
    {
        if (!isAttacking)
        {
            isAttacking = true;

            attack2prefab.GetComponent<BoxCollider2D>().enabled = true;
            gameObject.GetComponent<PlayerMovement>().setDirectionLocked(true);
            Invoke("resetAttack", 1f);
            Invoke("attackCooldownReset", attackCooldown);
        }
    }
    
    void resetAttack()
    {
        attack1prefab.GetComponent<BoxCollider2D>().enabled = false;
        attack2prefab.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<PlayerMovement>().setDirectionLocked(false);
    }

    void attackCooldownReset()
    {
        isAttacking = false;
    }


    public float GetAttack1Delay()
    {
        return 0.5f;
    }

    public float GetAttack2Delay()
    {
        return 0.5f;
    }

    public bool CanAttack1Hit()
    {
        // TODO: Placeholder
        return false;
    }

    public bool CanAttack2Hit()
    {
        // TODO: Placeholder
        return false;
    }

    public double GetAttack1AiWeight()
    {
        // TODO: Placeholder
        return 1.0;
    }

    public double GetAttack2AiWeight()
    {
        // TODO: Placeholder
        return 1.0;
    }

}
