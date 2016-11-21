using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MovementScript : MonoBehaviour {

    private Vector2 moveForce;
    private Rigidbody2D rigid;

	// Use this for initialization
	void Start () {
        moveForce = new Vector2(0f, 0f);
        rigid = gameObject.GetComponentInParent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.parent = rigid.gameObject.transform;     
	
	}

    /// <summary>
    /// Apply move forces to character based on the current move force.
    /// </summary>
    void FixedUpdate()
    {
        if (moveForce.x>0.5f)
        {
            gameObject.GetComponentInParent<Rigidbody2D>().transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            Debug.Log("Transform: " + gameObject.GetComponentInParent<Rigidbody2D>().transform.rotation.ToString());
            Debug.Log(gameObject.GetComponentInParent<Rigidbody2D>().transform.ToString());
            Debug.Log("should be facing right");
        } else if(moveForce.x<-0.5f)
        {
            gameObject.GetComponentInParent<Rigidbody2D>().transform.localEulerAngles = new Vector3(0f, 180f, 0f);
            Debug.Log("Transform: " + gameObject.GetComponentInParent<Rigidbody2D>().transform.rotation.ToString());
            Debug.Log(gameObject.GetComponentInParent<Rigidbody2D>().transform.ToString());
            Debug.Log("Should be facing left");
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

    /// <summary>
    ///  Applys a jumpforce to the character.
    /// </summary>
    /// <param name="jumpForce"></param>
    private void Jump(Vector2 jumpForce)
    {
        rigid.AddForce(jumpForce);
    }
}
