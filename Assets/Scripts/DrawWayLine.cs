using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawWayLine : MonoBehaviour
{
    public Material lineMat; //线段材质 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPostRender()
    {
        GL.PushMatrix();
        lineMat.SetPass(0);

        RenderPath();

        GL.PopMatrix();
    }

    public void RenderPath()
    {

        //生成线
        for (int i = 0; i < Route.Instacnce.childNodeList.Count; i++)
        {
            Vector3 currentPos = Route.Instacnce.childNodeList[i].position;
            if (i > 0)
            {
                Vector3 prevPos = Route.Instacnce.childNodeList[i - 1].position;
                GL.Begin(GL.LINES);
                GL.Color(Color.green);
                GL.Vertex(prevPos);
                GL.Vertex(currentPos);
                GL.End();
            }
        }
    }
}
