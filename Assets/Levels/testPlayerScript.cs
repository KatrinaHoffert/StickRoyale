using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class testPlayerScript : NetworkBehaviour
{
    // The HP for the character
    [SyncVar]
    public int hitpoints;

    //public bool directionRight = true;

    void Start()
    {
        hitpoints = 100;
    }

    [Command]
    void CmdSpawnAtk()
    {
        Object a = Instantiate(Resources.Load<Object>("Prefabs/MageProjectile"), this.transform.position, new Quaternion(0, 0, 0, 0));
        GameObject b = (GameObject)a;
        b.GetComponent<Projectile>().owner = this.gameObject.GetComponent<AnimationControl>().playerIndex;
    }
    [ClientRpc]
    void RpcSpawnAtk()
    {
        Object a = Instantiate(Resources.Load<Object>("Prefabs/MageProjectile"), this.transform.position, new Quaternion(0, 0, 0, 0));
        GameObject b = (GameObject)a;
        b.GetComponent<Projectile>().owner = this.gameObject.GetComponent<AnimationControl>().playerIndex;
    }

    int timeSinceAttack = 0;

    void turnRight(int i)
    {

        if (levelscript.players[i].GetComponent<AnimationControl>().facingRight)
        {
            //levelscript.players[i].transform.Rotate(0, -180, 0);
            levelscript.players[i].GetComponent<AnimationControl>().facingRight = true;
        }

    }
    

    void turnLeft(int i)
    {
        if (levelscript.players[i].GetComponent<AnimationControl>().facingRight)
        {
            //levelscript.players[i].transform.Rotate(0, 180, 0);
            levelscript.players[i].GetComponent<AnimationControl>().facingRight = false;
        }
    }

    [Command]
    void CmdChooseDirection()
    {
        ChooseDirection();
        
    }
    public void ChooseDirection()
    {
        if (levelscript.players != null)
        {
            if (levelscript.players[this.GetComponent<AnimationControl>().playerIndex].GetComponent<AnimationControl>().facingRight)
            {
                turnRight(this.GetComponent<AnimationControl>().playerIndex);
            }
            else
            {
                turnLeft(this.GetComponent<AnimationControl>().playerIndex);
            }
        }
    }

    
    void FixedUpdate()
    {


        //CmdChooseDirection();

        
        timeSinceAttack++;
        //if the localplayer presses keys move this character, if someone else presses keys do nothing
        if (!isLocalPlayer)
        {
            return;
        }
        if (!isServer)
        {
            //chooseDirection(this.GetComponent<AnimationControl>().playerIndex);
        }


        if (Input.GetKey(KeyCode.Z))
        {
            if (timeSinceAttack > 5)
            {
                timeSinceAttack = 0;
                CmdSpawnAtk();
                RpcSpawnAtk();
                if (!isServer)
                {
                    Object a = Instantiate(Resources.Load<Object>("Prefabs/MageProjectile"), this.transform.position, new Quaternion(0, 0, 0, 0));
                    GameObject b = (GameObject)a;
                    b.GetComponent<Projectile>().owner = this.gameObject.GetComponent<AnimationControl>().playerIndex;
                }

            }

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
        if (transform.position.y < -50)
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

        if(col.gameObject.tag == "MageBullet")
        {
            if (col.gameObject.GetComponent<Projectile>().owner != this.gameObject.GetComponent<AnimationControl>().playerIndex)
            {
                CmdDamaged(5);
            }
        }
    }

    
}