using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JewelsManager : MonoBehaviour {
    private static JewelsManager _instance;
    public GameObject lastSelected;
    public GameObject currentSeleted;
    public Vector3 lastSelectedPosition;
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
    private bool isReset;

    private void Start()
    {
        isReset = false;
    }
    //交换宝石
    public void CallExchangeJewel()
    {
        bool isConnected =  IsConnected(currentSeleted, lastSelected);
        //相邻才能交换
        if (isConnected)
        {
            StartCoroutine(ExchangeJewel(currentSeleted, lastSelected));
        }
    }

    IEnumerator ExchangeJewel(GameObject obj1, GameObject obj2)
    {
        GameManager.Instance.canSelect = false;
        if (obj1 != obj2)
        {
            GameObject tempObj = obj1;
            //DoTween制作宝石交换动画
            obj1.GetComponent<Jewel>().Move(obj2);
            obj2.GetComponent<Jewel>().Move(tempObj);
        }
        yield return new WaitForSeconds(0.4f);
        GameManager.Instance.canSelect = true;
        if(!isReset)
        {
            GameManager.Instance.isExchangeDone = true;
        }
        isReset = false;
    }

    public void ResetOriginJewel()
    {
        isReset = true;
        AudioManager.Instance.PlayAudio("Audio/sfx_lock");
        StartCoroutine(ExchangeJewel(lastSelected, currentSeleted));
        ResetAllJewelSelected();
    }
    
    public void ResetAllJewelSelected()
    {
        lastSelected = currentSeleted = null;
        GameManager.Instance.SetAllJewelSelectedIsFalse();
    }

    public bool IsConnected(GameObject obj1,GameObject obj2)
    {
        Vector3 obj1Position = GameManager.Instance.GetJewelPosition(obj1);
        Vector3 obj2Position = GameManager.Instance.GetJewelPosition(obj2);
        return ((obj1Position.x == obj2Position.x && Mathf.Abs(obj1Position.y - obj2Position.y) == 1) || (obj1Position.y == obj2Position.y && Mathf.Abs(obj1Position.x - obj2Position.x) == 1)) ? true : false;
    }
}
