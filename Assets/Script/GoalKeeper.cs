using UnityEngine;
using System.Collections;

public class GoalKeeper : AIState {

    // Use this for initialization

    public override void Enter(StatefulBehaviour behaviour)
    {
    }
    public override void Execute(StatefulBehaviour behaviour)
    {
        // -1.16 0.61
        // go to right or left to follow the ball
        // 
        float fast = 2.0f;
        Player player = (Player)behaviour;

        //if (player.steer.IsArriveOn())
        //{
        //    if(player.steer.IsAtTarget())
        //    {
        //        player.steer.AbortAllMovement();
        //    }
        //}
        // if ball is moving too fast then we should also move faster
        if (player.puck.GetComponent<Rigidbody2D>().velocity.x > 1)
        {
            fast = 4.0f;
        }
        
        if (player.puck.transform.position.y > player.transform.position.y &&
            player.transform.position.y < player.team.goal.leftPost.y)
        {
            //MonoBehaviour.print(string.Format("{0} goal keeper moving up", player.name));
            player.transform.Translate(Vector3.up * Time.deltaTime * fast);
        }
        if (player.puck.transform.position.y < player.transform.position.y &&
            player.transform.position.y > player.team.goal.rightPost.y)
        {
            //MonoBehaviour.print(string.Format("{0} goal keeper moving down", player.name));
            player.transform.Translate(Vector3.down * Time.deltaTime * fast);
        }
        // what if some how we moved out of the zone
        if (player.transform.position.y > player.team.goal.rightPost.y)
        {
            player.transform.Translate(Vector3.down * Time.deltaTime * fast);
        }

        if (player.transform.position.y < player.team.goal.leftPost.y) {
            player.transform.Translate(Vector3.up * Time.deltaTime * fast);
        }

        if (player.PuckWithinGoalKeeperRange())
        {
            // i should pass the ball to someone in the team
            // and become controlling players until someonen receives the puck
            player.team.setControllingPlayer(player);
            if(player.team.supportingPlayer == null)
            {
                Player support = player.FindSupport();
                player.team.setSupprotingPlayer(support);
            }
            Vector3 kickDirection = player.team.supportingPlayer.transform.position - 
                player.transform.position;
            player.puck.Kick(kickDirection, 20.0f);
            player.puck.owner = player;
            if (player.team.receiver != null)
            {
                player.team.supportingPlayer.ChangeState(ScriptableObject.CreateInstance<ReceivePuck>());
            }
            //MonoBehaviour.print(string.Format("gk kicked the ball to {0}", player.team.supportingPlayer.name));
            return;
        }
    }
    public override void Exit(StatefulBehaviour behaviour)
    {
    }

}
