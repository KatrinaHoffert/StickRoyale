using UnityEngine;
using UnityEngine.Networking;
using System.Collections;



public class KnightAttack : NetworkBehaviour {
    // a lock for attacking
    bool isAttacking;
    //refrences the attack prefabs. used for their colliders
    GameObject attack1prefab;
    GameObject attack2prefab;
    public int attackDamage1 = 5;
    public int attackDamage2 = 5;
    

	// Use this for initialization
	void Start () {
        isAttacking = false;
        Transform[] stuff = GetComponentsInChildren<Transform>();
        foreach(Transform thing in stuff)
        {
            if(thing.CompareTag("Attack1"))
            {
                attack1prefab = thing.gameObject;
            }
            if(thing.CompareTag("Attack2"))
            {
                attack2prefab = thing.gameObject;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (gameObject.GetComponentInChildren<NetworkIdentity>().isLocalPlayer)
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

    }
    //first attack variation
    void attack1()
    {
        if(!isAttacking)
        {
            isAttacking = true;

            attack1prefab.GetComponent<BoxCollider2D>().enabled=true;
            Invoke("resetAttack", 1f);
        }

    }
    //second attack variation
    void attack2()
    {
        if (!isAttacking)
        {
            isAttacking = true;

            attack2prefab.GetComponent<BoxCollider2D>().enabled = true;
            Invoke("resetAttack", 1f);
        }
    }

    void resetAttack()
    {
        isAttacking = false;
        attack1prefab.GetComponent<BoxCollider2D>().enabled = false;
        attack2prefab.GetComponent<BoxCollider2D>().enabled = false;

    }

    [Command]
    void CmdapplyDamageOnHit1(string targetId)
    {
        //target.GetComponent<KnightHealth>().SendMessage("Damage", 5);
        PlayerBase[] players = FindObjectsOfType<PlayerBase>();
        foreach (var player in players)
        {
            if (player.uniquePlayerId == targetId)
            {
                player.GetComponentInParent<KnightHealth>().SendMessage("CmdDamage",attackDamage1);
            }
        }
    }
    [Command]
    void CmdapplyDamageOnHit2(string targetId)
    {
        //target.GetComponent<KnightHealth>().SendMessage("Damage", 5);
        PlayerBase[] players = FindObjectsOfType<PlayerBase>();
        foreach (var player in players)
        {
            if (player.uniquePlayerId == targetId)
            {
                player.GetComponentInParent<KnightHealth>().SendMessage("CmdDamage", attackDamage2);
            }
        }
    }
}
