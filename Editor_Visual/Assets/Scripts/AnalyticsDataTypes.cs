using System;
using System.Collections.Generic;

[Serializable]
public class AnalyticsData
{
    public List<GameRunData> gameRuns;
    public List<SessionData> gameSessions;
    public List<PositionDataRaw> positions;
    public List<DeathData> deaths;
    public List<KillData> kills;
    public List<HitData> playerHits;
    public List<HitData> enemyHits;
    public List<CheckpointData> checkpoints;
    public List<HealData> heals;
    public List<SimpleEventData> breakBoxes;
    public List<SimpleEventData> buttons;
    public List<SimpleEventData> keys;
    public List<SimpleEventData> jumps;
}

[Serializable]
public class SessionData
{
    public string Session_id;
    public string Player_id;
}

[Serializable]
public class GameRunData
{
    public string GameRun_ID;
    public string GameSession_ID;
    public string Win;
    public string IsRestart;
    public string Run_Start;
    public string Run_End;
}

[Serializable]
public class PositionDataRaw
{
    public string GameRun_ID;
    public string Pos_x;
    public string Pos_y;
    public string Pos_z;
}

[Serializable]
public class DeathData
{
    public string GameRun_ID;
    public string Pos_x;
    public string Pos_y;
    public string Pos_z;
}

[Serializable]
public class KillData
{
    public string GameRun_ID;
    public string Pos_x;
    public string Pos_y;
    public string Pos_z;
}

[Serializable]
public class HitData
{
    public string GameRun_ID;
    public string Pos_x;
    public string Pos_y;
    public string Pos_z;
}

[Serializable]
public class CheckpointData
{
    public string GameRun_ID;
    public string Pos_x;
    public string Pos_y;
    public string Pos_z;
}

[Serializable]
public class HealData
{
    public string GameRun_ID;
    public string Pos_x;
    public string Pos_y;
    public string Pos_z;
}

[Serializable]
public class SimpleEventData
{
    public string GameRun_ID;
    public string Pos_x;
    public string Pos_y;
    public string Pos_z;
}