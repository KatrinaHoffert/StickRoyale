using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class KnightHealth : NetworkBehaviour {


    //sync var makes it sync between clients
    [SyncVar]public int health = 100;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    [Command]
    public void CmdDamage(int dam)
    {
        if(!gameObject.GetComponentInChildren<NetworkIdentity>().isServer)
        {
            return;
        }
        health -= dam;
    }
}
