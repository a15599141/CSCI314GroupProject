using System.Collections;
using UnityEngine;
using SWNetwork;
using System.Collections.Generic;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int routePosition;//玩家所在格子位置
    public bool isEngaging; //玩家是否遭遇
    public Player tarPlayer;//目标玩家
    public bool BattleTargetIsPlayer;// 玩家战斗目标是否为玩家 true=玩家 false=玩家的空间站
    public bool isOnTradeStation;//玩家是否在交易站

    //玩家属性
    public int id;
    int energy;//玩家当前能量币
    int maxHP; //飞船总血量
    int currHP;//飞船当前血量
    int atk;//飞船攻击
    int def;//飞船防御
    int evd;//飞船闪避
    public List<Item> playerItemBag = new List<Item>(); //玩家道具背包
    public List<Equipment> playerEquipmentBag = new List<Equipment>(); //玩家战斗装备背包
    private void Awake() //初始状态及属性
    {
        isEngaging = false;
        isOnTradeStation = false;
        energy = 10;
        maxHP = 6;
        currHP = 6;
        atk = 3;
        def = 2;
        evd = 1;
    }

    // Start is called before the first frame update
    void Start()
    {
  
    }
    // Update is called once per frame
    void Update()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("TradeStation"))
        {
            Debug.Log("TradeStation arrived");
            isOnTradeStation = true;
        }
        else if (other.transform.CompareTag("Player"))
        {
            //判断当前玩家进入战斗状态
            if (PlayerManager.Instance.currPlayer == this)
            {
                Debug.Log(name + " vs " + other.name + "battle incomming");
                isEngaging = true;
                tarPlayer = other.GetComponent<Player>();
            }
        }
       
    }
    public int getEnergy()
    {
        return energy;
    }

    public bool setEnergy(int e) //扣钱e设置负数，加钱设置e为正数
    {
        if (energy+e<0)
        {
            return false;
        }
        else
        {
            energy += e;
            return true;
        }
    }
    public void DirectSetEnergy(int e) //直接设置能源
    {
        if (e >= 0)energy = e;
    }

    public void setHP(int hp)  //扣钱e设置负数，加钱设置e为正数
    {
        if(currHP+hp>maxHP) currHP = maxHP;
        else currHP += hp;
    }

    public int getMaxHP()
    {
        return maxHP;
    }
    public int getCurrHP()
    {
        return currHP;
    }

    public int getATK()
    {
        return atk;
    }
    public int getDEF()
    {
        return def;
    }
    public int getEVD()
    {
        return evd;
    }

    public void Repaire()
    {
        currHP = maxHP;
    }
    public void GetNewSpaceShip(int m, int a, int d, int e)
    {
        maxHP = m; 
        currHP = m;
        atk = a;
        def = d;
        evd = e;
    }
    // 空间站战斗
    public string Battle(int diceNum1, int diceNum2,Station sta)
    {
        int damage;
        damage = (atk + diceNum1) - (sta.def + diceNum2);
        damage = damage <= 0 ? 1 : damage;
        sta.setHP(damage);
        return tarPlayer.name + "'s station got " + damage + " damage from " + name;
    }
    // 玩家战斗（方法重载）
    public string Battle(int diceNum1, int diceNum2, Equipment e1, Equipment e2, bool defenderChooseDefend)
    {
        int damage;
        if (defenderChooseDefend) //如果防御者选择防御
        {
            damage = (atk + diceNum1 + e1.ATK) - (tarPlayer.def + diceNum2 + e2.DEF);//计算战斗伤害
            damage = damage <= 0 ? 1 : damage;//如果造成的伤害小于1，则固定1点伤害
            tarPlayer.currHP -= damage;
            CanvasManager.Instance.getDamageSound.Play();
            return tarPlayer.name + " got " + damage + " damage from " + name;
        }
        else //如果防御者选择闪避
        {
            damage = (atk + diceNum1 + e1.ATK) - (tarPlayer.evd + diceNum2 + e2.EVD); //计算闪避是否成功
            if (damage <= 0) 
                return tarPlayer.name + " has successfully evaded from attack"; // 如果闪避成功，则伤害为0
            else
            {
                damage = atk + diceNum1 + e1.ATK; // 如果闪避失败，则伤害拉满
                tarPlayer.currHP -= damage;
                CanvasManager.Instance.getDamageSound.Play();
                return tarPlayer.name + " got " + damage + " damage from " + name;
            }
        }
    }
}
