using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JewelPicture : MonoBehaviour {

    public int identityCode;
    private Animator anim;
    // Use this for initialization
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Fade()
    {
        //Debug.Log(GameManager.Instance.GetJewelPosition(gameObject) + " " + transform.Find("JewelPicture").GetComponent<Image>().sprite.name + " " + transform.Find("JewelPicture").GetComponent<JewelPicture>().identityCode);
        anim.SetTrigger("JewelCrash");
        StartCoroutine(FadeJewel());
    }
    IEnumerator FadeJewel()
    {
        yield return new WaitForSeconds(0.7f);
        transform.SetParent(null);
        Destroy(gameObject);
    }
}
