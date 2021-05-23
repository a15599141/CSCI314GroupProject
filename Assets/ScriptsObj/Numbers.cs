using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Numbers : MonoBehaviour
{
    public GameObject Num1, Num2, Num3, Num4, Num5, Num6;
    public GameObject Dice;
    public int DiceFaceUpNum;
    // Start is called before the first frame update
    void Start()
    {
        setAllFalse();
    }
    
    // Update is called once per frame
    void Update()
    {
        /*DiceFaceUpNum = Dice.GetComponent<Dice>().GetNum();
        setAllFalse();
        if (DiceFaceUpNum == 1)
        {
            Num1.SetActive(true);
        }
        if (DiceFaceUpNum == 2)
        {
            Num2.SetActive(true);
        }
        if (DiceFaceUpNum == 3)
        {
            Num3.SetActive(true);
        }
        if (DiceFaceUpNum == 4)
        {
            Num4.SetActive(true);
        }
        if (DiceFaceUpNum == 5)
        {
            Num5.SetActive(true);
        }
        if (DiceFaceUpNum == 6)
        {
            Num6.SetActive(true);
        }*/
    }

    void setAllFalse()
    {
        Num1.SetActive(false);
        Num2.SetActive(false);
        Num3.SetActive(false);
        Num4.SetActive(false);
        Num5.SetActive(false);
        Num6.SetActive(false);
    }
}
