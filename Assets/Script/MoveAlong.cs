using UnityEngine;
using System.Collections;

public class MoveAlong : AIState {


    float maxDribbleForce = 5.0f;

    public override void Execute(StatefulBehaviour behaviour)
    {
        Player player = (Player)behaviour;
        //if (player.Heading().x != player.team.goal.Facing().x)
        //{
        //    // we need to rotate the object
        //    double kickingForce = 0.5;
        //    Vector3 direction = player.Heading();
        //    player.puck.Kick(direction, kickingForce);
        //} else
        //{
        //    player.puck.Kick(player.team.goal.Facing(), maxDribbleForce);
        //}
        //MonoBehaviour.print("moving along");
        //if we are close to right post then move toward that
        // or move toward left
        Vector3 dir = new Vector3(0, 0, 0);
        float leftPostDistance = (player.transform.position - player.team.opponent.goal.leftPost).magnitude;
        float rightPostDistance = (player.transform.position - player.team.opponent.goal.rightPost).magnitude;
        if (leftPostDistance < rightPostDistance)
        {
            dir = player.team.opponent.goal.leftPost - player.transform.position;
        } else
        {
            dir = player.team.opponent.goal.rightPost - player.transform.position;
        }
        player.puck.Kick(dir, maxDribbleForce);
        Player supportme = player.FindSupport();
        supportme.ChangeState(ScriptableObject.CreateInstance<SupportAttacker>());
        player.ChangeState(ScriptableObject.CreateInstance<ChasePuck>());
        return;
    }
    public override void Enter(StatefulBehaviour behaviour)
    {
        Player player = (Player)behaviour;
        player.team.setControllingPlayer(player);
    }
    public override void Exit(StatefulBehaviour behaviour)
    {
        return;
    }
}
