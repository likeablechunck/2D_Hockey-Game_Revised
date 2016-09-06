using UnityEngine;
using System.Collections;

public class Team : StatefulBehaviour {

	public Team opponent;
	public Player receiver;
	public Player controllingPlayer;
    public Player supportingPlayer;
    public Player bestSupportingPlayer;
    public Player[] players;
    public FieldRegion[] defendingRegion;
    public FieldRegion[] startingRegion;
    public FieldRegion[] attackingRegion;
    public bool isManual;
    public Goal goal;
    Puck puck;
    public Pitch pitch;
    public int score = 0;
    public TextMesh scoreboard;

    public Color controllingColor;
    public Color supportingColor;
    public Color receivingColor;
    

    public float timeSinceLastSupportSpotCalculated = 0;
    SupportSpot bestSupportSpot = null;

    // Use this for initialization
    void Start () {
        if (name.Equals("blueTeam"))
        {
            opponent = GameObject.Find("redTeam").GetComponent<Team>();
        } else if (name.Equals("redTeam"))
        {
                opponent = GameObject.Find("blueTeam").GetComponent<Team>();
        }
        puck = GameObject.FindObjectOfType<Puck>();
        clearPlayerRolesAndColors();
    }


    public void setControllingPlayer(Player player)
    {
        if (controllingPlayer == player)
        {
            return;
        }
        if (controllingPlayer != null)
        {
            controllingPlayer.GetComponent<SpriteRenderer>().color = Color.white;
        }
        controllingPlayer = player;
        if(player!= null)
        {
            controllingPlayer.GetComponent<SpriteRenderer>().color = controllingColor;
        }
    }

    public void setSupprotingPlayer(Player player)
    {
        if (supportingPlayer == player)
        {
            return;
        }
        else {
            if (supportingPlayer != null)
            {
                supportingPlayer.GetComponent<SpriteRenderer>().color = Color.white;
                // the previous player which was in support attacker state should now
                // move back to wait
                if(!supportingPlayer.goalKeeper)
                supportingPlayer.ChangeState(ScriptableObject.CreateInstance<Wait>());
            }
            supportingPlayer = player;
            if (player != null)
            {
                supportingPlayer.GetComponent<SpriteRenderer>().color = supportingColor;
            }
        }
        // if i am supporting player now then i should wait where i am
    }

    public void setReceiver(Player player)
    {
        if (receiver == player)
        {
            return;
        }
        if (receiver != null)
        {
            receiver.GetComponent<SpriteRenderer>().color = Color.white;
            receiver.ChangeState(ScriptableObject.CreateInstance<Wait>());
        }
        receiver = player;
        if (player != null)
        {
            receiver.GetComponent<SpriteRenderer>().color = receivingColor;
        }
    }

    public void setManualPlayer(Player newManualPlayer)
    {
        Player prevManual = null;
        foreach (Player player in players)
        {
            if (player.statename.ToLower().Contains("manual"))
            {
                prevManual = player;
                break;
            }
        }
        if (prevManual == newManualPlayer)
        {
            return;
        }
        if (prevManual != null)
        {
            prevManual.GetComponent<SpriteRenderer>().color = Color.white;
            prevManual.ChangeState(ScriptableObject.CreateInstance<Wait>());
        }
        if (newManualPlayer != null)
        {
            newManualPlayer.GetComponent<SpriteRenderer>().color = receivingColor;
            newManualPlayer.ChangeState(ScriptableObject.CreateInstance<ManualPlayer>());
        }
    }


