using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class CanvasManager : MonoBehaviour
{
    private static CanvasManager _instance;
    public static CanvasManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("Canvas Manager").GetComponent<CanvasManager>();
            }
            return _instance;
        }
    }

    public GameObject[] playerPanels; //玩家属性面板
    public RawImage playerPanelHighlight;//玩家属性面板高亮

    public GameObject canvasMessage;//提示信息面板
    public Text textMessage;//提示信息面板文字

    public GameObject canvasConfirm;//确认信息面板
    public Button buttonConfirm;//确认按钮
    public Button buttonCancel;//确认界面取消按钮
    public Text textConfirm;//确认面板文字提示
    
    public GameObject canvasShop;//商店面板对象
    public GameObject canvasEnemyStation;//遭遇敌方空间站面板对象

    //战斗相关
    public GameObject canvasBattle;//战斗界面
    public DiceForBattle diceForAttack;//战斗攻击用骰子
    public DiceForBattle diceForDefend;//战斗防御用骰子
    public DiceForBattle diceForRepair;//维修用骰子
    public Button BattleAttackButton; //攻击按钮
    public Button BattleDefendButton; //防御按钮
    public Button AttackerEquipButton; //攻击者使用战斗装备按钮
    public Button DefenderEquipButton; //防御者使用战斗装备按钮
    public Button BattleEvadeButton; //闪避按钮
    public int AttackerUsedEquipmentIdx; //记录战斗时攻击者用的装备的背包索引，用于战斗结算，背包移除
    public int DefenderUsedEquipmentIdx; //记录战斗时防御者用的装备的背包索引，用于战斗结算，背包移除
    public bool AttackerIsEquiped; //判断攻击者是否使用了装备
    public bool DefenderIsEquiped; //判断防御者是否使用了装备
    public bool DefenderChooseDefend; //防御者选择防御还是闪避；

    public Text textBattlePlayer1Name;
    public Text textBattlePlayer1Status;
    public Text battleEquipmentEffect1;
    public Text textBattlePlayer1HP;
    public RawImage battleSpaceShipImage1;
    public RawImage battleEquipmentImage1;

    public Text textBattlePlayer2Name;
    public Text textBattlePlayer2Status;
    public Text battleEquipmentEffect2;
    public Text textBattlePlayer2HP;
    public RawImage battleSpaceShipImage2;
    public RawImage battleEquipmentImage2;

    //维修界面
    public GameObject canvasRepair;

    //商店UI及商品选择高光
    public RawImage itemHighlight;
    public RawImage equipmentHighlight;
    public RawImage spaceShipHighlight;
    public Button[] items;     // 商店道具按钮
    public Button[] equipments;// 商店战斗装备按钮
    public Button[] spaceShips;// 商店飞船按钮
    public Button buyButton; // 购买按钮
    
    // 商店物品列表
    List<Item> itemList = new List<Item>();
    List<Equipment> equipmentList = new List<Equipment>();
    List<SpaceShip> spaceShipList = new List<SpaceShip>();
    public static int maxSpaceShipOfOneColor = 3; //定义每个颜色的玩家可购买的飞船种类数量

    //储存商店物品图片，储存顺序查看inspector
    public RawImage[] itemImages; //道具图片
    public RawImage[] equipmentImages; //战斗装备图片
    public RawImage[] spaceShipImages;//飞船图片

    //空间站图片集
    public RawImage[] stationRedImages;
    public RawImage[] stationYellowImages;
    public RawImage[] stationBlueImages;
    public RawImage[] stationGreenImages;
    public GameObject StationDefaultMaterial;
    public bool stationIsDestoryedByPlayer;

    //定义背包
    public RawImage[] bagImages; // UI界面的背包图片
    public static int itemBagMaxCapacity = 2;//背包道具容量
    public static int equipmentBagMaxCapacity = 3;//背包战斗装备容量
    public Button[] useItemButtons; // 背包道具区左上角的使用按钮
    public Button[] useEquipmentButtons; // 背包战斗装备区左上角的使用按钮
    public Button cancelItemUseButton; // 取消道具使用按钮
    public Button confirmItemUseButton; // 确认回合开始道具使用按钮
    public Button cancelEquipButton; // 取消装备使用按钮
    public Button playerUseItemButton; //玩家回合开始时，选择使用道具按钮
    public int playerUsedItemIdx; //记录当前玩家回合开始时，使用的道具在背包里的索引，用于道具生效及背包移除
    public bool playerHasUsedItem; // 记录玩家回合开始时是否使用道具

    // 回合数
    public TMP_Text roundText;  //游戏轮数显示器
    public int roundCount;     //游戏轮数计数器

    // 音效及BGM
    public AudioSource BGM;
    public AudioSource battleBGM;
    public AudioSource attackSound;
    public AudioSource defendSound;
    public AudioSource evadeSound;
    public AudioSource getDamageSound;
    public AudioSource repairSound;
    public AudioSource errorSound;
    public AudioSource updateSound;

    // Start is called before the first frame update
    void Start()
    {
        roundCount = 1; //初始回合为1
        PlayerUseItemEvent(); // 回合开始时使用道具事件初始化
        TradeStationInitialize(); // 商店初始化
        BattleInitialize(); // 战斗系统初始化

    }
    // Update is called once per frame
    void Update()
    {
        playerPanelHighlight.transform.position = playerPanels[PlayerManager.Instance.currPlayerIndex].transform.position; //当前玩家面板高亮
    }

    public void showMessage(string msg)
    {
        textMessage.text = msg;
        canvasMessage.SetActive(true);
        StartCoroutine(delayMessage()); 
    }
    IEnumerator delayMessage()
    {
        yield return new WaitForSeconds(2.5f); //消息弹窗显示2.5秒
        canvasMessage.SetActive(false);
    }
    public void IsConfirm(ConfirmType type)
    {
        canvasConfirm.SetActive(true);
        switch (type)
        {
            case ConfirmType.isBattle:
                textConfirm.text = "Battle?";
                buttonConfirm.onClick.AddListener(BattleConfirm);
                buttonCancel.onClick.AddListener(BattleCancel);
                break;
            case ConfirmType.isTradeStation:
                textConfirm.text = "Trade?";
                buttonConfirm.onClick.AddListener(TradeStationConfirm);
                buttonCancel.onClick.AddListener(TradeStationCancel);
                break;
            case ConfirmType.isConstruction:
                textConfirm.text = "Construct station here? Cost 5 energy";
                buttonConfirm.onClick.AddListener(ConstructConfirm);
                buttonCancel.onClick.AddListener(ConstructCancel);
                break;
            case ConfirmType.energyNotEnough:
                textConfirm.text = "Energy not enough";
                errorSound.Play();
                buttonConfirm.onClick.AddListener(ConfirmExit);
                buttonCancel.onClick.AddListener(ConfirmExit);
                break;
            case ConfirmType.itemBagIsFull:
                textConfirm.text = "Item bag is full";
                errorSound.Play();
                buttonConfirm.onClick.AddListener(ConfirmExit);
                buttonCancel.onClick.AddListener(ConfirmExit);
                break;
            case ConfirmType.equipmentBagIsFull:
                textConfirm.text = "Equipment bag is full";
                errorSound.Play();
                buttonConfirm.onClick.AddListener(ConfirmExit);
                buttonCancel.onClick.AddListener(ConfirmExit);
                break;
            case ConfirmType.spaceShipPurchaseError:
                textConfirm.text = "You only can purchase the spaceships with your own color";
                errorSound.Play();
                buttonConfirm.onClick.AddListener(ConfirmExit);
                buttonCancel.onClick.AddListener(ConfirmExit);
                break;
        }
    }

    public void ConstructConfirm()
    {
        Player player = PlayerManager.Instance.currPlayer;
        //消费能量
        if (player.setEnergy(-5))
        {
            Material ma = player.transform.GetChild(0).GetComponent<MeshRenderer>().material;// 获取玩家颜色
            Route.Instacnce.childNodeList[player.routePosition].GetChild(0).GetComponent<MeshRenderer>().material = ma;//空间站变为玩家颜色
            PlayerManager.Instance.stations[player.routePosition] = new Station(player);//添加入station集合
            PlayerManager.Instance.stations[player.routePosition].setHPToMax(); //新占领的空间站补满血
            PlayerManager.Instance.EndTheTurn(); //结束回合
            ConfirmExit();
        }
        else
        {
            showMessage("Insufficient energy ! Construction failed");
            ConstructCancel();
        }
    }

    public void ConstructCancel() // 玩家取消建造
    {
        Player currPlayer = PlayerManager.Instance.currPlayer;
        if (stationIsDestoryedByPlayer) // 如果空间站被玩家打爆
        {
            Material defaultMaterial = StationDefaultMaterial.GetComponent<MeshRenderer>().material;//获取默认空间站材质
            Route.Instacnce.childNodeList[currPlayer.routePosition].transform.GetChild(0).GetComponent<MeshRenderer>().material = defaultMaterial;//空间站材质恢复为默认
            stationIsDestoryedByPlayer = false;
            PlayerManager.Instance.stations[currPlayer.routePosition] = null; //将空间站从空间站列表移除
        }
        PlayerManager.Instance.EndTheTurn();
        ConfirmExit();
    }
    public void BattleConfirm()
    {
        UpdateBattlePanel(); //更新战斗面板信息
        BGM.Stop();
        battleBGM.Play(); //放战斗音乐
        canvasBattle.SetActive(true);//显示战斗面板
    }
    public void BattleCancel()
    {
        PlayerManager.Instance.BattleCancel();
        ConfirmExit();
    }
    public void TradeStationConfirm()
    {
        canvasShop.SetActive(true);
        ConfirmExit();
    }
    public void TradeStationCancel()
    {
        PlayerManager.Instance.TradeCancel();
        ConfirmExit();
    }

    void ConfirmExit()
    {
        canvasConfirm.SetActive(false);
        buttonConfirm.onClick.RemoveAllListeners();
        buttonCancel.onClick.RemoveAllListeners();
    }

    //打开商店界面
    public void OpenCanvasShop()
    {
        canvasShop.SetActive(true);
    }

    //关闭商店界面
    public void CloseTradeStation()
    {
        PlayerManager.Instance.currPlayer.isOnTradeStation = false;
        PlayerManager.Instance.EndTheTurn();
        canvasShop.SetActive(false);
    }

    //打开维修界面
    public void OpenCanvasRepair()
    {
        canvasRepair.SetActive(true);
    }
    //关闭维修界面
    public void CloseCanvasRepaire()
    {
        canvasRepair.SetActive(false);
    }

    public void DelayRepaire()
    {
        Invoke("Repaire", 2.5f); //延迟2.5秒判断飞船是否修复，保证骰子动画播放完毕
    }
    public void Repaire()
    {
        if (diceForRepair.diceNumber>3) //如果骰子点数大于3，修复飞船
        {
            repairSound.Play(); // 播放维修音效
            PlayerManager.Instance.currPlayer.Repaire(); //飞船维修血条拉满
            UpdatePlayerPanel();
            CloseCanvasRepaire();
            showMessage("your spaceship has repaired!"); // 维修成功弹窗提示
        }
        else //如果骰子点数小于等于3，维修失败
        {
            errorSound.Play(); // 播放维修失败音效
            CloseCanvasRepaire();
            showMessage("Failed to repaire !");  // 维修失败弹窗提示
            PlayerManager.Instance.EndTheTurn(); // 直接结束回合，跳到下一个玩家
        }
    }

    //打开遭遇敌方空间站界面
    public void OpenCanvasEnemyStation()
    {
        canvasEnemyStation.SetActive(true);
    }

    //关闭遭遇敌方空间站界面
    public void CloseCanvasEnemyStation()
    {
        canvasEnemyStation.SetActive(false);
    }

    //敌方空间站补给
    public void EnemyStationSupply()
    {
        Player player = PlayerManager.Instance.currPlayer;
        if (player.setEnergy(-5))
        {
            player.tarPlayer.setEnergy(5);
            CloseCanvasEnemyStation();
            showMessage("paid 5 energy to "+player.tarPlayer.name);
            PlayerManager.Instance.EndTheTurn();
        }
        else
        {
            showMessage("not enough energy!");
        }
    }
    public void PlayerUseItemEvent() // 回合开始时使用道具事件
    {
        playerUsedItemIdx = itemBagMaxCapacity; // 初始化索引
        playerUseItemButton.onClick.AddListener(() => // 使用道具按钮添加监听事件
        {
            playerUseItemButton.interactable = false;
            confirmItemUseButton.gameObject.SetActive(true); //显示确认使用按钮
            confirmItemUseButton.interactable = false; //未选择使用道具时，确认按钮将不可用
            cancelItemUseButton.gameObject.SetActive(true); // 显示取消使用按钮

            Player currPlayer = PlayerManager.Instance.currPlayer;
            for (int i = 0; i < currPlayer.playerItemBag.Count; i++)
            {
                useItemButtons[i].onClick.RemoveAllListeners(); // 移除当前背包使用按钮的监听事件
                useItemButtons[i].gameObject.SetActive(true); // 根据背包道具数量启用对应使用按钮
            }
            useItemButtons[0].onClick.AddListener(() => // 点击背包使用按钮，更新战斗界面equipment信息
            {
                playerUsedItemIdx = 0; // 记录当前使用的道具在背包的索引
                playerHasUsedItem = true; 
                useItemButtons[0].interactable = false; 
                useItemButtons[1].interactable = true;
                confirmItemUseButton.interactable = true; //选择使用道具时，确认按钮变为可用
            });
            useItemButtons[1].onClick.AddListener(() => // 点击背包使用按钮，更新战斗界面equipment信息
            {
                playerUsedItemIdx = 1; // 记录当前使用的道具在背包的索引
                playerHasUsedItem = true;
                useItemButtons[1].interactable = false;
                useItemButtons[0].interactable = true;
                confirmItemUseButton.interactable = true; //未选择使用道具时，确认按钮变为可用
            });
        });

        confirmItemUseButton.onClick.AddListener(() => // 确认使用道具按钮添加监听事件
        {
            Player currPlayer = PlayerManager.Instance.currPlayer;
            if (currPlayer.getCurrHP()==currPlayer.getMaxHP())//如果当前血量为满，将不可使用道具
            {
                showMessage("You space ship is on optimal state!");//弹出提示框
                cancelItemUseButton.onClick.Invoke();
            }
            else
            {
                useItemButtons[0].gameObject.SetActive(false); // 隐藏道具1使用按钮
                useItemButtons[0].interactable = true; // 道具1使用按钮重新变为默认可用
                useItemButtons[1].gameObject.SetActive(false);// 隐藏道具2使用按钮
                useItemButtons[1].interactable = true;// 道具2使用按钮重新变为默认可用
                playerHasUsedItem = false;

                cancelItemUseButton.gameObject.SetActive(false);//隐藏取消使用道具
                confirmItemUseButton.interactable = false; //取消使用道具时，确认按钮重新变为默认不可用
                confirmItemUseButton.gameObject.SetActive(false); //隐藏确认使用道具

                currPlayer.setHP(currPlayer.playerItemBag[playerUsedItemIdx].HP); // 使用道具给飞船加血
                showMessage("You have recover you space ship by " + currPlayer.playerItemBag[playerUsedItemIdx].HP);//弹出道具使用提示框
                currPlayer.playerItemBag.RemoveAt(playerUsedItemIdx); //将道具从背包移除
                playerUsedItemIdx = itemBagMaxCapacity; //初始化索引
                UpdatePlayerPanel();//刷新玩家面板
                UpdatePlayerBag(); // 刷新背包界面
            }
        });

        cancelItemUseButton.onClick.AddListener(() => // 取消使用道具按钮添加监听事件
        {
            useItemButtons[0].gameObject.SetActive(false); // 隐藏道具1使用按钮
            useItemButtons[0].interactable = true; // 道具1使用按钮重新变为默认可用
            useItemButtons[1].gameObject.SetActive(false);// 隐藏道具2使用按钮
            useItemButtons[1].interactable = true;// 道具2使用按钮重新变为默认可用
            playerUsedItemIdx = itemBagMaxCapacity; //初始化索引
            playerHasUsedItem = false;

            cancelItemUseButton.gameObject.SetActive(false);//隐藏取消使用道具
            confirmItemUseButton.interactable = false; //取消使用道具时，确认按钮重新变为默认不可用
            confirmItemUseButton.gameObject.SetActive(false); //隐藏确认使用道具
            playerUseItemButton.interactable = true;
        });
    }
    public void EndBattle() // 结算战斗
    {
        Player currPlayer = PlayerManager.Instance.currPlayer;//获取当前玩家
        Equipment AttackerUsedEquipment = new Equipment();
        Equipment DefenderUsedEquipment = new Equipment(); 

        if (AttackerIsEquiped) // 如果攻击者使用了装备
        {
            AttackerUsedEquipment = PlayerManager.Instance.currPlayer.playerEquipmentBag[AttackerUsedEquipmentIdx]; //获取攻击者使用的装备，用于战斗计算
            currPlayer.playerEquipmentBag.RemoveAt(AttackerUsedEquipmentIdx); //将使用的装备从背包移除
            AttackerUsedEquipmentIdx = equipmentBagMaxCapacity; // 重新初始化索引
        }
        if (DefenderIsEquiped) // 如果防御者使用了装备
        {
            DefenderUsedEquipment = PlayerManager.Instance.currPlayer.tarPlayer.playerEquipmentBag[DefenderUsedEquipmentIdx];//获取防御者使用装备，用于战斗计算
            currPlayer.tarPlayer.playerEquipmentBag.RemoveAt(DefenderUsedEquipmentIdx); //将使用的装备从背包移除
            DefenderUsedEquipmentIdx = equipmentBagMaxCapacity; // 重新初始化索引
        }

        string battleResultMessage; // 战斗结果信息
        Station station = PlayerManager.Instance.stations[currPlayer.routePosition];//玩家所在空间站
        if (currPlayer.BattleTargetIsPlayer) // 如果是玩家战斗
        {
            //处理玩家战斗，返回结果信息
            battleResultMessage = currPlayer.Battle(diceForAttack.diceNumber, diceForDefend.diceNumber, AttackerUsedEquipment, DefenderUsedEquipment, DefenderChooseDefend);
        }
        else  // 如果是空间站战斗
        {
            getDamageSound.Play();
            station = PlayerManager.Instance.stations[currPlayer.routePosition];//玩家所在空间站
            battleResultMessage = currPlayer.Battle(diceForAttack.diceNumber, diceForDefend.diceNumber, station); //处理空间站战斗，返回结果信息
            // 重新启用相关按钮
            stationIsDestoryedByPlayer = false; // 初始化
            BattleDefendButton.gameObject.SetActive(true);
            BattleEvadeButton.gameObject.SetActive(true);
            DefenderEquipButton.gameObject.SetActive(true);
        }

        canvasBattle.SetActive(false);//关闭战斗界面
        BattleAttackButton.interactable = true; // 启用攻击按钮
        AttackerEquipButton.interactable = true; // 启用攻击者使用装备按钮
        DefenderEquipButton.interactable = true; // 启用防御者使用装备按钮
        battleEquipmentImage1.texture = null;
        battleEquipmentImage2.texture = null;
        battleEquipmentEffect1.text = "Not  equips";
        battleEquipmentEffect2.text = "Not  equips";

        showMessage(battleResultMessage); //弹出战斗结果
        battleBGM.Stop(); //战斗BGM停止播放
        BGM.Play();

        if(!currPlayer.BattleTargetIsPlayer && station.hp<=0) //如果是空间站战斗且战斗结束后空间站血量低于0
        {
            stationIsDestoryedByPlayer = true;
            showMessage("You have destroyed player" + station.getOwner().id + "'s station");
            UpdatePlayerBag();
            IsConfirm(ConfirmType.isConstruction); // 提示玩家是否占领
        }
        else PlayerManager.Instance.EndTheTurn(); //结束回合
    }
    public void UpdatePlayerPanel() //更新所有玩家属性面板
    {
        int panelIndex = 0;
        foreach (GameObject item in PlayerManager.Instance.playerObjects)
        {
            Player player = item.GetComponent<Player>();
            playerPanels[panelIndex].transform.GetChild(1).GetComponent<Text>().text
                = "Name: Player " + player.id;
            playerPanels[panelIndex].transform.GetChild(2).GetComponent<Text>().text
                = "Energy: " + player.getEnergy();
            playerPanels[panelIndex].transform.GetChild(3).GetComponent<Text>().text
                = "HP: " + player.getCurrHP() + "/" +player.getMaxHP();
            playerPanels[panelIndex].transform.GetChild(4).GetComponent<Text>().text
                = "ATK: " + player.getATK();
            playerPanels[panelIndex].transform.GetChild(5).GetComponent<Text>().text
                = "DEF: " + player.getDEF();
            playerPanels[panelIndex].transform.GetChild(6).GetComponent<Text>().text
                = "EVD: " + player.getEVD();
            panelIndex++;
        }
    } 
    public void UpdatePlayerBag() //更新当前玩家背包图片
    {
        Player currPlayer = PlayerManager.Instance.currPlayer;
        foreach (RawImage img in bagImages)
        {
            img.texture = null; // 清空背包材质
            img.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = ""; // 清空鼠标悬停'道具'细节显示
            img.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = ""; // 清空鼠标悬停'战斗装备'细节显示
        }
        for (int i = 0; i < currPlayer.playerItemBag.Count; i++) //从当前玩家'道具'背包加载图片材质
        {
            bagImages[i].texture = currPlayer.playerItemBag[i].image.texture; //替换材质
            bagImages[i].transform.GetChild(0).GetChild(2).GetComponent<Text>().text = currPlayer.playerItemBag[i].name;//更新鼠标悬停细节显示（道具名字）
            bagImages[i].transform.GetChild(0).GetChild(3).GetComponent<Text>().text = "Current HP +" + currPlayer.playerItemBag[i].HP;//更新鼠标悬停细节显示（道具效果）
        }
        for (int i = 0; i < currPlayer.playerEquipmentBag.Count; i++) //从当前玩家'战斗装备'背包加载图片材质
        {
            bagImages[i + itemBagMaxCapacity].texture = currPlayer.playerEquipmentBag[i].image.texture; //替换材质
            bagImages[i + itemBagMaxCapacity].transform.GetChild(0).GetChild(2).GetComponent<Text>().text = currPlayer.playerEquipmentBag[i].name;//鼠标悬停细节显示（名字）
            bagImages[i + itemBagMaxCapacity].transform.GetChild(0).GetChild(3).GetComponent<Text>().text = //鼠标悬停细节显示（效果）
                "ATK+"  + currPlayer.playerEquipmentBag[i].ATK+
                " DEF+" + currPlayer.playerEquipmentBag[i].DEF+
                " EVD+" + currPlayer.playerEquipmentBag[i].EVD;
        }
    }
    public void UpdatePlayerBag(Player player) //(方法重载)背包界面更新为指定玩家的背包
    {
        foreach (RawImage img in bagImages)
        {
            img.texture = null; // 清空背包材质
            img.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = ""; // 清空鼠标悬停'道具'细节显示
            img.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = ""; // 清空鼠标悬停'战斗装备'细节显示
        }
        for (int i = 0; i < player.playerItemBag.Count; i++) //从当前玩家'道具'背包加载图片材质
        {
            bagImages[i].texture = player.playerItemBag[i].image.texture; //替换材质
            bagImages[i].transform.GetChild(0).GetChild(2).GetComponent<Text>().text = player.playerItemBag[i].name;//更新鼠标悬停细节显示（道具名字）
            bagImages[i].transform.GetChild(0).GetChild(3).GetComponent<Text>().text = "Current HP +" + player.playerItemBag[i].HP;//更新鼠标悬停细节显示（道具效果）
        }
        for (int i = 0; i < player.playerEquipmentBag.Count; i++) //从当前玩家'战斗装备'背包加载图片材质
        {
            bagImages[i + itemBagMaxCapacity].texture = player.playerEquipmentBag[i].image.texture; //替换材质
            bagImages[i + itemBagMaxCapacity].transform.GetChild(0).GetChild(2).GetComponent<Text>().text = player.playerEquipmentBag[i].name;//鼠标悬停细节显示（名字）
            bagImages[i + itemBagMaxCapacity].transform.GetChild(0).GetChild(3).GetComponent<Text>().text = //鼠标悬停细节显示（效果）
                "ATK+" + player.playerEquipmentBag[i].ATK +
                " DEF+" + player.playerEquipmentBag[i].DEF +
                " EVD+" + player.playerEquipmentBag[i].EVD;
        }
    }
    public void UpdateBattlePanel()//更新战斗界面
    {
        Player player = PlayerManager.Instance.currPlayer;
        //更新战斗界面中左边玩家的信息
        textBattlePlayer1Name.text ="Player " + player.id;
        textBattlePlayer1HP.text = player.getCurrHP() + " / " + player.getMaxHP();
        textBattlePlayer1Status.text = "ATK: " + player.getATK() + 
                                       " DEF:" + player.getDEF() +
                                       " EVD:" + player.getEVD();
        battleSpaceShipImage1.texture = playerPanels[player.id-1].transform.GetChild(0).GetComponent<RawImage>().texture;
          
        if (player.BattleTargetIsPlayer) // 如果攻击目标是玩家
        {
            // 更新战斗界面中右边玩家的信息
            textBattlePlayer2Name.text = "Player " + player.tarPlayer.id;
            textBattlePlayer2HP.text = player.tarPlayer.getCurrHP() + " / " + player.tarPlayer.getMaxHP();
            textBattlePlayer2Status.text = "ATK: " + player.tarPlayer.getATK() + 
                                           " DEF:" + player.tarPlayer.getDEF() + 
                                           " EVD:" + player.tarPlayer.getEVD();
            battleSpaceShipImage2.texture = playerPanels[player.tarPlayer.id-1].transform.GetChild(0).GetComponent<RawImage>().texture;
            //关闭确认菜单
            ConfirmExit();
        }
        else  // 如果攻击目标是空间站
        {  // 更新战斗界面中右边空间站的信息
            Station station = PlayerManager.Instance.stations[player.routePosition];
            textBattlePlayer2Name.text = station.getOwner().name + "'s Station";
            textBattlePlayer2HP.text = station.getHP() + " / " + station.getMaxHP();
            textBattlePlayer2Status.text = "ATK:" + station.getATK() + "\n" +
                                           "DEF:" + station.getDEF();
            if (station.getOwner().id == 1) battleSpaceShipImage2.texture = stationRedImages[station.level - 1].texture; 
            if (station.getOwner().id == 2) battleSpaceShipImage2.texture = stationYellowImages[station.level - 1].texture;
            if (station.getOwner().id == 3) battleSpaceShipImage2.texture = stationBlueImages[station.level - 1].texture;
            if (station.getOwner().id == 4) battleSpaceShipImage2.texture = stationGreenImages[station.level - 1].texture;
            //关闭空间站遭遇面板
            CloseCanvasEnemyStation();
        }

    }
    public void BattleInitialize() // 战斗系统初始化
    {
        AttackerUsedEquipmentIdx = equipmentBagMaxCapacity; //初始化该索引，用于判定玩家是否使用了装备
        DefenderUsedEquipmentIdx = equipmentBagMaxCapacity; //初始化该索引，用于判定玩家是否使用了装备
        BattleDefendButton.interactable = false;  // 禁用防御按钮
        BattleEvadeButton.interactable = false;   // 禁用闪避按钮
        DefenderEquipButton.interactable = false; //禁用防御者使用装备按钮

        BattleAttackButton.onClick.AddListener(() => // 攻击按钮添加监听事件
        {
            diceForAttack.RollDice(); // 摇攻击骰子
            BattleAttackButton.interactable = false; // 禁用攻击按钮
            AttackerEquipButton.interactable = false; // 禁用使用装备按钮
            for (int i = 0; i < equipmentBagMaxCapacity; i++) //禁用背包使用按钮
            {
                useEquipmentButtons[i].gameObject.SetActive(false);
                cancelEquipButton.gameObject.SetActive(false);
            }
            BattleDefendButton.interactable = true; // 启用防御按钮
            BattleEvadeButton.interactable = true;  // 启用闪避按钮
            DefenderEquipButton.interactable = true;// 启用防御者使用装备按钮
            Player attacker = PlayerManager.Instance.currPlayer;
            if (!attacker.BattleTargetIsPlayer)
            {
                AttackerIsEquiped = AttackerUsedEquipmentIdx < equipmentBagMaxCapacity; //记录攻击者是否使用装备
                Invoke("EndBattle", 2.5f); // 如果攻击的是空间站, 等待2.5秒后进行战斗结算，保证骰子动画播放完毕
            }

        });

        AttackerEquipButton.onClick.AddListener(() => //攻击者使用装备按钮添加监听事件
        {
            AttackerEquipButton.interactable = false; // 禁用使用装备按钮
            cancelEquipButton.gameObject.SetActive(true); // 启用‘取消使用’按钮
            Player attacker = PlayerManager.Instance.currPlayer;
            for (int i = 0; i < attacker.playerEquipmentBag.Count; i++)
            {
                useEquipmentButtons[i].onClick.RemoveAllListeners(); // 移除当前背包使用按钮的监听事件
                useEquipmentButtons[i].gameObject.SetActive(true); // 根据背包道具数量启用对应使用按钮
            }
            useEquipmentButtons[0].onClick.AddListener(() => // 点击背包使用按钮，更新战斗界面equipment信息
            {
                AttackerUsedEquipmentIdx = 0; // 记录当前使用的装备背包索引，用于战斗结算
                battleEquipmentImage1.texture = attacker.playerEquipmentBag[0].image.texture;
                battleEquipmentEffect1.text = "ATK: +" + attacker.playerEquipmentBag[0].ATK +
                                                  " DEF: +" + attacker.playerEquipmentBag[0].DEF +
                                                  " EVD: +" + attacker.playerEquipmentBag[0].EVD;
            });
            useEquipmentButtons[1].onClick.AddListener(() => // 点击背包使用按钮，更新战斗界面equipment信息
            {
                AttackerUsedEquipmentIdx = 1; // 记录当前使用的装备背包索引，用于战斗结算
                battleEquipmentImage1.texture = attacker.playerEquipmentBag[1].image.texture;
                battleEquipmentEffect1.text = "ATK: +" + attacker.playerEquipmentBag[1].ATK +
                                              " DEF: +" + attacker.playerEquipmentBag[1].DEF +
                                              " EVD: +" + attacker.playerEquipmentBag[1].EVD;
            });
            useEquipmentButtons[2].onClick.AddListener(() => // 点击背包使用按钮，更新战斗界面equipment信息
            {
                AttackerUsedEquipmentIdx = 2; // 记录当前使用的装备背包索引，用于战斗结算
                battleEquipmentImage1.texture = attacker.playerEquipmentBag[2].image.texture;
                battleEquipmentEffect1.text = "ATK: +" + attacker.playerEquipmentBag[2].ATK +
                                              " DEF: +" + attacker.playerEquipmentBag[2].DEF +
                                              " EVD: +" + attacker.playerEquipmentBag[2].EVD;
            });
        });

        DefenderEquipButton.onClick.AddListener(() => //防御者使用装备按钮添加监听事件
        {
            DefenderEquipButton.interactable = false; // 禁用使用装备按钮
            cancelEquipButton.gameObject.SetActive(true); // 启用‘取消使用’按钮
            Player defender = PlayerManager.Instance.currPlayer.tarPlayer;
            UpdatePlayerBag(defender); // 背包更新为防御者的
            for (int i = 0; i < defender.playerEquipmentBag.Count; i++)
            {
                useEquipmentButtons[i].onClick.RemoveAllListeners(); // 移除当前背包使用按钮的监听事件
                useEquipmentButtons[i].gameObject.SetActive(true); // 根据背包道具数量启用对应使用按钮
            }
            useEquipmentButtons[0].onClick.AddListener(() => // 点击背包使用按钮，更新战斗界面equipment信息
            {
                DefenderUsedEquipmentIdx = 0; // 记录当前使用的装备背包索引，用于战斗结算
                battleEquipmentImage2.texture = defender.playerEquipmentBag[0].image.texture;
                battleEquipmentEffect2.text = "ATK: +" + defender.playerEquipmentBag[0].ATK +
                                              " DEF: +" + defender.playerEquipmentBag[0].DEF +
                                              " EVD: +" + defender.playerEquipmentBag[0].EVD;
            });
            useEquipmentButtons[1].onClick.AddListener(() => // 点击背包使用按钮，更新战斗界面equipment信息
            {
                DefenderUsedEquipmentIdx = 1; // 记录当前使用的装备背包索引，用于战斗结算
                battleEquipmentImage2.texture = defender.playerEquipmentBag[1].image.texture;
                battleEquipmentEffect2.text = "ATK: +" + defender.playerEquipmentBag[1].ATK +
                                              " DEF: +" + defender.playerEquipmentBag[1].DEF +
                                              " EVD: +" + defender.playerEquipmentBag[1].EVD;
            });
            useEquipmentButtons[2].onClick.AddListener(() => // 点击背包使用按钮，更新战斗界面equipment信息
            {
                DefenderUsedEquipmentIdx = 2; // 记录当前使用的装备背包索引，用于战斗结算
                battleEquipmentImage2.texture = defender.playerEquipmentBag[2].image.texture;
                battleEquipmentEffect2.text = "ATK: +" + defender.playerEquipmentBag[2].ATK +
                                              " DEF: +" + defender.playerEquipmentBag[2].DEF +
                                              " EVD: +" + defender.playerEquipmentBag[2].EVD;
            });
        });

        BattleDefendButton.onClick.AddListener(() => // 防御按钮添加监听事件
        {
            diceForDefend.RollDice(); // 摇防御骰子
            BattleDefendButton.interactable = false; // 禁用防御按钮
            BattleEvadeButton.interactable = false; // 禁用闪避按钮
            DefenderEquipButton.interactable = false; // 禁用防御按钮
            for (int i = 0; i < equipmentBagMaxCapacity; i++) useEquipmentButtons[i].gameObject.SetActive(false);//隐藏背包使用按钮
            cancelEquipButton.gameObject.SetActive(false);//隐藏‘取消使用’按钮
            DefenderChooseDefend = true; //记录防御者选择防御
            AttackerIsEquiped = AttackerUsedEquipmentIdx < equipmentBagMaxCapacity; //记录攻击者是否使用装备
            DefenderIsEquiped = DefenderUsedEquipmentIdx < equipmentBagMaxCapacity; //记录防御者是否使用装备
            Invoke("EndBattle", 2.5f); // 等待2.5秒后进行战斗结算，保证骰子动画播放完毕
        });

        BattleEvadeButton.onClick.AddListener(() => // 闪避按钮添加监听事件
        {
            diceForDefend.RollDice(); // 摇防御骰子
            BattleDefendButton.interactable = false; // 禁用防御按钮
            BattleEvadeButton.interactable = false; // 禁用闪避按钮
            DefenderEquipButton.interactable = false; // 禁用防御者使用装备按钮
            for (int i = 0; i < equipmentBagMaxCapacity; i++) useEquipmentButtons[i].gameObject.SetActive(false);//隐藏背包使用按钮
            cancelEquipButton.gameObject.SetActive(false);//隐藏‘取消使用’按钮
            DefenderChooseDefend = false; //记录防御者选择闪避
            AttackerIsEquiped = AttackerUsedEquipmentIdx < equipmentBagMaxCapacity; //记录攻击者是否使用装备
            DefenderIsEquiped = DefenderUsedEquipmentIdx < equipmentBagMaxCapacity; //记录防御者是否使用装备
            Invoke("EndBattle", 2.5f); // 等待2.5秒后进行战斗结算，保证骰子动画播放完毕
        });

        cancelEquipButton.onClick.AddListener(() => // 取消使用按钮添加监听事件
        {
            if (BattleAttackButton.interactable) // 如果是攻击者取消使用
            {
                battleEquipmentImage1.texture = null;
                battleEquipmentEffect1.text = "Not  equips";
                AttackerEquipButton.interactable = true; // 启用使用装备按钮
                AttackerUsedEquipmentIdx = equipmentBagMaxCapacity; // 重新初始化索引
            }
            else // 如果是防御者取消使用
            {
                battleEquipmentImage2.texture = null;
                battleEquipmentEffect2.text = "Not  equips";
                DefenderEquipButton.interactable = true;
                DefenderUsedEquipmentIdx = equipmentBagMaxCapacity; // 重新初始化索引
            }

            for (int i = 0; i < equipmentBagMaxCapacity; i++) useEquipmentButtons[i].gameObject.SetActive(false);//隐藏背包使用按钮
            cancelEquipButton.gameObject.SetActive(false);
        });

    }
    public void TradeStationInitialize() // 商店初始化
    {
        //按图片顺序，添加Item到列表
        itemList.Add(new Item("Fix packs1", 3, "Based on your current health, grant your spaceship a +3 HP value this turn. It consumes three points of energy", 3, itemImages[0]));
        itemList.Add(new Item("Fix packs2", 5, "Based on your current health, grant your spaceship a +5 HP value this turn. It consumes eight points of energy.", 5, itemImages[1]));
        itemList.Add(new Item("Fix packs3", 15, "Based on your current health, grant your spaceship a +8 HP value this turn. It consumes fifteen points of energy", 9, itemImages[2]));
        //添加Equipment到列表
        equipmentList.Add(new Equipment("Ammo1", 3, "Grant your spaceship a +1 attack value this turn. It consumes three points of energy.", 1, 1, 0, 0, equipmentImages[0]));
        equipmentList.Add(new Equipment("Ammo2", 5, "Grant your spaceship a +2 attack value this turn. It consumes five points of energy.", 2, 2, 0, 0, equipmentImages[1]));
        equipmentList.Add(new Equipment("Ammo3", 9, "Grant your spaceship a +3 attack value this turn. It consumes nine points of energy.", 3, 3, 0, 0, equipmentImages[2]));
        equipmentList.Add(new Equipment("Shield1", 3, "Grant your spaceship a +1 defense value this turn. It consumes three points of energy.", 1, 0, 1, 0, equipmentImages[3]));
        equipmentList.Add(new Equipment("Shield2", 5, "Grant your spaceship a +2 defense value this turn. It consumes five points of energy.", 2, 0, 2, 0, equipmentImages[4]));
        equipmentList.Add(new Equipment("Shield3", 7, "Grant your spaceship a +3 defense value this turn. It consumes seven points of energy.", 3, 0, 3, 0, equipmentImages[5]));
        equipmentList.Add(new Equipment("Engine1", 2, "Grant your spaceship a +1 dodge value this turn. It consumes two points of energy.", 1, 0, 0, 1, equipmentImages[6]));
        equipmentList.Add(new Equipment("Engine2", 7, "Grant your spaceship a +2 dodge value this turn. It consumes seven points of energy.", 2, 0, 0, 2, equipmentImages[7]));
        equipmentList.Add(new Equipment("Engine3", 10, "Grant your spaceship a +3 dodge value this turn. It consumes ten points of energy.", 3, 0, 0, 3, equipmentImages[8]));
        //添加SpaceShip到列表
        spaceShipList.Add(new SpaceShip("SpaceShip1", 20, "Replace your current spaceship with the following properties of Level 1 spaceship that costs 20 Energy", 1, 8,  4, 3, 2));
        spaceShipList.Add(new SpaceShip("SpaceShip2", 25, "Replace your current spaceship with the following properties of Level 2 spaceship that costs 25 Energy", 2, 10, 6, 5, 4));
        spaceShipList.Add(new SpaceShip("SpaceShip3", 30, "Replace your current spaceship with the following properties of Level 3 spaceship that costs 30 Energy.", 3, 12, 8, 7, 6));
        spaceShipList.Add(new SpaceShip("SpaceShip1", 20, "Replace your current spaceship with the following properties of Level 1 spaceship that costs 20 Energy", 1, 8, 4, 3, 2));
        spaceShipList.Add(new SpaceShip("SpaceShip2", 25, "Replace your current spaceship with the following properties of Level 2 spaceship that costs 25 Energy", 2, 10, 6, 5, 4));
        spaceShipList.Add(new SpaceShip("SpaceShip3", 30, "Replace your current spaceship with the following properties of Level 3 spaceship that costs 30 Energy.", 3, 12, 8, 7, 6));
        spaceShipList.Add(new SpaceShip("SpaceShip1", 20, "Replace your current spaceship with the following properties of Level 1 spaceship that costs 20 Energy", 1, 8, 4, 3, 2));
        spaceShipList.Add(new SpaceShip("SpaceShip2", 25, "Replace your current spaceship with the following properties of Level 2 spaceship that costs 25 Energy", 2, 10, 6, 5, 4));
        spaceShipList.Add(new SpaceShip("SpaceShip3", 30, "Replace your current spaceship with the following properties of Level 3 spaceship that costs 30 Energy.", 3, 12, 8, 7, 6));
        spaceShipList.Add(new SpaceShip("SpaceShip1", 20, "Replace your current spaceship with the following properties of Level 1 spaceship that costs 20 Energy", 1, 8, 4, 3, 2));
        spaceShipList.Add(new SpaceShip("SpaceShip2", 25, "Replace your current spaceship with the following properties of Level 2 spaceship that costs 25 Energy", 2, 10, 6, 5, 4));
        spaceShipList.Add(new SpaceShip("SpaceShip3", 30, "Replace your current spaceship with the following properties of Level 3 spaceship that costs 30 Energy.", 3, 12, 8, 7, 6));

        // 商店道具按钮添加点击事件
        foreach (Button item in items) item.onClick.AddListener(() =>
        {
            itemHighlight.transform.position = item.transform.position;
            int idx = Array.IndexOf(items, item); // 获取道具按钮下标
            string str = "CanvasTradeStation/PanelForDrag/Items/Detail/"; //商店物品详情的目录
            //点击商店物体后，根据按钮下标，更新右侧物品详情（道具）
            GameObject.Find(str + "Name").GetComponent<Text>().text = itemList[idx].name;
            GameObject.Find(str + "EnergyCost").GetComponent<Text>().text = itemList[idx].energyCost + " Energy";
            GameObject.Find(str + "Description").GetComponent<Text>().text = itemList[idx].description;
            GameObject.Find(str + "Effect").GetComponent<Text>().text =
            "Recovery: HP +" + itemList[idx].HP;
            GameObject.Find(str + "Image").GetComponent<RawImage>().texture = itemList[idx].image.texture;
            // 商店道具购买按钮添加监听事件
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() =>
            {
                Player currentPlayer = PlayerManager.Instance.currPlayer;//获取当前玩家对象
                if(currentPlayer.setEnergy(-itemList[idx].energyCost)) //如果买得起道具
                {
                    if (currentPlayer.playerItemBag.Count == itemBagMaxCapacity) IsConfirm(ConfirmType.itemBagIsFull); //如果背包已满,弹出提示框
                    else
                    {
                        currentPlayer.playerItemBag.Add(itemList[idx]);//放入背包
                        playerPanels[PlayerManager.Instance.currPlayerIndex].transform.GetChild(2).GetComponent<Text>().text 
                         = "Energy: " + currentPlayer.getEnergy(); // 更新玩家能源数据
                    }                                 
                }
                else IsConfirm(ConfirmType.energyNotEnough); //如果能源不够,弹出提示框
                UpdatePlayerBag();//刷新背包
            });
        });

        // 商店战斗装备按钮添加点击事件
        foreach (Button equipment in equipments) equipment.onClick.AddListener(() =>
        {
            equipmentHighlight.transform.position = equipment.transform.position;
            int idx = Array.IndexOf(equipments, equipment);
            string str = "CanvasTradeStation/PanelForDrag/Equipments/Detail/"; //详情的目录
            //点击商店物体后，更新右侧物品详情（战斗装备）
            GameObject.Find(str + "Name").GetComponent<Text>().text = equipmentList[idx].name;
            GameObject.Find(str + "EnergyCost").GetComponent<Text>().text = equipmentList[idx].energyCost + " Energy";
            GameObject.Find(str + "Description").GetComponent<Text>().text = equipmentList[idx].description;
            GameObject.Find(str + "Effect").GetComponent<Text>().text =
            "ATK+" + equipmentList[idx].ATK +
            "  DEF+" + equipmentList[idx].DEF +
            "  EVD+" + equipmentList[idx].EVD;
            GameObject.Find(str + "Image").GetComponent<RawImage>().texture = equipmentList[idx].image.texture;
            // 添加战斗装备购买按钮监听事件
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() =>
            {
                Player currentPlayer = PlayerManager.Instance.currPlayer;//获取当前玩家对象
                if (currentPlayer.setEnergy(-equipmentList[idx].energyCost)) //如果买得起该装备
                {
                    if (currentPlayer.playerEquipmentBag.Count == equipmentBagMaxCapacity) IsConfirm(ConfirmType.equipmentBagIsFull); //如果背包已满,弹出提示框
                    else
                    {
                        currentPlayer.playerEquipmentBag.Add(equipmentList[idx]);//放入背包
                        playerPanels[PlayerManager.Instance.currPlayerIndex].transform.GetChild(2).GetComponent<Text>().text
                         = "Energy: " + currentPlayer.getEnergy(); // 更新玩家能源数据
                    }
                }
                else IsConfirm(ConfirmType.energyNotEnough); //如果能源不够,弹出提示框
                UpdatePlayerBag(); // 刷新背包
            });
        });

        // 商店飞船按钮添加点击事件
        foreach (Button spaceShip in spaceShips) spaceShip.onClick.AddListener(() =>
        {
            spaceShipHighlight.transform.position = spaceShip.transform.position; // 选中飞船高亮显示
            int idx = Array.IndexOf(spaceShips, spaceShip); //获取选中飞船在飞船列表的索引
            string str = "CanvasTradeStation/PanelForDrag/SpaceShips/Detail/"; //详情的目录
            //点击商店物体后，更新右侧物品详情（飞船）
            GameObject.Find(str + "Name").GetComponent<Text>().text = spaceShipList[idx].name;
            GameObject.Find(str + "EnergyCost").GetComponent<Text>().text = spaceShipList[idx].energyCost.ToString() + " Energy";
            GameObject.Find(str + "Description").GetComponent<Text>().text = spaceShipList[idx].description;
            GameObject.Find(str + "Effect").GetComponent<Text>().text =
            "HP+" + spaceShipList[idx].HP +
            "  ATK+" + spaceShipList[idx].ATK +
            "  DEF+" + spaceShipList[idx].DEF +
            "  EVD+" + spaceShipList[idx].EVD;
            GameObject.Find(str + "Image").GetComponent<RawImage>().texture = spaceShip.GetComponent<RawImage>().texture;
            // 添加购买按钮监听事件
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() =>
            {
                Player currentPlayer = PlayerManager.Instance.currPlayer;//获取当前玩家对象
                if (currentPlayer.id*maxSpaceShipOfOneColor>=idx+1 && idx>=(currentPlayer.id-1)*maxSpaceShipOfOneColor) // 如果购买的是对应颜色的飞船
                {
                    if (currentPlayer.setEnergy(-spaceShipList[idx].energyCost)) // 如果买得起
                    {
                        updateSound.Play(); // 播放飞船替换音效
                        playerPanels[currentPlayer.id - 1].transform.GetChild(0).GetComponent<RawImage>().texture = spaceShip.GetComponent<RawImage>().texture;//更新图片
                        currentPlayer.GetNewSpaceShip(spaceShipList[idx].HP, spaceShipList[idx].ATK, spaceShipList[idx].DEF, spaceShipList[idx].EVD);
                        UpdatePlayerPanel(); // 更新玩家面板
                    }
                    else IsConfirm(ConfirmType.energyNotEnough); // 如果能源不够,弹出提示框
                }
                else IsConfirm(ConfirmType.spaceShipPurchaseError); // 如果购买的飞船颜色不对应,弹出提示框

            });
        });


    } 
}
