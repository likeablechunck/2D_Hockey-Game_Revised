using UnityEngine;
using System.Collections;

public class AISteering : MonoBehaviour {

    public string state = "";
    Vector3 moveDirection;
    float moveSpeed;
    Puck targetPuck;
    Vector3 targetPosition;
    float seekSpeed;
    float pursuitSpeed;
    Player player;

    void Start()
    {
        player = gameObject.GetComponent<Player>();
    }

	
	// Update is called once per frame
	void Update () {
        //print(string.Format("{0} steering state is {1}", this.name, state));
        if (state.Equals("arrive"))
        {
            if (!IsAtTarget())
            {
                Arrive();
            }
        } else if (state.Equals("pursuitpuck"))
        {
            PursuitPuck();
        } else if (state.Equals("move"))
        {
            Move();
        }
    }


    public void Move()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        state = "";
    }

    public void MoveTo(Vector3 direction, float speed)
    {
        state = "move";
        moveDirection = direction;
        moveSpeed = speed;
    }

    public void Seek(Vector3 target, float speed)
    {
        // if the position we are moving to is very close to another existing player then
        // dont move there
        Vector3 direction = target - transform.position;
        direction.z = 0;
        float safeDistance = 0.05f;
        // if we are outside of our zone then dont move any more

        if (!player.InTheField(0.5f, 0.5f))
        {
            if (player.IsTooFarBelow(0.5f))
            {
                transform.Translate(Vector3.up * speed * Time.deltaTime);
            }
            if (player.IsTooFarLeft(0.5f))
            {
                transform.Translate(Vector3.right * speed * Time.deltaTime);
            }

            if (player.IsTooFarUp(0.5f))
            {
                transform.Translate(Vector3.down * speed * Time.deltaTime);
            }
            if (player.IsTooFarRight(0.5f))
            {
                transform.Translate(Vector3.left * speed * Time.deltaTime);
            }
            return;
        }
        if (direction.magnitude > safeDistance)
        {

            //print(string.Format("{0} lllll", direction.magnitude));
            Vector3 moveVector = direction.normalized * speed * Time.deltaTime;
            //print(string.Format("seek : {0} {1} {2} {3}", player.name, moveVector.x, moveVector.y, speed));
            if (!player.IsAnyOtherPlayerThere(transform.position + moveVector, 0.50))
            {
                //print(string.Format("seek : {0} {1} {2} {3}", player.name, moveVector.x, moveVector.y, speed));
                transform.Translate(moveVector);
            } else
            {
                // try +/- 0.50 on y axis to see if there is a way out
                if (!player.IsAnyOtherPlayerThere(transform.position + moveVector + new Vector3(0, 0.50f, 0), 0.50)) {
                    transform.Translate(moveVector + new Vector3(0, 0.10f, 0));
                }
                if (!player.IsAnyOtherPlayerThere(transform.position + moveVector + new Vector3(0, -0.50f, 0), 0.50)) {
                    transform.Translate(moveVector + new Vector3(0, -0.10f, 0));
                }
                // try to go around so 
            }
        } else
        {
            transform.position = target;
        }
    }

    void PursuitPuck()
    {
        // if another player is closer to puck then i should leave it
        // if puck is going to hit me in 1-5 seconds at the positon
        // i am at then just stay

        //Vector3 currentDistanceMagnitude = (transform.position - targetPuck.startingPosition).magnitude;
        Player closestPlayer = player.team.GetClosestPlayerToBallBothTeams();
        if (!closestPlayer.name.Equals(player.name) && (transform.position - targetPuck.transform.position).magnitude < 0.4) {
            Seek(Vector3.up, pursuitSpeed);
            return;
        }
        // if i am too close to the goal keeper then move back
        if (player.IsTooCloseToGoal())
        {
            if (player.team.goal.facing.x == -1)
            {
                Seek(Vector3.right, pursuitSpeed);
            } else
            {
                Seek(Vector3.left, pursuitSpeed);
            }
        }
        int iterationCounter = 1;

        Vector3 futurePosition = Vector3.zero;
        while (iterationCounter < 5)
        {
            futurePosition = (targetPuck.transform.position + targetPuck.instantVelocity * iterationCounter * Time.deltaTime);

            if (player.team.goal.facing.x == -1)
            {
                if (transform.position.x > futurePosition.x &&
                    (transform.position - futurePosition).magnitude < 2)
                {
                    //print(string.Format("{0} will go here {1}", this.name, futurePosition));

                    Seek(futurePosition, pursuitSpeed);
                    // then stay here
                    return;
                }
            } else
            {
                if (transform.position.x < futurePosition.x &&
                                (transform.position - futurePosition).magnitude < 2)
                {
                    //print(string.Format("{0} will go here {1}", this.name, futurePosition));
                    Seek(futurePosition, pursuitSpeed);
                    // then stay here
                    return;
                }
            }
            iterationCounter++;
        }

        Seek(futurePosition, pursuitSpeed);
        //int iterationAhead = 5;
        //Vector3 targetVelocity = targetPuck.instantVelocity;
        //Vector3 targetFuturePosition = targetPuck.transform.position + targetVelocity * iterationAhead;
        ////print(string.Format("Pursuit : {0}:{1}", targetFuturePosition, pursuitSpeed));
        //if (Mathf.Abs(transform.position.y - player.homeRegion.Center().y) < 3.5) {
        //    Seek(targetFuturePosition, pursuitSpeed);
        //}
    }



    public void ArriveOn(Vector3 position, float speed)
    {
        state = "arrive";
        targetPosition = position;
        seekSpeed = speed;
        //MonoBehaviour.print(string.Format("{0} will arriVe at {1}", name, targetPosition));
    }

    void Arrive()
    {
        Seek(targetPosition, seekSpeed);
        //if (Mathf.Abs(transform.position.y - player.homeRegion.Center().y) < 3.5)
        //{
        //    Seek(targetPosition, seekSpeed);
        //}
    }

    public void PursuitPuckOn(Puck target, float speed)
    {
        state = "pursuitpuck";
        targetPuck = target;
        pursuitSpeed = speed;
    }


    public void PursuitOff()
    {
        if (state == "pursuitpuck")
        {
            state = "";
        }
    }
    public void AbortAllMovement()
    {
        if (IsPursuitPuckOn())
        {
            PursuitOff();
        }

        if (IsArriveOn())
        {
            ArriveOff();
        }
        state = "";
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
        //print(string.Format("{0} aborted all moves!", this.name));
    }

    public bool IsPursuitPuckOn()
    {
        return state.Equals("pursuitpuck") ;
    }

    public bool IsArriveOn()
    {
        return state.Equals("arrive");
    }

    public void ArriveOff()
    {
        if (state == "arrive")
        {
            state = "";
        }
    }

    public bool IsAtTarget()
    {
        float atTargetDistance = 0.4f;
        if (state.Equals("arrive"))
        {
            if(transform.position == targetPosition)
            {
                //print(string.Format("{0} have reached target", name));
            }
            return transform.position == targetPosition;
            //return ((transform.position - targetPosition).magnitude < atTargetDistance);
            //return ((transform.position - targetPosition).magnitude < atTargetDistance);
        }
        else if (state.Equals("pursuitpuck"))
        {

            // it is important whether ball is in front of or behind
            if (player.team.goal.facing.x == -1)
            {
                return (transform.position.x > targetPuck.transform.position.x &&
                    (transform.position - targetPuck.transform.position).magnitude < atTargetDistance);
            } else
            {
                return (transform.position.x < targetPuck.transform.position.x && 
                    (transform.position - targetPuck.transform.position).magnitude < atTargetDistance);
            }
        }
        return false;
    }
}
