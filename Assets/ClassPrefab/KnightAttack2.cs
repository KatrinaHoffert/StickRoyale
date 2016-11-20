using UnityEngine;
using System.Collections;

public class KnightAttack2 : MonoBehaviour {

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
            coll.gameObject.GetComponent<KnightHealth>().SendMessage("Damage",5);
        }
    }
}
