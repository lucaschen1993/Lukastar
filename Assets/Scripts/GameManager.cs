using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System;

public class GameManager : MonoBehaviour {

    //public Sprite[] jewelSprites;
    public bool canSelect;
    private float xOffset = 100f;
    private float yOffset = -100f;
    public GameObject jewelPrefab;
    private GameObject gamePlayPanel;
    private GameObject[,] jewelObjects;
    private int rowSize, colSize;
    public GameObject mainMenu;
    List<GameObject> needRemoveJewels;
    private Sprite[] jewelSprites;
    private List<GameObject> reMatchObjects;
    #region GameManager 单例模式 Instance()
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.Find("GameManager").GetComponent<GameManager>();
            return _instance;
        }
    }
    #endregion

    public bool isMatch;
    public bool isExchangeDone;

    // Use this for initialization
    void Start() {
        rowSize = 6;
        colSize = 6;
        isMatch = false;
        canSelect = true;
        isExchangeDone = false;
        needRemoveJewels = new List<GameObject>();
        jewelObjects = new GameObject[rowSize, colSize];
        jewelSprites = Resources.LoadAll<Sprite>("Graphics/Jewels");
        reMatchObjects = new List<GameObject>();
        gamePlayPanel = GameObject.Find("/UICanvas/GamePlay/GamePlayPanel");
    }

    // Update is called once per frame
    void Update() {
        if(isExchangeDone == true)
        {
            isExchangeDone = false;
            MatchRule();
        }
        if(isMatch)
        {
            isMatch = false;
            JewelsManager.Instance.ResetAllJewelSelected();
            RemoveJewel(needRemoveJewels);
        }
    }

    private void MatchRule()
    {
        GameObject lastSelected = JewelsManager.Instance.lastSelected;
        GameObject currentSeleted = JewelsManager.Instance.currentSeleted;
        Vector3 lastPosition = GetJewelPosition(lastSelected);
        Vector3 currentPosition = GetJewelPosition(currentSeleted);

        // 判断宝石的移动是否符合游戏的规则
        Jewel.MoveDirection moveDirection = Jewel.MoveDirection.noMove;
        #region 移动规则
        //符合下移规则
        if (lastPosition.x + 1 == currentPosition.x && lastPosition.y == currentPosition.y)
        {
            moveDirection = Jewel.MoveDirection.moveDown;
        }
        //符合上移规则
        else if (lastPosition.x - 1 == currentPosition.x && lastPosition.y == currentPosition.y)
        {
            moveDirection = Jewel.MoveDirection.moveUp;
        }
        //符合右移规则
        else if (lastPosition.y + 1 == currentPosition.y && lastPosition.x == currentPosition.x)
        {
            moveDirection = Jewel.MoveDirection.moveRight;
        }
        //符合左移规则
        else if (lastPosition.y - 1 == currentPosition.y && lastPosition.x == currentPosition.x)
        {
            moveDirection = Jewel.MoveDirection.moveLeft;
        }
        //判断是否匹配
        StartCoroutine(MatchJewel(currentSeleted, lastSelected, moveDirection));
        #endregion
    }

    void InitializedGame() {
        int id = 0;
        for (int i = 0; i < rowSize; i++)
        {
            for (int j = 0; j < colSize; j++)
            {
                List<int> equalLists = new List<int>();
                equalLists.Clear();
                List<int> remainIndex = new List<int>();
                remainIndex.Clear();
                //把sprite[]转化为int list
                for (int k = 0; k < jewelSprites.Length; k++)
                {
                    remainIndex.Add(int.Parse(jewelSprites[k].name));
                }
                //把相同的放进数组equalList中
                if (j >= 2 && string.Equals(jewelObjects[i, j - 2].gameObject.transform.Find("JewelPicture").GetComponent<Image>().sprite.name, jewelObjects[i, j - 1].gameObject.transform.Find("JewelPicture").GetComponent<Image>().sprite.name))
                {
                    equalLists.Add(int.Parse(jewelObjects[i, j - 2].gameObject.transform.Find("JewelPicture").GetComponent<Image>().sprite.name));
                }
                if (i >= 2 && string.Equals(jewelObjects[i - 2, j].gameObject.transform.Find("JewelPicture").GetComponent<Image>().sprite.name, jewelObjects[i - 1, j].gameObject.transform.Find("JewelPicture").GetComponent<Image>().sprite.name))
                {
                    equalLists.Add(int.Parse(jewelObjects[i - 2, j].gameObject.transform.Find("JewelPicture").GetComponent<Image>().sprite.name));
                }
                //判断equasList中的元素是否相同，若相同则删除一个
                if (equalLists != null && equalLists.Count == 2 && equalLists[0] == equalLists[1])
                {
                    equalLists.Remove(equalLists[1]);
                }
                //移除remainIndex里面与equalLists相同的元素
                for (int o = 0; o < equalLists.Count; o++)
                {
                    for (int p = 0; p < remainIndex.Count; p++)
                    {
                        if (equalLists[o] == remainIndex[p])
                        {
                            remainIndex.Remove(remainIndex[p]);
                            break;
                        }
                    }
                }
                jewelPrefab.transform.Find("JewelPicture").GetComponent<Image>().sprite = jewelSprites[remainIndex[UnityEngine.Random.Range(0, remainIndex.Count)] - 1];
                GameObject itemObject = GameObject.Instantiate<GameObject>(jewelPrefab, gamePlayPanel.transform, false);
                itemObject.GetComponent<RectTransform>().localPosition = new Vector2(-250f + j * xOffset, 250f + i * yOffset);
                //Debug.Log(i + " " + j);
                itemObject.transform.Find("JewelPicture").GetComponent<JewelPicture>().identityCode = id;
                jewelObjects[i, j] = itemObject;
                id++;
            }
        }
    }

    IEnumerator StartReInitializedJewels()
    {
        yield return new WaitForSeconds(0.1f);
        int reNeedCount = 0;
        //再次生成宝石
        //1.遍历第一行
        for (int i = 0; i < colSize; i++)
        {
            //Debug.Log(jewelObjects[0, i].transform.Find("JewelPicture").GetComponent<Image>().sprite.name);
            if (!jewelObjects[0, i].transform.Find("JewelPicture"))
            {
                reNeedCount++;
                //2.若当前格子为空，则向下继续寻找空的格子，直到找到下一个格子不为空，把空的格子统计为reNeedCount
                for (int j = 1; j < rowSize; j++)
                {
                    if (jewelObjects[j, i].transform.Find("JewelPicture") == null)
                    {
                        //Debug.Log("find: " + i);                        
                        reNeedCount++;
                    }
                    else
                        break;
                }
            }
            //3.给最下面的空格子生成一个新的宝石，然后做个循环向上填满该列的格子
            if (reNeedCount > 0)
            {
                int indexOffset = reNeedCount;
                for (int j = reNeedCount - 1; j >= 0; j--)
                {
                    Transform initializedChild = Instantiate(jewelPrefab.transform.Find("JewelPicture"), gamePlayPanel.transform);
                    initializedChild.name = "JewelPicture";
                    initializedChild.GetComponent<Image>().sprite = jewelSprites[UnityEngine.Random.Range(1, 8)];
                    //生成的宝石的坐标类似于一列
                    initializedChild.localPosition = new Vector3(-250f + i * xOffset, 250f - (indexOffset - j) * yOffset, 0);
                    initializedChild.transform.SetParent(jewelObjects[j, i].transform);
                    initializedChild.DOLocalMove(Vector3.zero, 0.3f);
                    //reMatchObjects.Add(jewelObjects[j, i]);
                }
            }
            //4.跳到下一列
            reNeedCount = 0;
        }
        if (reMatchObjects.Count > 0)
        {
            //重新匹配游戏盘
            CallReMatchGameBorad();
        }
    }

    private void CallReMatchGameBorad()
    {
        StartCoroutine(ReMatchGameBorad());
    }

    IEnumerator ReMatchGameBorad()
    {
        isMatch = false;
        yield return new WaitForSeconds(1f);
        for (int p = 0; p < reMatchObjects.Count; p++)
        {
            //有问题
            //reMatchObjects[p].transform.Find("JewelPicture").GetComponent<Image>().sprite  为空
            //Debug.Log(GetJewelPosition(reMatchObjects[p]) + " " + reMatchObjects[p].transform.Find("JewelPicture").GetComponent<Image>().sprite.name);
            int colMatchCount = 1;
            int rowMatchCount = 1;
            List<GameObject> colMatchLists = new List<GameObject>();
            List<GameObject> rowMatchLists = new List<GameObject>();
            if (reMatchObjects[p].transform.Find("JewelPicture"))
            {
                Vector3 jewelPosition = GetJewelPosition(reMatchObjects[p]);
                GameObject currentJewel = reMatchObjects[p];
                int x = (int)jewelPosition.x;
                int y = (int)jewelPosition.y;
                //向下匹配
                for (int i = x + 1; i < rowSize; i++)
                {
                    if (jewelObjects[i, y].transform.Find("JewelPicture") != null && string.Equals(jewelObjects[i, y].transform.Find("JewelPicture").GetComponent<Image>().sprite.name, currentJewel.transform.Find("JewelPicture").GetComponent<Image>().sprite.name))
                    {
                        rowMatchLists.Add(jewelObjects[i, y]);
                        rowMatchCount++;
                    }
                    else
                        break;
                }
                //向上匹配
                for (int i = x - 1; i >= 0; i--)
                {
                    if (jewelObjects[i, y].transform.Find("JewelPicture") != null && string.Equals(jewelObjects[i, y].transform.Find("JewelPicture").GetComponent<Image>().sprite.name, currentJewel.transform.Find("JewelPicture").GetComponent<Image>().sprite.name))
                    {
                        rowMatchLists.Add(jewelObjects[i, y]);
                        rowMatchCount++;
                    }
                    else
                        break;
                }
                //向左匹配
                for (int i = y - 1; i >= 0; i--)
                {
                    if (jewelObjects[x, i].transform.Find("JewelPicture") != null && string.Equals(jewelObjects[x, i].transform.Find("JewelPicture").GetComponent<Image>().sprite.name, currentJewel.transform.Find("JewelPicture").GetComponent<Image>().sprite.name))
                    {
                        colMatchLists.Add(jewelObjects[x, i]);
                        colMatchCount++;
                    }
                    else
                        break;
                }
                //向右匹配
                for (int i = y + 1; i < colSize; i++)
                {
                    if (jewelObjects[x, i].transform.Find("JewelPicture") != null && string.Equals(jewelObjects[x, i].transform.Find("JewelPicture").GetComponent<Image>().sprite.name, currentJewel.transform.Find("JewelPicture").GetComponent<Image>().sprite.name))
                    {
                        colMatchLists.Add(jewelObjects[x, i]);
                        colMatchCount++;
                    }
                    else
                        break;
                }
                //Debug.Log(GetJewelPosition(reMatchObjects[p]) +":  "+colMatchCount);
                //判断是否能消除
                if (colMatchCount >= 3 && rowMatchCount >= 3)
                {
                    needRemoveJewels.Add(reMatchObjects[p]);
                    reMatchObjects.Remove(reMatchObjects[p]);
                    for (int i = 0; i < colMatchLists.Count; i++)
                    {
                        needRemoveJewels.Add(colMatchLists[i]);
                        if(reMatchObjects.Contains(colMatchLists[i]))
                        {
                            reMatchObjects.Remove(colMatchLists[i]);
                        }
                    }
                    for (int i = 0; i < rowMatchLists.Count; i++)
                    {
                        needRemoveJewels.Add(rowMatchLists[i]);
                        if (reMatchObjects.Contains(rowMatchLists[i]))
                        {
                            reMatchObjects.Remove(rowMatchLists[i]);
                        }
                    }
                }
                else if (colMatchCount >= 3)
                {
                    needRemoveJewels.Add(reMatchObjects[p]);
                    for (int i = 0; i < colMatchLists.Count; i++)
                    {
                        needRemoveJewels.Add(colMatchLists[i]);
                        if (reMatchObjects.Contains(colMatchLists[i]))
                        {
                            reMatchObjects.Remove(colMatchLists[i]);
                        }
                    }
                }
                else if (rowMatchCount >= 3)
                {
                    needRemoveJewels.Add(reMatchObjects[p]);
                    for (int i = 0; i < rowMatchLists.Count; i++)
                    {
                        needRemoveJewels.Add(rowMatchLists[i]);
                        if (reMatchObjects.Contains(rowMatchLists[i]))
                        {
                            reMatchObjects.Remove(rowMatchLists[i]);
                        }
                    }
                }
                //Debug.Log(currentJewel.transform.Find("JewelPicture").GetComponent<Image>().sprite.name);
                //播放爆炸的声音
                AudioManager.Instance.PlayAudio("Audio/boom");
                isMatch = true;
            }
            else
            {
                continue;
            }
        }
            
        reMatchObjects.Clear();
        //先下移宝石
        //CallMoveDownJewels();
        //再重新生成宝石
        ReInitializedJewels();
}


    //重新生成宝石
    public void ReInitializedJewels()
    {
        StartCoroutine(StartReInitializedJewels());
    }
    //宝石下移操作
    internal void CallMoveDownJewels()
    {
        StartCoroutine(StartMoveDownJewels());
    }

    IEnumerator StartMoveDownJewels()
    {
        yield return new WaitForSeconds(0.1f);
        //自下而上遍历整个游戏棋盘，遇到空的向上遍历把上面的sprite拿给自己
        for (int i = rowSize - 1; i > 0; i--)
        {
            for (int j = 0; j < colSize; j++)
            {
                //Debug.Log(GetJewelPosition(jewelObjects[i, j])+" "+jewelObjects[i, j].transform.Find("JewelPicture").GetComponent<Image>().sprite);
                //当前格子为空，则不断往上找不为空的格子并把它的sprite赋值给当前格子，则找到的格子赋值为空
                if (jewelObjects[i, j].transform.Find("JewelPicture") == null)
                {
                    reMatchObjects.Add(jewelObjects[i, j]);
                    for (int k =i; k >= 0; k--)
                    {
                        if (jewelObjects[k, j].transform.Find("JewelPicture") != null)
                        {
                            //Debug.Log("拿");
                            //当前对象
                            GameObject thisObj = jewelObjects[i, j];
                            //当前对象上面其中一个对象
                            GameObject topObj = jewelObjects[k, j];
                            GameObject topObjChild = topObj.transform.Find("JewelPicture").gameObject;
                            topObjChild.transform.SetParent(thisObj.transform);
                            topObjChild.transform.DOLocalMove(Vector3.zero, 0.3f);
                            //移动了需要重新匹配的对象添加到reMatchObjects中
                            reMatchObjects.Add(jewelObjects[k, j]);
                            break;
                        }
                    }
                }
            }
        }
    }
    //匹配宝石
    IEnumerator MatchJewel(GameObject current, GameObject last, Jewel.MoveDirection moveDirection)
    {
        isMatch = false;
        yield return new WaitForEndOfFrame();
        #region MatchJewel的参数
        //添加找到current需要的对象
        int rowCurrentCount = 1;
        int colCurrentCount = 1;
        List<GameObject> rowCurrentCountObjs = new List<GameObject>();
        rowCurrentCountObjs.Clear();
        List<GameObject> colCurrentCountObjs = new List<GameObject>();
        colCurrentCountObjs.Clear();

        //添加找到last需要的对象
        int rowLastCount = 1;
        int colLastCount = 1;
        List<GameObject> rowLastCountObjs = new List<GameObject>();
        rowLastCountObjs.Clear();
        List<GameObject> colLastCountObjs = new List<GameObject>();
        colLastCountObjs.Clear();

        //找到current对应的二维数组坐标
        Vector3 currentObjPosition = GetJewelPosition(current);
        int ca = (int)currentObjPosition.x;
        int cb = (int)currentObjPosition.y;
        GameObject currentObj = jewelObjects[ca, cb];

        //找到last对应的二维数组坐标
        Vector3 lastObjPosition = GetJewelPosition(last);
        int la = (int)lastObjPosition.x;
        int lb = (int)lastObjPosition.y;
        GameObject lastObj = jewelObjects[la, lb];
        #endregion
        //Debug.Log("currentObj : " + current.transform.Find("JewelPicture").GetComponent<Image>().sprite.name + " lastObj : " + lastObj.transform.Find("JewelPicture").GetComponent<Image>().sprite.name);

        //两个宝石的匹配算法
        //Debug.Log(moveDirection);
        switch (moveDirection)
        {
            case Jewel.MoveDirection.moveUp:
                //current : up, left, right  ; 
                #region current 上移匹配
                //横向扫描->向右
                for (int i = cb + 1; i <= cb + 2; i++)
                {
                    if (i < colSize && jewelObjects[ca, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(currentObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[ca, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        colCurrentCountObjs.Add(jewelObjects[ca, i]);
                        colCurrentCount++;
                    }
                    else
                        break;
                }
                //横向扫描->向左
                for (int i = cb - 1; i >= cb - 2; i--)
                {
                    if (i >= 0 && jewelObjects[ca, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(currentObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[ca, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        colCurrentCountObjs.Add(jewelObjects[ca, i]);
                        colCurrentCount++;
                    }
                    else
                        break;
                }
                //纵向扫描->向上
                for (int j = ca - 1; j >= ca - 2; j--)
                {
                    if (j >= 0 && jewelObjects[j, cb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(currentObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[j, cb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        rowCurrentCountObjs.Add(jewelObjects[j, cb]);
                        rowCurrentCount++;
                    }
                    else
                        break;
                }
                #endregion
                //last : left, right, down ;
                #region last 下移匹配
                //横向扫描->向右
                for (int i = lb + 1; i <= lb + 2; i++)
                {
                    if (i < colSize && jewelObjects[la, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(lastObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[la, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        colLastCountObjs.Add(jewelObjects[la, i]);
                        colLastCount++;
                    }
                    else
                        break;
                }
                //横向扫描->向左
                for (int i = lb - 1; i >= lb - 2; i--)
                {
                    if (i >= 0 && jewelObjects[la, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(lastObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[la, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        colLastCountObjs.Add(jewelObjects[la, i]);
                        colLastCount++;
                    }
                    else
                        break;
                }
                //纵向扫描->向下
                for (int j = la + 1; j <= la + 2; j++)
                {
                    if (j < rowSize && jewelObjects[j, lb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(lastObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[j, lb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        rowLastCountObjs.Add(jewelObjects[j, lb]);
                        rowLastCount++;
                    }
                    else
                        break;
                }
                #endregion
                break;
            case Jewel.MoveDirection.moveDown:
                //current : left, right, down ;
                #region current 下移匹配
                //横向扫描->向右
                for (int i = cb + 1; i <= cb + 2; i++)
                {
                    if (i < colSize && jewelObjects[ca, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(currentObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[ca, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        colCurrentCountObjs.Add(jewelObjects[ca, i]);
                        colCurrentCount++;
                    }
                    else
                        break;
                }
                //横向扫描->向左
                for (int i = cb - 1; i >= cb - 2; i--)
                {
                    if (i >= 0 && jewelObjects[ca, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(currentObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[ca, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        colCurrentCountObjs.Add(jewelObjects[ca, i]);
                        colCurrentCount++;
                    }
                    else
                        break;
                }
                //纵向扫描->向下
                for (int j = ca + 1; j <= ca + 2; j++)
                {
                    if (j < rowSize && jewelObjects[j, cb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(currentObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[j, cb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        rowCurrentCountObjs.Add(jewelObjects[j, cb]);
                        rowCurrentCount++;
                    }
                    else
                        break;
                }
                #endregion
                //last : up, left, right ;
                #region last 上移匹配
                //横向扫描->向右
                for (int i = lb + 1; i <= lb + 2; i++)
                {
                    if (i < colSize && jewelObjects[la, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(lastObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[la, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        colLastCountObjs.Add(jewelObjects[la, i]);
                        colLastCount++;
                    }
                    else
                        break;
                }
                //横向扫描->向左
                for (int i = lb - 1; i >= lb - 2; i--)
                {
                    if (i >= 0 && jewelObjects[la, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(lastObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[la, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        colLastCountObjs.Add(jewelObjects[la, i]);
                        colLastCount++;
                    }
                    else
                        break;
                }
                //纵向扫描->向上
                for (int j = la - 1; j >= la - 2; j--)
                {
                    if (j >= 0 && jewelObjects[j, lb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(lastObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[j, lb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        rowLastCountObjs.Add(jewelObjects[j, lb]);
                        rowLastCount++;
                    }
                    else
                        break;
                }
                #endregion
                break;
            case Jewel.MoveDirection.moveLeft:
                //current : up, left, down ; 
                #region current 左移匹配
                //横向扫描->向左
                for (int i = cb - 1; i >= cb - 2; i--)
                {
                    if (i >= 0 && jewelObjects[ca, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(currentObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[ca, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        colCurrentCountObjs.Add(jewelObjects[ca, i]);
                        colCurrentCount++;
                    }
                    else
                        break;
                }
                //横向扫描->向上
                for (int j = ca - 1; j >= ca - 2; j--)
                {
                    if (j >= 0 && jewelObjects[j, cb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(currentObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[j, cb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        rowCurrentCountObjs.Add(jewelObjects[j, cb]);
                        rowCurrentCount++;
                    }
                    else
                        break;
                }
                //横向扫描->向下
                for (int j = ca + 1; j <= ca + 2; j++)
                {
                    if (j < rowSize && jewelObjects[j, cb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(currentObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[j, cb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        rowCurrentCountObjs.Add(jewelObjects[j, cb]);
                        rowCurrentCount++;
                    }
                    else
                        break;
                }
                #endregion
                //last : up, right, down ; 
                #region last 右移匹配
                //纵向扫描->向上
                for (int j = la - 1; j >= la - 2; j--)
                {
                    if (j >= 0 && jewelObjects[j, lb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(lastObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[j, lb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        rowLastCountObjs.Add(jewelObjects[j, lb]);
                        rowLastCount++;
                    }
                    else
                        break;
                }
                //纵向扫描->向下
                for (int j = la + 1; j <= la + 2; j++)
                {
                    if (j < rowSize && jewelObjects[j, lb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(lastObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[j, lb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        rowLastCountObjs.Add(jewelObjects[j, lb]);
                        rowLastCount++;
                    }
                    else
                        break;
                }
                //横向扫描->向右
                for (int i = lb + 1; i <= lb + 2; i++)
                {
                    if (i < colSize && jewelObjects[la, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(lastObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[la, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        colLastCountObjs.Add(jewelObjects[la, i]);
                        colLastCount++;
                    }
                    else
                        break;
                }
                #endregion
                break;
            case Jewel.MoveDirection.moveRight:
                //current : up, right , down ; 
                #region current 右移匹配
                for (int i = cb + 1; i <= cb + 2; i++)
                {
                    if (i < colSize && jewelObjects[ca, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(currentObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[ca, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        colCurrentCountObjs.Add(jewelObjects[ca, i]);
                        colCurrentCount++;
                    }
                    else
                        break;
                }
                //纵向扫描->向上
                for (int j = ca - 1; j >= ca - 2; j--)
                {
                    if (j >= 0 && jewelObjects[j, cb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(currentObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[j, cb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        rowCurrentCountObjs.Add(jewelObjects[j, cb]);
                        rowCurrentCount++;
                    }
                    else
                        break;
                }
                //纵向扫描->向下
                for (int j = ca + 1; j <= ca + 2; j++)
                {
                    if (j < rowSize && jewelObjects[j, cb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(currentObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[j, cb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        rowCurrentCountObjs.Add(jewelObjects[j, cb]);
                        rowCurrentCount++;
                    }
                    else
                        break;
                }
                #endregion
                //last : up , left, down ;
                #region last 左移匹配
                //横向扫描->向左
                for (int i = lb - 1; i >= lb - 2; i--)
                {
                    if (i >= 0 && jewelObjects[la, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(lastObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[la, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        colLastCountObjs.Add(jewelObjects[la, i]);
                        colLastCount++;
                    }
                    else
                        break;
                }
                //纵向扫描->向上
                for (int j = la - 1; j >= la - 2; j--)
                {
                    if (j >= 0 && jewelObjects[j, lb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(lastObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[j, lb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        rowLastCountObjs.Add(jewelObjects[j, lb]);
                        rowLastCount++;
                    }
                    else
                        break;
                }
                //纵向扫描->向下
                for (int j = la + 1; j <= la + 2; j++)
                {
                    if (j < rowSize && jewelObjects[j, lb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name != null && string.Equals(lastObj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[j, lb].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
                    {
                        rowLastCountObjs.Add(jewelObjects[j, lb]);
                        rowLastCount++;
                    }
                    else
                        break;
                }
                #endregion
                break;
            default:
                break;
        }

        //Match判断
        //Debug.Log("colCurrentCount: "+colCurrentCount + " " + "rowCurrentCount: " + rowCurrentCount + " " + "colLastCount:"+colLastCount + " " + "rowLastCount: "+rowLastCount);
        if (colLastCount >= 3 || rowLastCount >= 3 || colCurrentCount >= 3 || rowCurrentCount >= 3)
        {
            // currentObj Match判断
            //T型或者L型
            if (colCurrentCount >= 3 && rowCurrentCount >= 3)
            {
                needRemoveJewels.Add(currentObj.transform.Find("JewelPicture").gameObject);
                for (int i = 0; i < rowCurrentCountObjs.Count; i++)
                {
                    needRemoveJewels.Add(rowCurrentCountObjs[i].transform.Find("JewelPicture").gameObject);
                }
                for (int i = 0; i < colCurrentCountObjs.Count; i++)
                {
                    needRemoveJewels.Add(colCurrentCountObjs[i].transform.Find("JewelPicture").gameObject);
                }
            }
            else if (colCurrentCount >= 3)
            {
                needRemoveJewels.Add(currentObj.transform.Find("JewelPicture").gameObject);
                for (int i = 0; i < colCurrentCountObjs.Count; i++)
                {
                    needRemoveJewels.Add(colCurrentCountObjs[i].transform.Find("JewelPicture").gameObject);
                }
            }
            else if (rowCurrentCount >= 3)
            {
                needRemoveJewels.Add(currentObj.transform.Find("JewelPicture").gameObject);
                for (int i = 0; i < rowCurrentCountObjs.Count; i++)
                {
                    needRemoveJewels.Add(rowCurrentCountObjs[i].transform.Find("JewelPicture").gameObject);
                }
            }
            // lastObj Match判断
            if (colLastCount >= 3 && rowLastCount >= 3)
            {
                needRemoveJewels.Add(lastObj.transform.Find("JewelPicture").gameObject);
                for (int i = 0; i < rowLastCountObjs.Count; i++)
                {
                    needRemoveJewels.Add(rowLastCountObjs[i].transform.Find("JewelPicture").gameObject);
                }
                for (int i = 0; i < colLastCountObjs.Count; i++)
                {
                    needRemoveJewels.Add(colLastCountObjs[i].transform.Find("JewelPicture").gameObject);
                }
            }
            else if (colLastCount >= 3)
            {
                needRemoveJewels.Add(lastObj.transform.Find("JewelPicture").gameObject);
                for (int i = 0; i < colLastCountObjs.Count; i++)
                {
                    needRemoveJewels.Add(colLastCountObjs[i].transform.Find("JewelPicture").gameObject);
                }
            }
            else if (rowLastCount >= 3)
            {
                needRemoveJewels.Add(lastObj.transform.Find("JewelPicture").gameObject);
                for (int i = 0; i < rowLastCountObjs.Count; i++)
                {
                    needRemoveJewels.Add(rowLastCountObjs[i].transform.Find("JewelPicture").gameObject);
                }
            }
            //播放爆炸的声音
            AudioManager.Instance.PlayAudio("Audio/boom");
            isMatch = true;
            //needRemoveJewels.Clear();
            //宝石下移
            //CallMoveDownJewels();
            //再次生成宝石
            //ReInitializedJewels();
        }
        if(isMatch == false)
        {
            JewelsManager.Instance.ResetOriginJewel();
            JewelsManager.Instance.ResetAllJewelSelected();
        }
    }
    //移除宝石
    private void RemoveJewel(List<GameObject> gameObject)
    {
        for (int i = 0; i < gameObject.Count; i++)
        {
            if (gameObject[i]!= null)
            {
                //reMatchObjects.Add(gameObject[i]);
                gameObject[i].GetComponent<JewelPicture>().Fade();
            }
        }
    }
    //重新开始游戏
    public void Restart()
    {
        for(int i=0;i<rowSize;i++)
        {
            for(int j =0; j<colSize;j++)
            {
                Destroy(jewelObjects[i, j]);
            }
        }

        InitializedGame();
    }
    //开始游戏
    public void GameStart()
    {
        //隐藏主界面
        mainMenu.SetActive(false);
        //AudioManager.Instance.PlayBGM();
        InitializedGame();
    }

    //得到宝石的位置
    public Vector3 GetJewelPosition(GameObject obj)
    {
        if(obj==null)
        {
            return Vector3.zero;
        }
        Vector3 jewelPosition = Vector3.zero;
        for(int i=0;i<rowSize;i++)
        {
            for(int j=0;j<colSize;j++)
            {
                 if(jewelObjects[i,j].Equals(obj))
                    {
                    jewelPosition.x = i;
                    jewelPosition.y = j;
                    jewelPosition.z = 0;
                    break;
                }
            }
        }
        return jewelPosition; ;
    }
    //把所有宝石的isSelected置为false
    public void SetAllJewelSelectedIsFalse()
    {
        for(int i=0;i<rowSize;i++)
        {
            for(int j =0;j<colSize;j++)
            {
                jewelObjects[i, j].transform.Find("Select").gameObject.SetActive(false);
                jewelObjects[i, j].GetComponent<Jewel>().isSelected = false;
            }
        }
    }
}
