using UnityEngine;
using System.Collections;

public class StatefulBehaviour : MonoBehaviour {

	AIState prevState = null;
	AIState currState = null;
    public string statename;
    public Vector3 lastPosition;
    public Vector3 instantVelocity;


	void Start() {
	}

	void Update() {
        instantVelocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
        if (currState != null)
        {
            //print(string.Format("state {0} whois {1}", currState, name));
            currState.Execute(this);
        }
	}

	public void ChangeState(AIState newState) {
        statename = newState.GetType().ToString();
        if (this.GetType() == typeof(Player))
        {
            ((Player)this).textmesh.text = ((Player)this).name + ":" + statename;
        }
        if (prevState != null) {
            prevState.Exit(this);
		}
        if (newState != null) {
            //if (name.Equals("blueTeamfw2"))
            //{
            //    print(string.Format("changing state from {0} to {1} whois {2}", prevState, newState, name));
            //}
            //if (this.GetType() == typeof(Player) && ((Player)this).goalKeeper)
            //{
            //    print(string.Format("changing state from {0} to {1} whois {2}", prevState, newState, name));
            //}
                if (prevState == null || newState.GetType() != prevState.GetType())
            {
                prevState = currState;
                currState = newState;
                currState.Enter(this);
            }
		}
		return;
	}
}
