using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct Item
{
    public string name;
    public int energyCost;
    public string description;
    public int HP;
    public RawImage image;
    public Item(string n, int e, string d, int H, RawImage i ) 
    { name = n; energyCost = e; description = d; HP = H; image = i; }
};
public struct Equipment
{
    public string name;
    public int energyCost;
    public string description;
    public int level;
    public int ATK;
    public int DEF;
    public int EVD;
    public RawImage image;
    public Equipment(string n, int e, string d, int l, int A, int D, int E, RawImage i)
    { name = n; energyCost = e; description = d; level = l; ATK = A; DEF = D; EVD = E; image = i; }
};
public struct SpaceShip
{
    public string name;
    public int energyCost;
    public string description;
    public int level;
    public int HP;
    public int ATK;
    public int DEF;
    public int EVD;
    public SpaceShip(string n, int e, string d, int l, int H, int A, int D, int E)
    { name = n; energyCost = e; description = d; level = l; HP = H; ATK = A; DEF = D; EVD = E; }
};