using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MovementScript : MonoBehaviour {

    private Vector2 moveForce;
    private Rigidbody2D rigid;

	// Use this for initialization
	void Start () {
        moveForce = new Vector2(0f, 0f);
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        if (moveForce.x>0.5f)
        {
            gameObject.transform.rotation.Set(0f, 0f, 0f, 0f);
        } else
        {
            gameObject.transform.rotation.Set(0f, 180f, 0f, 0f);
        }

        if(rigid.velocity.magnitude<4)
        {
            ApplyMoveForce(moveForce);
        }

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

    private void Jump(Vector2 jumpForce)
    {
        rigid.AddForce(jumpForce);
    }
}
