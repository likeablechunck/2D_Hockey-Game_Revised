using UnityEngine;
using System.Collections;

public class ChasePuck : AIState {


	public override void Enter(StatefulBehaviour behaviour) {
		Player player = (Player)behaviour;
        player.steer.PursuitPuckOn(player.puck, 2.0f);
	}
	
	public override void Exit(StatefulBehaviour behaviour) {
        return;
	}
	
	public override void Execute(StatefulBehaviour behaviour) {
        Player player = (Player)behaviour;
        if(player.team.isManual && player.IsManualPlayer())
        {
            MonoBehaviour.print(string.Format("doing nothing {0}", player.name));
            return;
        }
        if (player.PuckWithinReceivingRange())
        {

            if (!player.team.isManual)
            {
                player.puck.owner = player; 
                player.ChangeState(ScriptableObject.CreateInstance<KickPuck>());
                player.steer.AbortAllMovement();
                player.team.setControllingPlayer(player);
            } else
            {
                //MonoBehaviour.print(string.Format("manual switch {0}", player.name));
                player.puck.owner = player;
                player.steer.AbortAllMovement();
                player.team.setManualPlayer(player);

            }
            return;
        }
        if (player.IsClosestTeamMemberToPuck())
        {
            //MonoBehaviour.print(string.Format("{0} kind of close {1}",
            // if its too close to goal keeper then dont go there
            if (!player.IsTooCloseToGoal())
            {
                //MonoBehaviour.print(string.Format("{0} need to go behind the ball - {1}", player.name,
                //    player.steer.state));
                if (!player.steer.IsPursuitPuckOn())
                {
                    player.steer.PursuitPuckOn(player.puck, 2.0f);
                }
            } else
            {
                player.steer.AbortAllMovement();
            }
            
            return;
        }
        // i dont have the ball anymore if i had it so lets wait
        player.ChangeState(ScriptableObject.CreateInstance<Wait>());
        // in this case whoever was supporting me should also change to wait
        if (player.team.supportingPlayer)
        {
            player.team.setSupprotingPlayer(null);
        }

    }
}
