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
    private float animationPlayTime;
    public GameObject jewelPrefab;
    private GameObject gamePlayPanel;
    private GameObject[,] jewelObjects;
    private int rowSize, colSize;
    public float moveDownTime;
    public GameObject mainMenu;
    private bool gameStart;
    private bool isMatch;
    private bool isRemove;
    private bool isMoveDown;
    private bool moveDownSuccess; 
    List<GameObject> needRemoveJewels;

    [SerializeField]
    private Sprite[] jewelSprites;
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

    public bool isExchangeDone;

    // Use this for initialization
    void Start() {
        rowSize = 6;
        colSize = 6;
        moveDownTime = 0.2f;
        animationPlayTime = 0.71f;
        canSelect = true;
        gameStart = false;
        isMatch = false;
        isRemove = false;
        isMoveDown = false;
        isExchangeDone = false;
        moveDownSuccess = false;
        needRemoveJewels = new List<GameObject>();
        jewelObjects = new GameObject[rowSize, colSize];
        jewelSprites = Resources.LoadAll<Sprite>("Graphics/Jewels");
        gamePlayPanel = GameObject.Find("/UICanvas/GamePlay/GamePlayPanel");
    }

    // Update is called once per frame
    void Update() {
   
    }
    //判断匹配的规则
    public void MatchRule()
    {
        GameObject lastSelected = JewelsManager.Instance.lastSelected;
        GameObject currentSeleted = JewelsManager.Instance.currentSeleted;

        bool lastMatch = MatchJewel(lastSelected);
        bool currentMatch = MatchJewel(currentSeleted);
        if (!lastMatch&&!currentMatch)
        {
            JewelsManager.Instance.ResetOriginJewel();
        }else if( lastMatch || currentMatch)
        {
            RemoveJewel();
            StartCoroutine(FillJewels());
        }
    }
    //初始化游戏
    void InitializedGame() {
        for (int i = 0; i <rowSize; i++)
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
                if (i >=2 && string.Equals(jewelObjects[i - 2, j].gameObject.transform.Find("JewelPicture").GetComponent<Image>().sprite.name, jewelObjects[i - 1, j].gameObject.transform.Find("JewelPicture").GetComponent<Image>().sprite.name))
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
                GameObject itemObject = Instantiate<GameObject>(jewelPrefab, gamePlayPanel.transform, false);
                itemObject.GetComponent<RectTransform>().localPosition = new Vector2(-250f + j * xOffset, 250f + (i-6) * yOffset);
                itemObject.GetComponent<RectTransform>().DOLocalMove(new Vector2(-250f + j * xOffset, 250f + i * yOffset),1.0f);
                jewelObjects[i, j] = itemObject;
            }
        }
        gameStart = true;
    }

    //重新匹配游戏宝石
    private void ReMatchGameBorad()
    {
        for (int i = 0; i < rowSize; i++)
        {
            for (int j = 0; j < colSize; j++)
            {
                if (jewelObjects[i, j].transform.Find("JewelPicture").gameObject)
                {
                    MatchJewel(jewelObjects[i, j]);
                }
            }
        }
        if(needRemoveJewels.Count>0)
        {
            RemoveJewel();
            StartCoroutine(FillJewels());
        }
    }

    //填充宝石的协程
    private IEnumerator FillJewels()
    {
        yield return new WaitForSeconds(animationPlayTime);
        //自下而上遍历整个游戏棋盘，遇到空的向上遍历把上面的sprite拿给自己
        for (int i = rowSize - 1; i > 0; i--)
        {
            for (int j = 0; j < colSize; j++)
            {
                //Debug.Log(GetJewelPosition(jewelObjects[i, j])+" "+jewelObjects[i, j].transform.Find("JewelPicture").GetComponent<Image>().sprite);
                //当前格子为空，则不断往上找不为空的格子并把它的sprite赋值给当前格子，则找到的格子赋值为空
                if (jewelObjects[i, j].transform.Find("JewelPicture")== null)
                {
                    for (int k =i; k >= 0; k--)
                    {
                        if (jewelObjects[k, j].transform.Find("JewelPicture") != null)
                        {
                            //当前对象
                            GameObject thisObj = jewelObjects[i, j];
                            //当前对象上面其中一个对象
                            GameObject topObj = jewelObjects[k, j];
                            GameObject topObjChild = topObj.transform.Find("JewelPicture").gameObject;
                            topObjChild.transform.SetParent(thisObj.transform);
                            topObjChild.transform.DOLocalMove(Vector3.zero, moveDownTime);
                            break;
                        }
                    }
                }
            }
        }
        
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
                    initializedChild.DOLocalMove(Vector3.zero, moveDownTime);
                }
            }
            //4.跳到下一列
            reNeedCount = 0;
        }
        yield return new WaitForSeconds(moveDownTime + 0.1f);
        ReMatchGameBorad();
    }

    //新的匹配方式
    private bool MatchJewel(GameObject obj)
    {
        Vector3 jewelPosition = GetJewelPosition(obj);
        int x = (int)jewelPosition.x;
        int y = (int)jewelPosition.y;

        List<GameObject> matchRowList = new List<GameObject>();
        List<GameObject> matchColList = new List<GameObject>();
        matchRowList.Add(obj.transform.Find("JewelPicture").gameObject);
        #region 纵向匹配
        for (int i = x + 1; i <rowSize; i++)
        {
            if (i < rowSize && Equals(obj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[i, y].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
            {
                matchRowList.Add(jewelObjects[i, y].transform.Find("JewelPicture").gameObject);
            }
            else
                break;
        }
        for (int i = x - 1; i >= 0; i--)
        {
            if (i >= 0 && Equals(obj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[i, y].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
            {
                matchRowList.Add(jewelObjects[i, y].transform.Find("JewelPicture").gameObject);
            }
            else
                break;
        }
        #endregion
        matchColList.Add(obj.transform.Find("JewelPicture").gameObject);
        #region 横向匹配
        for (int i = y - 1; i >=0; i--)
        {
            if (i >= 0 && Equals(obj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[x, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
            {
                matchColList.Add(jewelObjects[x, i].transform.Find("JewelPicture").gameObject);
            }
            else
                break;
        }
        for (int i = y + 1; i <colSize; i++)
        {
            if (i < colSize && Equals(obj.transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name, jewelObjects[x, i].transform.Find("JewelPicture").gameObject.GetComponent<Image>().sprite.name))
            {
                matchColList.Add(jewelObjects[x, i].transform.Find("JewelPicture").gameObject);
            }
            else
                break;
        }
        #endregion

        //1.横向连消
        if (matchColList.Count >= 3 && matchRowList.Count < 3)
        {
            for (int i = 0; i < matchColList.Count; i++)
            {
                needRemoveJewels.Add(matchColList[i]);
            }
            //播放爆炸的声音
            AudioManager.Instance.PlayAudio("Audio/boom");
            //state = GameState.Match;
            return true;
        }
        //2.纵向连消
        else if (matchRowList.Count >= 3 && matchColList.Count < 3)
        {
            for (int i = 0; i < matchRowList.Count; i++)
            {
                needRemoveJewels.Add(matchRowList[i]);
            }
            //播放爆炸的声音
            AudioManager.Instance.PlayAudio("Audio/boom");
            //state = GameState.Match;
            return true;
        }
        //3.特殊情况
        else if (matchColList.Count >= 3 && matchRowList.Count >= 3)
        {
            matchRowList.RemoveAt(0);
            for (int i = 0; i < matchRowList.Count; i++)
            {
                needRemoveJewels.Add(matchRowList[i]);
            }
            for (int i = 0; i < matchColList.Count; i++)
            {
                needRemoveJewels.Add(matchColList[i]);
            }
            //播放爆炸的声音
            AudioManager.Instance.PlayAudio("Audio/boom");
            //state = GameState.Match;
            return true;
        }
        //state = GameState.Normal;
        return false;
    }

    //移除宝石
    private bool RemoveJewel()
    {
        bool needRefill = false;
        for (int i = 0; i < needRemoveJewels.Count; i++)
        {
            if (needRemoveJewels[i]!= null)
            {
                needRemoveJewels[i].GetComponent<JewelPicture>().Fade();
                needRefill = true;
            }
        }        
        JewelsManager.Instance.ResetAllJewelSelected();
        needRemoveJewels.Clear();
        isRemove = true;
        return needRefill;
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
        gameStart = true;
        mainMenu.SetActive(false);
        AudioManager.Instance.PlayBGM();
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
