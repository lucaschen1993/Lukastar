using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JewelsManager : MonoBehaviour {
    private static JewelsManager _instance;
    public GameObject lastSeleted;
    public GameObject currentSeleted;
    public Vector3 lastSeletedPosition;
    public Vector3 currentSeletedPosition;

    public static JewelsManager Instance
    {
        get
        {
            if(_instance==null)
                    _instance = GameObject.Find("JewelsManager").GetComponent<JewelsManager>();
            return _instance;
        }
    }    

    public void ExchangeJewel(Jewel.MoveDirection moveDirection)
    {
        GameManager.Instance.canSelect = false;
        //交换当前两个宝石
        GameManager.Instance.ExchangeJewel(currentSeleted,lastSeleted);
        //Debug.Log("currentSeleted : " + currentSeleted.transform.transform.Find("JewelPicture").GetComponent<Image>().sprite.name + " lastSeleted : " + lastSeleted.transform.transform.Find("JewelPicture").GetComponent<Image>().sprite.name);
        bool isMatch = GameManager.Instance.MatchJewel(currentSeleted, lastSeleted, moveDirection);
        if (!isMatch)  
        {
            AudioManager.Instance.PlayAudio("Audio/sfx_lock");
            //没匹配的情况下，交换回来
            GameManager.Instance.ExchangeJewel(lastSeleted, currentSeleted);
            GameManager.Instance.canSelect = true;
        }
        //重置GameManager的isMatch和canSelect    
        GameManager.Instance.isMatch = false;
        GameManager.Instance.canSelect = true;
        ResetAllJewelSelected();
    }

    public void CurrentTolast(GameObject obj)
    {
        if(currentSeleted!=null)
        lastSeleted = obj;
    }
    public Vector3 GetJewelPosition(GameObject obj)
    {
        return GameManager.Instance.GetJewelPosition(obj);
    }

    internal void CurrentToLastPostion(Vector3 objPosition)
    {
        if (currentSeletedPosition != null)
            lastSeletedPosition = objPosition;
    }

    public void ResetAllJewelSelected()
    {
        lastSeleted = null;
        currentSeleted = null;

        GameManager.Instance.SetAllJewelSelectedIsFalse();
    }
}
