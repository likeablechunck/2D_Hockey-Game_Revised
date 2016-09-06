using UnityEngine;
using System.Collections;

public abstract class AIState : ScriptableObject
{
	public virtual void Execute(StatefulBehaviour behaviour) {
		return;
	}
	public virtual void Enter(StatefulBehaviour behaviour) {
		return;
	}
	public virtual void Exit(StatefulBehaviour behaviour) {
		return;
	}
}
