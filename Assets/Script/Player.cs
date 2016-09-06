using UnityEngine;
using System.Collections;

public class Player : StatefulBehaviour {
	public Team team;
	public Puck puck;
	public Pitch pitch;
	public Vector3 velocity;
    public string teamName;
    public FieldRegion homeRegion;
    public bool goalKeeper = false;
    public AISteering steer;
    

    public TextMesh textmesh;
	
	public bool IsControllingPlayer() {
        return team.controllingPlayer == this;
	}
    public bool IsManualPlayer()
    {
        Player manualPlayer = null;
        int i = 0;
        foreach (Player player in team.players)
        {
            if (player.statename.ToLower().Contains("manual"))
            {
                manualPlayer = player;
                i++; 
                
            }
        }
        if (i>1)
        {
            print("we have two players!!!!");
        }
        return manualPlayer == this;
    }

    public bool IsAheadOfAttacker() {
        if (team.goal.facing.x == 1)
        {
            // this is the left team attacking
            if (team.controllingPlayer != null && transform.position.x > team.controllingPlayer.transform.position.x)
            {
                return true;
            }
        } else
        {
            if (team.controllingPlayer != null && transform.position.x < team.controllingPlayer.transform.position.x)
            {
                return true;
            }
        }
        return false;
	}

	public bool IsClosestTeamMemberToPuck() {
        Player closest = team.GetClosestPlayerToBall();
        //MonoBehaviour.print(string.Format("closest member to puck is {0}", closest.name));
        return closest.name.Equals(name);
    }


    public bool IsTooCloseToGoal()
    {
        Vector3 current = transform.position;
        if (current.y < team.goal.leftPost.y || current.y > team.goal.rightPost.y) {
            return current.x < team.goal.leftPost.x + 0.65 && current.x > team.goal.leftPost.x - 0.25;
        }
        return false;
    }

    public bool InTheField(float minXDistance, float minYDistance)
    {
        return Mathf.Abs(transform.position.x - pitch.leftTop.x) > minXDistance &&
            minXDistance < Mathf.Abs(pitch.leftTop.x + pitch.width - transform.position.x) &&
            minYDistance < Mathf.Abs(pitch.leftTop.y - transform.position.y) &&
            Mathf.Abs(transform.position.y - (pitch.leftTop.y - pitch.height)) > minYDistance;
    }

    public bool IsTooFarLeft(float minDistance)
    {
        return transform.position.x < pitch.leftTop.x ||
             transform.position.x - pitch.leftTop.x < minDistance;
    }

    public bool IsTooFarUp(float minDistance)
    {
        return transform.position.y > pitch.leftTop.y ||
            pitch.leftTop.y - transform.position.y < minDistance;
    }


    public bool IsTooFarBelow(float minDistance)
    {
        return transform.position.y < pitch.leftTop.y - pitch.height ||
            transform.position.y - (pitch.leftTop.y - pitch.height) < minDistance;
    }
    public bool IsTooFarRight(float minDistance)
    {
        return transform.position.x > pitch.leftTop.x + pitch.width ||
             pitch.leftTop.x + pitch.width - transform.position.x < minDistance;
    }




    public Player FindClosestPlayerToMe()
    {
        float minSupportDistance = Mathf.Infinity;
        Player closestPlayer = null;
        if (team.players != null)
        {
            foreach (Player player in team.players)
            {
                if (player == this || player.goalKeeper)
                {
                    continue;
                }
                if ((player.transform.position - transform.position).magnitude < minSupportDistance)
                {
                    closestPlayer = player;
                    minSupportDistance = (player.transform.position - transform.position).magnitude;
                }
            }
        }
        return closestPlayer;
    }

