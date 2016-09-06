using UnityEngine;
using System.Collections;

public class Defending : AIState {


	public override void Enter(StatefulBehaviour behaviour) {
		Team team = (Team)behaviour;
		team.UpdateTargetsOfWaitingPlayers(team.defendingRegion);
    }
	
	public override void Exit(StatefulBehaviour behaviour) {
		return;
	}
	
	public override void Execute(StatefulBehaviour behaviour) {
		//Team team = (Team)behaviour;
  //      if (team.timeSinceLastSupportSpotCalculated > 10)
  //      {
  //          team.timeSinceLastSupportSpotCalculated = 0;
  //      }
  //      if (team.InControl ()) {
		//	team.ChangeState(ScriptableObject.CreateInstance<Attack>());
  //          return;
  //      }
  //      return;
	}
}
