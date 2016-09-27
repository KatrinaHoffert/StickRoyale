using UnityEngine;
using System.Collections;

public class MovementScript : MonoBehaviour {

    private Vector2 moveForce;
    private Rigidbody2D rigid;

	// Use this for initialization
	void Start () {
        moveForce = new Vector2(0f, 0f);
        rigid = GameObject.FindObjectOfType<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        ApplyMoveForce(moveForce);
    }

    /// <summary>
    /// Applies the moveforce to the gameObject
    /// </summary>

    private void ApplyMoveForce(Vector2 force)
    {
        rigid.AddForce(force);
    }
    /// <summary>
    /// Changes the force of the movement
    /// </summary>
    /// <param name="newForce"></param>
    void ChangeMoveForce(Vector2 newForce)
    {
        moveForce = newForce;
    }
}
