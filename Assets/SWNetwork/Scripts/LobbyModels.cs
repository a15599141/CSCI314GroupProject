using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class RoomCustomData
{
    public string name;
    public TeamCustomData team1;
    public TeamCustomData team2;
    public TeamCustomData team3;
    public TeamCustomData team4;
}

[Serializable]
public class TeamCustomData
{
    public List<string> players = new List<string>();
}
