using UnityEngine;
using System.Collections;

public class Attack : AIState {

    public override void Enter(StatefulBehaviour behaviour) {
		Team team = (Team)behaviour;
        team.UpdateTargetsOfWaitingPlayers(team.attackingRegion);
    }

	public override void Exit(StatefulBehaviour behaviour) {
		return;
	}

	public override void Execute(StatefulBehaviour behaviour) {
        
		//Team team = (Team)behaviour;team.timeSinceLastSupportSpotCalculated++;
  //      if (team.timeSinceLastSupportSpotCalculated > 10)
  //      {
  //          team.timeSinceLastSupportSpotCalculated = 0;
  //      }
  //      if (!team.InControl ()) {
		//	team.ChangeState(ScriptableObject.CreateInstance<Defending>());
  //          return;
		//}
    }
}
