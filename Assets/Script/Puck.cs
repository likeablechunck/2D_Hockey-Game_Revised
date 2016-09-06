using UnityEngine;
using System.Collections;

public class Puck : MonoBehaviour {
	
	// Store ball old position
	// the player that owns the ball at the moment
	public Player owner;
    public Vector3 lastDirection;
    public Vector3 startingPosition;
    public float speed = 10;
    public TextMesh ballOwnerTExt;
    public Vector3 instantVelocity;
    Vector3 lastPosition;

	// Use this for initialization
	void Start () {
        lastPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        instantVelocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;

        float directionX = lastDirection.x;
        float directionY = lastDirection.y;
        if (owner != null)
        {
            ballOwnerTExt.text = owner.name;
        } else
        {
            ballOwnerTExt.text = "no one";
        }
        //print(string.Format("puck is at {0}", transform.position));
        bool edgeHit = false;
        //if (transform.position.x < -7.35) 
        if (transform.position.x < -6.50)
        {
            transform.position = new Vector3(-6.20f, transform.position.y, 0);
            directionX = lastDirection.x * -1;
            edgeHit = true;
            //} else if (transform.position.x > 7.15)
        }
        else if (transform.position.x > 6.60)
        {
            directionX = lastDirection.x * -1;
            transform.position = new Vector3(6.50f, transform.position.y, 0);
            edgeHit = true;
        }

        //if (transform.position.y > 3.74)
        if (transform.position.y > 3.45)
        {
            directionY = lastDirection.y * -1;
            edgeHit = true;
            transform.position = new Vector3(transform.position.x, 3.40f, 0);
        }
        //if (transform.position.y < -4.99)
        if (transform.position.y < -4.60)
        {
            directionY = lastDirection.y * -1;
            edgeHit = true;
            transform.position = new Vector3(transform.position.x, -4.50f, 0);
        }
        if (edgeHit)
        {
            lastDirection = new Vector3(directionX, directionY, 0);
            lastDirection.Normalize();
            GetComponent<Rigidbody2D>().AddForce(lastDirection * speed, ForceMode2D.Force);
            return;
        } else
        {
            GetComponent<Rigidbody2D>().AddForce(lastDirection * speed, ForceMode2D.Force);
        }
    }

    public void KickOff()
    {
        transform.position = startingPosition;
    }

	public void Kick(Vector3 direction, float force) {
        lastPosition = transform.position;
        Vector3 noise = new Vector3(0, Random.Range(-0.15f, 0.15f), 0);
        lastDirection = direction;
        lastDirection.Normalize();
        lastDirection = lastDirection + noise;
        speed = force;
        // add noise 20% noise kick
        //print(string.Format("ball is being kicked direction : {0}", lastDirection, force));
        GetComponent<Rigidbody2D>().AddForce(lastDirection * speed, ForceMode2D.Force);
    }

    public void Dribble(Vector3 direction, float force)
    {
        lastPosition = transform.position;
        lastDirection = direction;
        lastDirection.Normalize();
        speed = force;
        print(string.Format("ball is being kicked direction : {0}", direction, force));
        GetComponent<Rigidbody2D>().AddForce(lastDirection * speed, ForceMode2D.Force);
    }

    public void Freeze()
    {
        lastPosition = transform.position;
        lastDirection = Vector3.zero;
        speed = 0;
    }
}