    public Player FindSupport()
    {
        Vector3 bestSupportingSpot = team.DetermineBestSupportingPosition();
        float minSupportDistance = Mathf.Infinity;
        Player closestPlayer = null;
        if (team.players != null)
        {
            foreach (Player player in team.players)
            {
                if (player == this || player.goalKeeper)
                {
                    continue;
                }
                if ((player.transform.position - bestSupportingSpot).magnitude < minSupportDistance)
                {
                    closestPlayer = player;
                    minSupportDistance = (player.transform.position - bestSupportingSpot).magnitude;
                }
            }
        }
        return closestPlayer;
    }

    public bool IsThreatened()
    {
        // if any of the other players are within distance of 1 from him
        Player[] opponents = team.opponent.players;
        foreach(Player opponent in opponents)
        {
            // is the other player in front of him ?
            if (team.goal.facing.x == 1 && 
                opponent.transform.position.x > transform.position.x &&
                (transform.position - opponent.transform.position).magnitude < 2)
            {
                //print(string.Format("{0} is threatened", name));
                return true;
            }
            if (team.goal.facing.x == -1 &&
                opponent.transform.position.x < transform.position.x &&
                (transform.position - opponent.transform.position).magnitude < 2)
            {
                //print(string.Format("{0} is threatened", name));
                return true;
            }
        }
        return false;
    }

    public bool PuckWithinGoalKeeperRange()
    {
        if (team.goal.facing.x == -1)
        {
            //return transform.position.x > puck.transform.position.x
            //    //&& transform.position.x - puck.transform.position.x <= 0.395 
            //    && transform.position.x - puck.transform.position.x <= 0.405
            //    && transform.position.y - puck.transform.position.y <= 0.48;
            ////&& transform.position.y - puck.transform.position.y <= 0.28;
            return (transform.position.x > puck.transform.position.x &&
                (transform.position - puck.transform.position).magnitude < 0.4);
        }
        else
        {
            //return transform.position.x < puck.transform.position.x
            //    //&& puck.transform.position.x - transform.position.x == 0.227
            //    && puck.transform.position.x - transform.position.x < 0.300
            //    && puck.transform.position.y < transform.position.y
            //    && transform.position.y - puck.transform.position.y <= 0.48;
            ////&& transform.position.y - puck.transform.position.y <= 0.28;
            return (transform.position.x < puck.transform.position.x &&
                (transform.position - puck.transform.position).magnitude < 0.4);
        }
    }
    public bool PuckWithinReceivingRange()
    {        // if its left team then puck should be close and its x should be > player x
        // if its right team puck should be close and its x should be 
        // make sure x and y are very close

        if (team.goal.facing.x == -1)
        {
            //return transform.position.x > puck.transform.position.x
            //    //&& transform.position.x - puck.transform.position.x <= 0.395 
            //    && transform.position.x - puck.transform.position.x <= 0.405
            //    && transform.position.y - puck.transform.position.y <= 0.48;
            ////&& transform.position.y - puck.transform.position.y <= 0.28;
            return (transform.position.x > puck.transform.position.x &&
                (transform.position - puck.transform.position).magnitude < 0.4);
        }
        else
        {
            //return transform.position.x < puck.transform.position.x
            //    //&& puck.transform.position.x - transform.position.x == 0.227
            //    && puck.transform.position.x - transform.position.x < 0.300
            //    && puck.transform.position.y < transform.position.y
            //    && transform.position.y - puck.transform.position.y <= 0.48;
            ////&& transform.position.y - puck.transform.position.y <= 0.28;
            return (transform.position.x < puck.transform.position.x &&
                (transform.position - puck.transform.position).magnitude < 0.4);
        }
    }

    public bool IsAnyOtherPlayerThere(Vector3 position, double radius)
    {
        foreach (Player player in team.players)
        {
            if (!player.name.Equals(name) &&
                    (position - player.transform.position).magnitude < radius)
            {
                //print(string.Format("{0} is at {1}", player.name, position));
                return true;
            }
        }
        foreach (Player player in team.opponent.players)
        {
            if ((position - player.transform.position).magnitude < radius)
            {
                return true;
            }
        }
        return false;
    }
}
