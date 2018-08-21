using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationManager : MonoBehaviour {

    public AnimationClip clearAnimation;
    private static AnimationManager _instance;
    public static AnimationManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.Find("AnimationManager").GetComponent<AnimationManager>();
            return _instance;
        }
    }
    private Animator anim;
    private bool isClearing;
    public bool IsClearing
    {
        get
        {
            return isClearing;
        }
    }
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {

	}
    public void CallPlayCrashAnim(GameObject gameObject)
    {
        isClearing = true;
        StartCoroutine(PlayCrashAnim(gameObject));
        
    }

    IEnumerator PlayCrashAnim(GameObject gameObject)
    {
        anim = gameObject.GetComponent<Animator>();
        if(anim!=null)
        {
            //Debug.Log("播放前:" + GameManager.Instance.GetJewelPosition(gameObject) + " " + gameObject.transform.Find("JewelPicture").GetComponent<Image>().sprite);
            anim.Play(clearAnimation.name);
            //Debug.Log("播放后:"+GameManager.Instance.GetJewelPosition(gameObject) + " " + gameObject.transform.Find("JewelPicture").GetComponent<Image>().sprite);
            yield return new WaitForSeconds(clearAnimation.length);

            if (gameObject.transform.Find("JewelPicture"))
            {
                GameObject childObj = gameObject.transform.Find("JewelPicture").gameObject;
                childObj.transform.SetParent(null);
                Destroy(childObj);
            }
        }
        anim = null;
    }
}