    public void clearPlayerRolesAndColors()
    {
        receiver = null;
        controllingPlayer = null;
        supportingPlayer = null;
        bestSupportingPlayer = null;
        foreach (Player player in players)
        {
            player.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    public void SetPlayersHomeRegions() {
        if (players != null)
        {
            int i = 0;
            while (i < players.Length)
            {
                players[i].homeRegion = defendingRegion[i];
                i++;
            }
        }
    }

    public void positionPlayersAt(FieldRegion[] regions)
    {
        if (players != null)
        {
            int i = 0;
            while (i < players.Length)
            {
                players[i].transform.position = regions[i].Center();
                i++;
            }
        }
    }

	public bool CanShoot(Vector3 from,  Vector3 target, double power) {
        float xDistanceFromTarget = Mathf.Abs(from.x - target.x);
        return xDistanceFromTarget > 0.5 && xDistanceFromTarget < 2.5;
	} 



    public bool IsPassSafeFromAllOpponents(Vector3 playerPosition, Vector3 passDestination,double force)
    {
        return true;
    }

	public bool InControl() {
        if (puck.owner == null)
        {
            return false;
        }

        return puck.owner.name.StartsWith(name);
	}

	public Vector3 DetermineBestSupportingPosition() {
        // do this every 5 frames
        if (timeSinceLastSupportSpotCalculated < 10 && bestSupportSpot != null)
        {
            return bestSupportSpot.position;
        }
        // there will be many spots between goals
        ArrayList supportSpots = new ArrayList();
        for (float x = pitch.leftTop.x + 2.10f; x < pitch.leftTop.x + pitch.width; x = x + 0.3f)
        {
            for (float y = pitch.leftTop.y; y > pitch.leftTop.y - pitch.height; y = y - 0.3f)
            {
                SupportSpot sp = new SupportSpot();
                sp.position = new Vector3(x, y, 0);
                sp.score = 0;
                supportSpots.Add(sp);
            }
        }
        bestSupportSpot = null;
        double bestScoreSoFar = 0.0d;
        double maxPassingForce = 10.0d;
        double maxShootingForce = 10.0d;
        //print(controllingPlayer.name);
        foreach (SupportSpot spot in supportSpots)
        {
            spot.score = 0;
            if (IsPassSafeFromAllOpponents(spot.position,
                spot.position, maxPassingForce))
            {
                spot.score += 1;
            }
            if (CanShoot(spot.position,
                opponent.goal.center, maxShootingForce))
            {
                //print(string.Format("can shoot from {0}", spot.position));
                spot.score += 4;
            }
            if (controllingPlayer)
            {
                float optimalDistance = 3.0f;
                float miniumumDistance = 1.0f;
                float distanceFromControllingPlayer = 
                    (controllingPlayer.transform.position - spot.position).magnitude;
                float deltaWithOptimalDistance = Mathf.Abs(optimalDistance - distanceFromControllingPlayer);
                
                if (deltaWithOptimalDistance < optimalDistance && distanceFromControllingPlayer > miniumumDistance)
                {
                    float scoreFactor = distanceFromControllingPlayer / optimalDistance;
                    spot.score += 4 * scoreFactor;
                }
            }
            if (spot.score > bestScoreSoFar)
            {
                bestSupportSpot = spot;
                bestScoreSoFar = spot.score;
            }
        }

        //print(string.Format("position: {0} score :{1}", bestSupportSpot.position, bestSupportSpot.score));
        return bestSupportSpot.position;
	}
    public bool IsOpponentWithinRadius(Player player, double radius) {
        Player[] opponents = opponent.players;
        for (int i = 0; i < opponents.Length; i++)
        {
            if (goal.facing.x == 1 &&
                opponent.transform.position.x > transform.position.x &&
                (transform.position - opponent.transform.position).magnitude < radius)
            {
                //print(string.Format("{0} is threatened", name));
                return true;
            }
            if (goal.facing.x == -1 &&
                opponent.transform.position.x < transform.position.x &&
                (transform.position - opponent.transform.position).magnitude < radius)
            {
                //print(string.Format("{0} is threatened", name));
                return true;
            }
        }
        return false;
	}

	public void RequestPass(Player player) {
        if (controllingPlayer != null && !controllingPlayer.goalKeeper)
        {
            controllingPlayer.ChangeState(ScriptableObject.CreateInstance<KickPuck>());
        }
    }

    public Player GetClosestPlayerToBallBothTeams()
    {
        Player closest = null;
        float minMagnitude = Mathf.Infinity;
        foreach (Player player in players)
        {
            if (player.goalKeeper)
            {
                continue;
            }
            Vector3 direction = player.transform.position - puck.transform.position;
            if (direction.magnitude < minMagnitude)
            {
                closest = player;
                minMagnitude = direction.magnitude;
            }
        }
        foreach (Player player in opponent.players)
        {
            if (player.goalKeeper)
            {
                continue;
            }
            Vector3 direction = player.transform.position - puck.transform.position;
            if (direction.magnitude < minMagnitude)
            {
                closest = player;
                minMagnitude = direction.magnitude;
            }
        }
        return closest;
    }

    public Player GetClosestPlayerToBall()
    {
        Player closest = null;
        float minMagnitude = Mathf.Infinity;
        foreach (Player player in players)
        {
            if(player.goalKeeper)
            {
                continue;
            }
            Vector3 direction = player.transform.position - puck.transform.position;
            if (direction.magnitude < minMagnitude)
            {
                closest = player;
                minMagnitude = direction.magnitude;
            }
        }
        return closest;
    }

    public bool CanPass(Player from, out Player receiver, out Vector3 puckTarget, double power, int minPassDistance)
    {
        receiver = null;
        puckTarget = new Vector3();
        return true;
    }

    public void UpdateTargetsOfWaitingPlayers(FieldRegion[] regions)
    {
        // region [1,1] is for goal keeper
        // region [0,2] [ 1,2] , [2,2] and [1,1] are for the players
        //print(string.Format("moving players to their home regions {0}", name));
        if(players != null)
        {
            int i = 0;
            while (i < players.Length)
            {
                if (!players[i].goalKeeper) 
                {
                    {
                        players[i].ChangeState(ScriptableObject.CreateInstance<Wait>());
                    }
                }
                i++;
            }
        }
    }
}
