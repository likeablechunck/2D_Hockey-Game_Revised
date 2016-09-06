using UnityEngine;
using System.Collections;

public class Pitch : MonoBehaviour {

    public Vector3 leftTop;
    public float width;
    public float height;
    public Puck puck;
    Team rightTeam;
	Team leftTeam;
    public Goal leftGoal;
	public Goal rightGoal;
	FieldRegion[][] regions;
    int periodTimeLength = 5 * 60;
    int timePassed = 0;
  

    // Use this for initialization
    void Start () {

        // the left goal will be in center of x = leftTop +1 and y = leftTop.y + height/2
        // the right goal will be in x = leftTop + width - 1 and y = leftTop.y + height/2
        // somehow we have to assciate the teams here or just create them here ? 
	
	}
	
	// Update is called once per frame
	void Update () {
        timePassed++;
	}

    public void StartPeriod(Team leftTeam, Team rightTeam)
    {
        this.leftTeam = leftTeam;
        this.rightTeam = rightTeam;
        leftGoal = GameObject.Find("leftGoal").GetComponent<Goal>();
        rightGoal = GameObject.Find("rightGoal").GetComponent<Goal>();
        this.rightTeam.goal = rightGoal;
        this.leftTeam.goal = leftGoal;
        this.rightTeam.attackingRegion = RightTeamAttackingPlayerRegions();
        this.rightTeam.defendingRegion = RightTeamDefendingRegions();
        this.rightTeam.startingRegion = RightTeamStartingRegions();
        this.leftTeam.startingRegion = LeftTeamStartingRegions();
        this.leftTeam.defendingRegion = LeftTeamDefendingPlayerRegions();
        this.leftTeam.attackingRegion = LeftTeamAttackingPlayerRegions();
        leftTeam.SetPlayersHomeRegions();
        rightTeam.SetPlayersHomeRegions();
    }

    /// <summary>
    /// 
    ///  y ------------->
    ///  x   *  0   1   2
    ///  |   0  x   x   x
    ///  |   1  x   x   x
    ///  V   2  x   x   x
    /// </summary>
    /// <returns></returns>


    FieldRegion[] LeftTeamStartingRegions()
    {
        FieldRegion[,] regions = leftTeamRegions();
        return new FieldRegion[] { regions[1, 0], regions[1, 1],regions[0, 2],
            regions[1, 2], regions[2, 2]};
    }

    FieldRegion[] RightTeamStartingRegions()
    {
        FieldRegion[,] regions = rightTeamRegions();
        //MonoBehaviour.print(regions[1, 0].Center());
        return new FieldRegion[] { regions[1, 0], regions[1, 1],regions[0, 2],
            regions[1, 2], regions[2, 2]};
    }

    FieldRegion[ ,] rightTeamRegions()
    {
        // right team region is counter clock wise
        FieldRegion[,] regions = new FieldRegion[3, 3];
        int i = 0;
        int j = 0;
        for (float y = leftTop.y - 1; y > leftTop.y - height; y = y - height/3.0f)
        {
            j = 0;
            for (float x = leftTop.x + width - 1.5f; x > leftTop.x + width/2; x = x - 2)
            {
                FieldRegion fr = new FieldRegion();
                fr.leftTop = new Vector3(x, y, 0);
                fr.width = 0.5f;
                fr.height = 0.5f;
                //print(string.Format("region[{0},{1}]: {2}", i, j, fr.leftTop));
                regions[i, j] = fr;
                j++;
                if (j == 3)
                {
                    break;
                }
            }
            i++;
            if (i == 3)
            {
                break;
            }
        }
        return regions;
    }

    FieldRegion[ , ] leftTeamRegions()
    {
        // left team regions are clockwize. goal keeper at [0,1]
        FieldRegion[,] regions = new FieldRegion[3, 3];
        int i = 0;
        int j = 0;
        for (float y = leftTop.y - 1.5f; y > leftTop.y - height; y = y - height / 3.0f)
        {
            j = 0;
            for (float x = leftTop.x + 1f; x < leftTop.x + width / 2; x = x + 2)
            {
                FieldRegion fr = new FieldRegion();
                fr.leftTop = new Vector3(x, y, 0);
                fr.width = 0.5f;
                fr.height = 0.5f;
                //print(string.Format("region[{0},{1}]: {2}", i, j, fr.leftTop));
                regions[i, j] = fr;
                j++;
                if (j == 3)
                {
                    break;
                }
            }
            i++;
            if (i == 3)
            {
                break;
            }
        }
        return regions;

    }

    /// <summary>
    /// 
    ///  y ------------->
    ///  x   *  0   1   2
    ///  |   0  x   x   x
    ///  |   1  x   x   x
    ///  V   2  x   x   x
    /// </summary>
    /// <returns></returns>

    FieldRegion[] LeftTeamDefendingPlayerRegions()
    {
        FieldRegion[,] regions = leftTeamRegions();
        return new FieldRegion[] { regions[1, 0], regions[0, 1],regions[2, 1],
            regions[0, 2], regions[2, 2]};
    }

    FieldRegion[] LeftTeamAttackingPlayerRegions()
    {
        FieldRegion[,] regions = leftTeamRegions();
        return new FieldRegion[] { regions[1, 0], regions[0, 2],regions[1, 2],
            regions[2, 2], regions[1, 1]};
    }

    FieldRegion[] RightTeamAttackingPlayerRegions()
    {
        FieldRegion[,] regions = leftTeamRegions();
        return new FieldRegion[] { regions[1, 0], regions[0, 2],regions[1, 2],
            regions[2, 2], regions[1, 1]};
    }



    FieldRegion[] RightTeamDefendingRegions()
    {
        FieldRegion[,] regions = rightTeamRegions();
        //goalkeeper wil be [1,1]
        //defender1 will be [0,2]
        //defender2 will be [0,2]
        //fw1 will be [0,2]
        //fw2 will be [0,2]
        return new FieldRegion[] { regions[1, 0], regions[0, 1],regions[2, 1],
            regions[0, 2], regions[2, 2]};

    }

    public bool PeriodInMotion() {
		// if period time is reached
        if (timePassed > periodTimeLength)
        {
            return false;
        }
        return true;
	}

	public bool GoalKeeperHasPuck() {
        if (leftTeam.players != null && rightTeam.players != null &&
            leftTeam.players.Length > 0 && rightTeam.players.Length > 0)
        {
            return leftTeam.controllingPlayer == leftTeam.players[0] ||
                rightTeam.controllingPlayer == rightTeam.players[0];
        } else
        {
            return false;
        }
	}

    public bool RightTeamScored(Vector3 puckPosition)
    {
        return puck.lastDirection.x < 0 &&
            puckPosition.y < leftGoal.leftPost.y && 
            puckPosition.y > leftGoal.rightPost.y &&
            puckPosition.x < leftGoal.leftPost.x 
            && puckPosition.x > leftGoal.leftPost.x - leftGoal.depth;
    }

    public bool LeftTeamScored(Vector3 puckPosition)
    {
        // if ball direction.x is also positive
        return puck.lastDirection.x > 0 && 
            puckPosition.y < rightGoal.leftPost.y &&
            puckPosition.y > rightGoal.rightPost.y &&
            puckPosition.x > rightGoal.leftPost.x
            && puckPosition.x < rightGoal.leftPost.x + rightGoal.depth;
    }
}
