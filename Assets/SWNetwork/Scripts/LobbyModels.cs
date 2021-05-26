using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class RoomCustomData // room data class that contains team info
{
    public string name;
    public TeamCustomData team1;
    public TeamCustomData team2;
    public TeamCustomData team3;
    public TeamCustomData team4;
}

[Serializable]
public class TeamCustomData // team info that contains player info
{
    public List<string> players = new List<string>();
}
