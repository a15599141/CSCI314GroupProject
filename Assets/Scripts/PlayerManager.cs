using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public List<GameObject> playerObjects;//玩家对象集合
    public List<GameObject> spawnPoints;//出生点对象集合
    public List<Station> stations = new List<Station>();//保存空间站集合

    public Dice dice;
    public bool moveAllowed;//判断当前玩家是否可以移动
    int playerNumber; //玩家数量

    public Player currPlayer;//当前玩家
    public int currPlayerIndex;

    int steps; //当前玩家需要移动的格子数
    int tempSteps;//缓存剩余格子数

    float moveSpeed = 10.0f;//移动速度

    //启动单例
    private static PlayerManager _instance;

    public static PlayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("Player Manager").GetComponent<PlayerManager>();
            }

            return _instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //初始化空间站
        for (int i = 0; i < Route.Instacnce.routeNum; i++)
        {
            stations.Add(null);
        }

        //playerObjects = GameObject.FindGameObjectsWithTag("Player").ToList();//获取所有玩家
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint").ToList();//获取所有出生点
        playerNumber = playerObjects.Count;//玩家总数

        //初始化玩家
        for (int i = 0; i < playerNumber; i++)
        {
            currPlayer = playerObjects[i].GetComponent<Player>();
            currPlayer.transform.position = spawnPoints[i].transform.position;
            currPlayer.routePosition = spawnPoints[i].transform.GetSiblingIndex();
            currPlayer.id = i + 1;
        }

        //选中第一个作为当前玩家
        currPlayerIndex = 0;
        currPlayer = playerObjects[currPlayerIndex].GetComponent<Player>();


        //更新UI界面玩家信息
        CanvasManager.Instance.UpdatePlayerPanel();
    }

    // Update is called once per frame
    void Update()
    {
        if (moveAllowed)
        {
            moveAllowed = false;
            steps = dice.diceNumber;
            tempSteps = steps; 
            //Debug.Log("dice number: " + steps);
            StartCoroutine(PlayerMove());
        }
    }

    IEnumerator PlayerMove()
    {
        if (tempSteps==0) yield break; // 如果玩家完全停止移动，退出协程

        while (steps > 0)
        {
            currPlayer.routePosition = (currPlayer.routePosition + 1) % Route.Instacnce.childNodeList.Count;

            Vector3 nextPos = Route.Instacnce.childNodeList[currPlayer.routePosition].position;
            //判断是否到达下一格
            while (MoveToNextNode(nextPos))
            {
                yield return null;
            }
            //移动完毕停顿
            yield return new WaitForSeconds(0.1f);
            steps--;

            //玩家经过商店或遭遇其他玩家时停止当前移动
            if (currPlayer.isOnTradeStation||currPlayer.isEngaging)
            {
                Debug.Log(currPlayer.name + "isOnTradeStationg or engagement ");
                tempSteps = steps;//缓存剩余格子数
                steps = 0;//停止当前移动
            }
        }

        if (currPlayer.isOnTradeStation)
        {
            //处理玩家经过商店
            CanvasManager.Instance.IsConfirm(ConfirmType.isTradeStation);//选择是否进入商店
        }else if (currPlayer.isEngaging)
        {
            //处理玩家遭遇战斗
            currPlayer.BattleTargetIsPlayer = true;
            CanvasManager.Instance.IsConfirm(ConfirmType.isBattle);//选择是否战斗
        }else
        {
            //处理玩家完全停下之后与station的交互
            DealWithStation();
        }
    }

    public void DealWithStation()
    {
        if (stations[currPlayer.routePosition] == null)//未被占领则提示是否建造
        {
            CanvasManager.Instance.IsConfirm(ConfirmType.isConstruction);
        }
        else if (stations[currPlayer.routePosition].isOwner(currPlayer))//如果是当前玩家的建筑则提示升级
        {
            Debug.Log("update");
            EndTheTurn();
        }
        else//如果不是当前玩家的建筑则提示战斗或过路费
        {
            Debug.Log("battle or pay " + stations[currPlayer.routePosition].getOwner());
            //EndTheTurn();
            currPlayer.tarPlayer = stations[currPlayer.routePosition].getOwner();//设定当前玩家的目标玩家为空间站所有者
            currPlayer.BattleTargetIsPlayer = false;
            CanvasManager.Instance.OpenCanvasEnemyStation();
        }
    }

    bool MoveToNextNode(Vector3 goal)
    {
        return goal != (currPlayer.transform.position = Vector3.MoveTowards(currPlayer.transform.position, goal, moveSpeed * Time.deltaTime));
    }

    public void EndTheTurn()
    {
        //清除当前玩家的状态
        currPlayer.isEngaging = false;
        currPlayer.isOnTradeStation = false;
        currPlayerIndex = (currPlayerIndex + 1) % playerNumber;//获取下一个玩家下标
        //currPlayerIndex = 0;
        currPlayer = playerObjects[currPlayerIndex].GetComponent<Player>();//切换到下一个玩家

        if (currPlayerIndex == 0)//如果下一个玩家是第一个玩家
        {
            CanvasManager.Instance.roundCount++; //回合数 + 1
            CanvasManager.Instance.roundText.text = "ROUND " + CanvasManager.Instance.roundCount.ToString(); //更新回合数
            //计算玩家获得能量根据拥有的空间站数量
            foreach (Station sta in stations)
            {
                if (sta!=null)//跳过无人空间站
                {
                    sta.gainEnergy();
                }
            }
        }
        CanvasManager.Instance.UpdatePlayerPanel(); // 更新玩家属性面板
        CanvasManager.Instance.UpdatePlayerBag(); // 更新背包界面
        CanvasManager.Instance.playerUseItemButton.interactable = true;//释放回合开始时使用道具按钮
        dice.rollButton.interactable = true;//释放摇骰子按钮

        //如果当前玩家血量过低，打开维修面板
        if (currPlayer.getCurrHP() <= 0)
        {
            CanvasManager.Instance.OpenCanvasRepair();
        }
    }

    //战斗取消
    public void BattleCancel()
    {
        currPlayer.isEngaging = false;
        if (tempSteps == 0) DealWithStation(); //如果玩家遭遇并取消战斗，且该点恰好为当前移动玩家的落点，则对落脚点station处理剩余事件
        else
        {
            steps = tempSteps;
            StartCoroutine(PlayerMove());
        }
    }

    //交易取消
    public void TradeCancel()
    {
        currPlayer.isOnTradeStation = false;
        if (tempSteps == 0) EndTheTurn(); //如果商店恰好为当前移动玩家的落点，结束回合
        else
        {
            steps = tempSteps;
            StartCoroutine(PlayerMove());
        }
    }
}
