using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
    public int owner = -1;
    public int speed;
    public bool directionRight;
    private int age = 0;
    /*
    public Projectile(int playerWhoSpawnedThis,bool playerFacingRight)
    {
        owner = playerWhoSpawnedThis;
        directionRight = playerFacingRight;
    }
    */
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        this.transform.position = this.transform.position + new Vector3(1, 0);
        if (owner != -1)
        {
            Physics2D.IgnoreCollision(this.gameObject.GetComponent<BoxCollider2D>(), levelscript.players[owner].GetComponent<BoxCollider2D>());
        }
        age++;
        if (age > 1000)
        {
            Destroy(this.gameObject);
        }
    }
}
