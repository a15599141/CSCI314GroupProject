using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    public int routeNum; //格子总数
    public List<Transform> childNodeList = new List<Transform>();// 保存格子对象

    private static Route _instacnce;//单例模式

    public static Route Instacnce
    {
        get
        {
            if (_instacnce == null)
                _instacnce = GameObject.FindWithTag("Route").GetComponent<Route>();
            return _instacnce;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        childNodeList.Clear();

        routeNum = transform.childCount;

        for (int i = 0;i<routeNum;i++)
        {
            childNodeList.Add(transform.GetChild(i));
        }

        for (int i = 0; i < childNodeList.Count; i++)
        {
            Vector3 currentPos = childNodeList[i].position;
            if (i > 0)
            {
                Vector3 prevPos = childNodeList[i - 1].position;
                Gizmos.DrawLine(prevPos, currentPos);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
