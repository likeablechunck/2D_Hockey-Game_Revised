using UnityEngine;
using System.Collections;

public class ManualPlayer : AIState {

    public override void Enter(StatefulBehaviour behaviour) {
        Player player = (Player)behaviour;
        player.GetComponent<SpriteRenderer>().color = Color.yellow;

        if (player.steer.IsPursuitPuckOn())
        {
            player.steer.AbortAllMovement();
        }
    }

    public override void Exit(StatefulBehaviour behaviour)
    {
        Player player = (Player)behaviour;
        player.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public override void Execute(StatefulBehaviour behaviour)
    {
        // cant have two manual players
        Player player = (Player)behaviour;

        if (player.steer.IsAtTarget())
        {
            player.steer.AbortAllMovement();
        }

        if (!player.IsManualPlayer()) {
            player.ChangeState(ScriptableObject.CreateInstance<Wait>());
        }
        
        float walkingSpeed = 4.0f;
        if (player.PuckWithinReceivingRange())
        {
            player.puck.Freeze();
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            //move it up
            if (player.IsTooFarUp(0.5f))
            {
                if (player.PuckWithinReceivingRange())
                {
                    player.puck.owner = player;
                    player.puck.transform.Translate(Vector3.down * Time.deltaTime * walkingSpeed * 2);
                    player.puck.Freeze();
                }
                player.transform.Translate(Vector3.down * Time.deltaTime * walkingSpeed);
            }
            else {
                if (player.PuckWithinReceivingRange())
                {
                    player.puck.owner = player;
                    player.puck.transform.Translate(Vector3.up * Time.deltaTime * walkingSpeed * 2);
                    player.puck.Freeze();
                }
                player.transform.Translate(Vector3.up * Time.deltaTime * walkingSpeed);
            }
            return;
        }


        if (Input.GetKey(KeyCode.RightArrow))
        {
            //move it to the right
            if (player.IsTooFarRight(0.5f))
            {
                if (player.PuckWithinReceivingRange())
                {
                    player.puck.owner = player;
                    player.puck.transform.Translate(Vector3.left * Time.deltaTime * walkingSpeed * 2);
                    player.puck.Freeze();
                }
                player.transform.Translate(Vector3.left * Time.deltaTime * walkingSpeed);
            }
            else {
                if (player.PuckWithinReceivingRange())
                {
                    player.puck.owner = player;
                    player.puck.transform.Translate(Vector3.right * Time.deltaTime * walkingSpeed * 2);
                    player.puck.Freeze();
                }
                player.transform.Translate(Vector3.right * Time.deltaTime * walkingSpeed);
            }
            return;
        }

    
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //move it to left
            if (player.IsTooFarLeft(0.5f))
            {
                if (player.PuckWithinReceivingRange()) {
                    player.puck.owner = player;
                    player.puck.transform.Translate(Vector3.right * Time.deltaTime * walkingSpeed * 2);
                    player.puck.Freeze();
                }
                player.transform.Translate(Vector3.right * Time.deltaTime * walkingSpeed);
            }
            else {
                if (player.PuckWithinReceivingRange())
                {
                    player.puck.owner = player;
                    player.puck.transform.Translate(Vector3.left * Time.deltaTime * walkingSpeed * 2);
                    player.puck.Freeze();
                }
                player.transform.Translate(Vector3.left * Time.deltaTime * walkingSpeed);
            }
            return;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            //move it to the right
            if (player.IsTooFarBelow(0.5f))
            {
                if (player.PuckWithinReceivingRange())
                {
                    player.puck.owner = player;
                    player.puck.transform.Translate(Vector3.up * Time.deltaTime * walkingSpeed * 2);
                    player.puck.Freeze();
                }
                player.transform.Translate(Vector3.up * Time.deltaTime * walkingSpeed);
            }
            else {
                if (player.PuckWithinReceivingRange())
                {
                    player.puck.owner = player;
                    player.puck.transform.Translate(Vector3.down * Time.deltaTime * walkingSpeed * 2);
                    player.puck.Freeze();
                }
                player.transform.Translate(Vector3.down * Time.deltaTime * walkingSpeed);
            }
            return;
        }
        // if space is hit then shoot
        // if p is hit then pass
        // if enter is hit then shoot
            if (Input.GetKey(KeyCode.Return))
            {
                if (player.PuckWithinReceivingRange()) {
                    player.puck.Freeze();
                    player.puck.owner = player;
                    player.puck.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
                    Vector3 kickDirection = player.team.opponent.goal.center - player.puck.transform.position;
                    player.puck.Kick(kickDirection, 40.0f);
                }
            }
            if (Input.GetKey(KeyCode.P))
            {
                if (player.PuckWithinReceivingRange()) {
                    Player closestPlayer = player.FindClosestPlayerToMe();
                    closestPlayer.steer.AbortAllMovement();
                    player.team.setReceiver(closestPlayer);
                    player.puck.owner = player;
                    player.puck.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
                    Vector3 kickDirection = player.team.receiver.transform.position - player.puck.transform.position;
                    player.puck.Kick(kickDirection, 20.0f);
                    player.team.receiver.ChangeState(ScriptableObject.CreateInstance<ReceivePuck>());
                }
            }
    }
}
