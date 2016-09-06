using UnityEngine;
using System.Collections;

public class Wait : AIState {

	public override void Execute(StatefulBehaviour behaviour) {
        Player player = (Player)behaviour;
        if (player.team.isManual && player.IsManualPlayer())
        {
            //do nothing
            return;
        }
        // if player is not yet at the target then keep going otherwise set arrive off
        if (player.steer.IsAtTarget())
        {
            player.steer.ArriveOff();
            player.velocity = new Vector3(0, 0, 0);
        }
        if (player.team.InControl() &&
            !player.IsControllingPlayer() &&
            player.IsAheadOfAttacker())
        {
            player.steer.AbortAllMovement();
            player.team.RequestPass(player);
            //but this means we should also sto pthe receiver and
            player.team.setReceiver(player);
            return;
        }
        // if we are too close to the goal we should head back or at least not move
        if (!player.IsManualPlayer() && !player.IsTooCloseToGoal() && player.InTheField(1.0f, 1.0f))
        {
            if (player.IsClosestTeamMemberToPuck())
            {
                player.team.setControllingPlayer(null);
                //MonoBehaviour.print(string.Format("{0} tadas", player.name));
                player.ChangeState(ScriptableObject.CreateInstance<ChasePuck>());
                return;
            }
            //else if (player.team.InControl())
            //{
            //    // at least make an effort and move to somewhere 
            //    // closer to the ball ( e.g move toward x or y direction at leaast)
            //Vector3 direction = Vector3.zero;
            //if (player.team.goal.facing.x == 1)
            //{
            //    direction = player.transform.position + Vector3.right;
            //    if (player.puck.lastDirection.x < 0)
            //    {
            //        direction = player.transform.position + Vector3.left;
            //    }
            //}
            //else
            //{
            //    direction = player.transform.position + Vector3.right;
            //    if (player.puck.lastDirection.x < 0)
            //    {
            //        direction = player.transform.position + Vector3.left;
            //    }
            //}
            //if (direction != Vector3.zero)
            //{
            //    player.steer.ArriveOn(direction, 1.0f);
            //}
        }
        //if (!player.InTheField(1.0f, 1.0f))
        //{
        //    player.steer.ArriveOn(player.homeRegion.Center(), 1.0f);
        //}
    }

	public override void Enter(StatefulBehaviour behaviour) {
        //Player player = (Player)behaviour;
        //if (player.puck.owner == this)
        //{
        //    //MonoBehaviour.print(string.Format("{0} tadas {1}", player.name));
        //    player.puck.owner = null;
        //}
    }
	public override void Exit(StatefulBehaviour behaviour) {
		return;
	}
}
