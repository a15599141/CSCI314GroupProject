using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler // support UI free dragging inside the screen
{
    /// <summary>
    /// offset of dragging
    /// </summary>
    Vector3 offset;

    RectTransform rt;
    Vector3 pos;
    float minWidth;             //minimun dragging width
    float maxWidth;            //maxium dragging width
    float minHeight;            //minimun dragging height
    float maxHeight;            //maxium dragging height
    float rangeX;               //horizontal dragging area 
    float rangeY;               //vertical dragging area

    void Update()
    {
        DragRangeLimit();
    }

    void Start()
    {
        rt = GetComponent<RectTransform>();
        pos = rt.position;

        minWidth = rt.rect.width / 2;
        maxWidth = Screen.width - (rt.rect.width / 2);
        minHeight = rt.rect.height / 2;
        maxHeight = Screen.height - (rt.rect.height / 2);
    }

    /// <summary>
    /// Drag Range Limit
    /// </summary>
    void DragRangeLimit()
    {
        //limit the dragging area 
        rangeX = Mathf.Clamp(rt.position.x, minWidth, maxWidth);
        rangeY = Mathf.Clamp(rt.position.y, minHeight, maxHeight);
        //update the position after drag
        rt.position = new Vector3(rangeX, rangeY, 0);
    }

    /// <summary>
    ///  start drag
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector3 globalMousePos;

        //transfer local position coordinates to world coordinates
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, eventData.position, null, out globalMousePos))
        {
            //calculate the offset between UI and pointer
            offset = rt.position - globalMousePos;
        }
    }

    /// <summary>
    /// when dragging
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
    }

    /// <summary>
    /// end of drag
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
       
    }

    /// <summary>
    /// update position after drag
    /// </summary>
    private void SetDraggedPosition(PointerEventData eventData)
    {
        Vector3 globalMousePos;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, eventData.position, null, out globalMousePos))
        {
            rt.position = offset + globalMousePos;
        }
    }
}