public class Station
{
    Player owner; //所有者, player1 = 1,player2 = 2, ...
    public int level;//等级
    public int def;//防御
    public int atk;//攻击
    public int maxHp;//血量最大值
    public int hp;//血量
    public int energyPerTurn;//每回合产生能源

    public Station(Player ow)
    {
        owner = ow;
        level = 1;
        atk = 0;
        def = 2;
        maxHp = 6;
        hp = 6;
        energyPerTurn = 2;
    }

    public bool isOwner(Player tp)
    {
        return owner == tp;
    }
    public Player getOwner()
    {
        return owner;
    }

    public void gainEnergy()
    {
        owner.setEnergy(energyPerTurn);
    }

    public string getATK()
    {
        return atk.ToString();
    }

    public string getDEF()
    {
        return def.ToString();
    }
    public string getMaxHP()
    {
        return maxHp.ToString();
    }
    public string getHP()
    {
        return hp.ToString();
    }

    public void setHP(int h)
    {
        this.hp -= h;
    }
    public void setHPToMax()
    {
        hp = maxHp;
    }
    public void setLevel(int l)
    {
        this.level = l;
    }
}
