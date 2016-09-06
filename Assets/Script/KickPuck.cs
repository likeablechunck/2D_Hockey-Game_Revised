using UnityEngine;
using System.Collections;

public class KickPuck : AIState {

    public override void Exit(StatefulBehaviour behaviour)
    {
        return;
    }
    public override void Enter(StatefulBehaviour behaviour)
    {
        return;
    }

    public override void Execute(StatefulBehaviour behaviour)
    {
        Player player = (Player)behaviour;
        //if (player.team.receiver != null || player.pitch.GoalKeeperHasPuck())
        //{+
        //    player.ChangeState(ScriptableObject.CreateInstance<ChasePuck>());
        //}+
        float kickPower = 30.0f;
        float passPower = 20.0f;
        Vector3 goal = player.team.opponent.goal.center;
        if (!player.PuckWithinReceivingRange())
        {
            player.team.setControllingPlayer(null);
            player.steer.AbortAllMovement();
            player.ChangeState(ScriptableObject.CreateInstance<Wait>());
            return;
        }
        if (player.team.CanShoot(player.puck.transform.position, goal, kickPower))
        {
            Vector3 kickDirection = goal - player.puck.transform.position;
            player.team.setControllingPlayer(null);
            player.puck.Kick(kickDirection, 30.0f);
            player.steer.AbortAllMovement();
            player.ChangeState(ScriptableObject.CreateInstance<Wait>());
            return;
        }

        if (player.IsThreatened() && player.team.receiver != null)
        {

            //MonoBehaviour.print(player.team.receiver.name);
            //Vector3 kickDirection = player.puck.transform.position - player.team.receiver.GetTarget();
            Vector3 kickDirection = player.team.receiver.transform.position - player.puck.transform.position;
            //if (player.team.goal.facing.x == -1)
            //{
            //    //if receiver is behind me
            //    if (player.team.receiver.transform.position.x > puck.transform.position.x)
            //    {
            //        kickDirection = player.team.receiver.transform.position - puck.transform.position
            //        + new Vector3(0.4f, 0, 0);
            //    } else
            //    {
            //        kickDirection = player.team.receiver.transform.position - puck.transform.position
            //        - new Vector3(0.1f, 0, 0);
            //    }
            //} else
            //{

            //    //if receiver is behind me
            //    if (player.team.receiver.transform.position.x < puck.transform.position.x)
            //    {
            //        kickDirection = player.team.receiver.transform.position - puck.transform.position
            //        - new Vector3(0.4f, 0, 0);
            //    }
            //    else
            //    {
            //        kickDirection = player.team.receiver.transform.position - puck.transform.position
            //        + new Vector3(0.4f, 0, 0);
            //    }

            //}
            player.team.setControllingPlayer(player);
            MonoBehaviour.print(string.Format("passing the ball to {0}", player.team.receiver.name));
            player.puck.Kick(kickDirection, passPower);
            player.steer.AbortAllMovement();
            player.team.receiver.ChangeState(ScriptableObject.CreateInstance<ReceivePuck>());
            player.ChangeState(ScriptableObject.CreateInstance<Wait>());
            //player.ChangeState(ScriptableObject.CreateInstance<SupportAttacker>());
            //Player supportme = player.FindSupport();
            //supportme.ChangeState(ScriptableObject.CreateInstance<SupportAttacker>());
        }
        else
        {
            player.steer.AbortAllMovement();
            Player supportme = player.FindSupport();
            if (player.team.supportingPlayer == supportme)
            {
                supportme.steer.ArriveOn(player.team.DetermineBestSupportingPosition(), 3.0f);
            }
            supportme.ChangeState(ScriptableObject.CreateInstance<SupportAttacker>());
            player.ChangeState(ScriptableObject.CreateInstance<MoveAlong>());
        }
    }
}
