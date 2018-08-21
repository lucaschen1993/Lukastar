using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Jewel : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    [SerializeField]
    private Vector3 locationJewel;

    private Transform jewelChild;
    private Vector3 jewelChildOrigin; //保存当前Jewel的原始位置
    public bool isSelected;
    public enum MoveDirection
    {
        moveUp,
        moveDown,
        moveLeft,
        moveRight,
    }

    private void Start()
    {
        jewelChild = transform.Find("JewelPicture");
        jewelChildOrigin = transform.position;
        isSelected = false;
    }
    private void Update()
    {
        LocationJewelPosition();
    }

    private void LocationJewelPosition()
    {
        locationJewel = GameManager.Instance.GetJewelPosition(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!isSelected)
        transform.Find("Select").gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!isSelected)
            transform.Find("Select").gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(GameManager.Instance.canSelect)
        {
            AudioManager.Instance.PlayAudio("Audio/sfx_click");
            if (!isSelected)
            {
                //选中宝石
                isSelected = true;
                transform.Find("Select").gameObject.SetActive(true);

                JewelsManager.Instance.currentSeleted = gameObject;
                JewelsManager.Instance.currentSeletedPosition = JewelsManager.Instance.GetJewelPosition(gameObject);
                //选中宝石以后的处理
                if (JewelsManager.Instance.lastSeleted != null)
                {
                    Vector3 lastPosition = JewelsManager.Instance.lastSeletedPosition;
                    Vector3 currentPosition = JewelsManager.Instance.currentSeletedPosition;
                    //Debug.Log("last: "+lastPosition + "    current: " + currentPosition);

                    // 判断宝石的移动是否符合游戏的规则
                    if (lastPosition.x + 1 == currentPosition.x && lastPosition.y == currentPosition.y)
                    {
                        //符合规则
                        JewelsManager.Instance.ExchangeJewel(MoveDirection.moveDown);
                    }
                    else if (lastPosition.x - 1 == currentPosition.x && lastPosition.y == currentPosition.y)
                    {
                        //符合规则
                        JewelsManager.Instance.ExchangeJewel(MoveDirection.moveUp);
                    }
                    else if (lastPosition.y + 1 == currentPosition.y && lastPosition.x == currentPosition.x)
                    {
                        //符合规则
                        JewelsManager.Instance.ExchangeJewel(MoveDirection.moveRight);
                    }
                    else if (lastPosition.y - 1 == currentPosition.y && lastPosition.x == currentPosition.x)
                    {
                        //符合规则
                        JewelsManager.Instance.ExchangeJewel(MoveDirection.moveLeft);
                    }
                    else
                    {
                        //不符合规则
                        JewelsManager.Instance.ResetAllJewelSelected();
                    }
                }
                if (JewelsManager.Instance.lastSeleted == null)
                {
                    JewelsManager.Instance.CurrentTolast(gameObject);
                    JewelsManager.Instance.CurrentToLastPostion(JewelsManager.Instance.currentSeletedPosition);
                    JewelsManager.Instance.currentSeleted = null;
                }
            }
            else if (isSelected)
            {
                transform.Find("Select").gameObject.SetActive(false);
                JewelsManager.Instance.lastSeleted = gameObject;
                isSelected = false;
                JewelsManager.Instance.ResetAllJewelSelected();
            }
        }        
    }
}
