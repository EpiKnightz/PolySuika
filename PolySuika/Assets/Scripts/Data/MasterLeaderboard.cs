using System;
using System.Collections.Generic;
using UnityEngine;

//[Serializable]
//public struct LeaderboardGroup //: IEquatable<LeaderboardGroup>
//{
//    public Leaderboard Leaderboard;
//    //public string LeaderboardName;
//    //public LeaderboardID LeaderboardID; // Instance ID of GameObject gamemode.

//    public LeaderboardGroup(Leaderboard leaderboard, LeaderboardID leaderboardID) : this()
//    {
//        Leaderboard = leaderboard;
//        //LeaderboardID = leaderboardID;
//    }

//    //public readonly bool Equals(LeaderboardGroup other)
//    //{
//    //    return LeaderboardID.Equals(other.LeaderboardID);
//    //}
//}

[Serializable]
public struct LeaderboardID : IEqualityComparer<LeaderboardID>
{
    public int GameModeID; // Instance ID of GameObject gamemode.
    public int LevelSetID; // Instance ID of LevelSet gamemode. If = -1 mean overall

    public LeaderboardID(int gameModeID, int levelSetID)
    {
        GameModeID = gameModeID;
        LevelSetID = levelSetID;
    }

    public bool Equals(LeaderboardID x, LeaderboardID y)
    {
        return x.GameModeID == y.GameModeID
                && x.LevelSetID == y.LevelSetID;
    }

    public int GetHashCode(LeaderboardID obj)
    {
        return (31 * obj.GameModeID) ^ (obj.LevelSetID >> 16);
    }
}

[Serializable]
public class MasterLeaderboard
{
    [SerializeField] public Dictionary<LeaderboardID, Leaderboard> LeaderboardDict;

    public void Update(Leaderboard leaderboard, int modeID, int setID)
    {
        LeaderboardID leaderboardID = new(modeID, setID);
        LeaderboardDict[leaderboardID] = leaderboard;

    }

    public Leaderboard Find(int modeID, int setID)
    {
        LeaderboardID leaderboardID = new(modeID, setID);
        return LeaderboardDict[leaderboardID];
    }
}
