using UnityEngine;
using System.Collections;

public class SupportAttacker : AIState {

    public override void Enter(StatefulBehaviour behaviour)
    {
        Player player = (Player)behaviour;
        Vector3 bestSupportingSpot = player.team.DetermineBestSupportingPosition();
        player.steer.ArriveOn(bestSupportingSpot, 3.0f);
        player.team.setSupprotingPlayer(player);
    }

    public override void Exit(StatefulBehaviour behaviour)
    {
        return;
    }

    public override void Execute(StatefulBehaviour behaviour)
    {
        Player player = (Player)behaviour;
        //MonoBehaviour.print(string.Format("im in support and not my steer state is {1} ", name, player.steer.state));
        if (player.steer.IsAtTarget())
        {
            player.steer.AbortAllMovement();
            player.team.setReceiver(player);
            // how about supporting player
            //player.team.setSupprotingPlayer(player);
            if (!player.IsThreatened())
            {
                player.team.RequestPass(player);
                return;
            }
        }
    }
}
