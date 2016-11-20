using UnityEngine;
using System.Collections;

public class KnightHealth : MonoBehaviour {

    public int health = 100;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Damage(int dam)
    {
        health -= dam;
    }
}
