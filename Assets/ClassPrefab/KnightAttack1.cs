using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class KnightAttack1 : NetworkBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update() {

    }
    
    
    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.CompareTag("Player"))
        {
            //coll.gameObject.GetComponent<KnightHealth>().SendMessage("Damage",5);
            Collider2D coll1 = coll;
            //gameObject.GetComponentInParent<KnightAttack>().SendMessage("CmdapplyDamageOnHit", coll1);
            //coll.gameObject.GetComponent<NetworkIdentity>()
        }
    }
}
