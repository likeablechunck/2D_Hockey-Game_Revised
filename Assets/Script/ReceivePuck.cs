using UnityEngine;
using System.Collections;

public class ReceivePuck : AIState {
	public override void Enter(StatefulBehaviour behaviour) {
        //MonoBehaviour.print("arrive enter");
        Player player = (Player)behaviour;
        if (player.team.isManual && player.IsManualPlayer())
        {
            // do nothing and wait for the ball
        }

        //player.team.setReceiver(player);
	}
	
	public override void Exit(StatefulBehaviour behaviour) {
        //Player player = (Player)behaviour;
        //player.team.setReceiver(null);
        //player.team.setSupprotingPlayer(null);
    }

    public override void Execute(StatefulBehaviour behaviour) {
        Player player = (Player)behaviour;
        //if (player.PuckWithinReceivingRange() || !player.team.InControl())
        //MonoBehaviour.print(string.Format("{0} arrive execute", player.name));
        if (player.PuckWithinGoalKeeperRange())
        {
            //if (player.PuckWithinReceivingRange () || !player.team.InControl ()) {
            //MonoBehaviour.print(string.Format("puck is with me {0} yoogoo", player.name));
            player.team.setReceiver(null);
            if (player.team.isManual)
            {
                // the current controlling player should switch back to wait
                //do nothing. you have the ball now so switch to manual control
                if(player.team.controllingPlayer != null && !player.team.controllingPlayer.goalKeeper)
                player.team.controllingPlayer.ChangeState(ScriptableObject.CreateInstance<Wait>());
                player.team.setControllingPlayer(player);
                player.ChangeState(ScriptableObject.CreateInstance<ManualPlayer>());
                player.puck.Freeze();
                player.steer.AbortAllMovement();

            }
            else {
                player.team.setControllingPlayer(player);
                Player supportme = player.FindSupport();
                supportme.ChangeState(ScriptableObject.CreateInstance<SupportAttacker>());
                player.ChangeState(ScriptableObject.CreateInstance<ChasePuck>());
            }
            return;
		}
        if (player.steer.IsAtTarget())
        {
            player.steer.AbortAllMovement();
        }
        else
        {
            player.steer.PursuitPuckOn(player.puck, 2.0f);
        }

        if (!player.team.InControl())
        {
            player.ChangeState(ScriptableObject.CreateInstance<Wait>());
        }
        return;
	}

}
