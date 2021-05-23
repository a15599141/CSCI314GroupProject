using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceForBattle : MonoBehaviour
{
    public int diceNumber; //骰子点数

    public void RollDice()
    {
        diceNumber = Random.Range(1, 7); // 生成1到6的随机整数，作为最后的骰子点数
        GetComponent<Animator>().Play("Rotate to " + diceNumber.ToString(), 0);// 根据点数播放骰子相应动画
    }
}
