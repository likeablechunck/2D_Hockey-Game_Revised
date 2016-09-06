using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

    public Team redTeam;
    public Team blueTeam;
    int blueTeamScore = 0;
    int redTeamScore = 0;
    public Pitch pitch;
    public Puck puck;
    Team leftTeam;
    Team rightTeam;
    bool gameStarted = false;
    public TextMesh blueTeamScoreText;
    public TextMesh redTeamScoreText;

    // Use this for initialization
    void Start () {
        // create 5 players for red team and 5 players for blue team
        redTeam.players = InstantiatePlayers(redTeam, "RedPlayer");
        blueTeam.players = InstantiatePlayers(blueTeam, "BluePlayer");
        leftTeam = redTeam;
        rightTeam = blueTeam;
        redTeam.pitch = pitch;
        blueTeam.pitch = pitch;
        redTeam.scoreboard = redTeamScoreText;
        blueTeam.scoreboard = blueTeamScoreText;
        setRedTeamColors();
        setBlueTeamColors();
        leftTeam.isManual = true;
        blueTeamScoreText.text = blueTeamScore.ToString();
        redTeamScoreText.text = redTeamScore.ToString();
        pitch.StartPeriod(leftTeam, rightTeam);
        // every x seconds we should switch sides
    }


    Player[] InstantiatePlayers(Team team, string prefabName)
    {
        Player p0 = InstantiatePlayer(team, prefabName, "gk");
        p0.goalKeeper = true;
        Player p1 = InstantiatePlayer(team, prefabName, "df1");
        Player p2 = InstantiatePlayer(team, prefabName, "df2");
        Player p3 = InstantiatePlayer(team, prefabName, "fw1");
        Player p4 = InstantiatePlayer(team, prefabName, "fw2");
        return new Player[5] { p0, p1, p2, p3, p4 };
    }

    Player InstantiatePlayer(Team team, string prefabName, string namePostfix)
    {
        GameObject p0 = (GameObject)Instantiate(Resources.Load(prefabName));
        p0.name = team.name + namePostfix;
        p0.GetComponent<Player>().pitch = pitch;
        p0.GetComponent<Player>().team = team;
        p0.GetComponent<Player>().puck = puck;
        p0.GetComponent<Player>().steer = p0.GetComponent<AISteering>();
        p0.GetComponent<Player>().textmesh = 
            GameObject.Find(p0.name + "Text").GetComponent<TextMesh>();
        return p0.GetComponent<Player>();
    }

    void setRedTeamColors()
    {
        redTeam.controllingColor = new Color(179, 20, 20);
        redTeam.supportingColor = Color.yellow;
        redTeam.receivingColor = new Color(255, 255, 193);
    }

    void setBlueTeamColors()
    {
        blueTeam.controllingColor = new Color(0, 0, 139);
        blueTeam.supportingColor = Color.cyan;
        blueTeam.receivingColor = new Color(115, 45, 156);
    }


    void kickOff()
    {
        leftTeam.positionPlayersAt(leftTeam.startingRegion);
        leftTeam.clearPlayerRolesAndColors();
        rightTeam.clearPlayerRolesAndColors();
        rightTeam.positionPlayersAt(rightTeam.startingRegion);
        pitch.puck.transform.position = new Vector3(0, 0, 0);
        redTeam.ChangeState(ScriptableObject.CreateInstance<Defending>());
        rightTeam.ChangeState(ScriptableObject.CreateInstance<Attack>());
        redTeam.players[0].ChangeState(ScriptableObject.CreateInstance<GoalKeeper>());
        blueTeam.players[0].ChangeState(ScriptableObject.CreateInstance<GoalKeeper>());
        Team manualTeam = redTeam.isManual ? redTeam : blueTeam;
        manualTeam.setControllingPlayer(manualTeam.players[3]);
        foreach (Player player in manualTeam.players)
        {
            if (!player.goalKeeper) {
                if (!player.statename.ToLower().Equals("wait"))
                {
                    player.ChangeState(ScriptableObject.CreateInstance<Wait>());
                }
            }
        }
        // pick a random player and make it manual
        manualTeam.players[Random.Range(1, 4)].ChangeState(ScriptableObject.CreateInstance<ManualPlayer>());
    }

    // Update is called once per frame
    void Update () {
        if (!gameStarted)
        {
            kickOff();
            gameStarted = true;
            return;
        }
        bool scored = false;
        if (pitch.LeftTeamScored(puck.transform.position))
        {
            leftTeam.score++;
            leftTeam.scoreboard.text = leftTeam.score.ToString();
            scored = true;
        } else if (pitch.RightTeamScored(puck.transform.position))
        {
            rightTeam.score++;
            rightTeam.scoreboard.text = rightTeam.score.ToString();
            scored = true;
        }
        // if ball position is now
        if (scored)
        {
            kickOff();
        }
	}
}
