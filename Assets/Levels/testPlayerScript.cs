using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class testPlayerScript : NetworkBehaviour
{
    // The HP for the character
    [SyncVar]
    public int hitpoints;

    void Start()
    {
        hitpoints = 100;
    }

    void FixedUpdate()
    {
        //if the localplayer presses keys move this character, if someone else presses keys do nothing
        if (!isLocalPlayer)
        {
            return;
        }

        //moves the characther with arrow keys
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var y = Input.GetAxis("Vertical") * Time.deltaTime * 300.0f;
        //transform.Translate(x, y, 0);
        this.GetComponent<Rigidbody2D>().AddForce(new Vector2(20*x, 20*y), ForceMode2D.Force);


        //die if y position is below a certain y
        CmdDie();
    }

    /// <summary>
    /// Die if below certain y, in this case -300
    /// the client must run this function so the command tag is used
    /// </summary>
    [Command]
    void CmdDie()
    {
        if (transform.position.y < -300)
        {
            this.hitpoints = 0;
        }
    }

    /// <summary>
    /// deduct an integer value from hitpoints, the client must run this function so the command tag is used
    /// </summary>
    /// <param name="damage"></param>
    [Command]
    void CmdDamaged(int damage)
    {
        this.hitpoints = this.hitpoints - damage;
    }
    
    /// <summary>
    /// when a player collides with this player take 1 damage
    /// </summary>
    /// <param name="col">The object we've collided with.</param>
    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            CmdDamaged(1);
        }
    }
}