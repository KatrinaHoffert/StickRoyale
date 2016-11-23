using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class KnightAttack2 : NetworkBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            string playerId = coll.gameObject.GetComponentInChildren<NetworkIdentity>().GetComponent<PlayerBase>().uniquePlayerId;
            gameObject.GetComponentInParent<KnightAttack>().SendMessage("CmdapplyDamageOnHit2", playerId);
        }
    }
}
