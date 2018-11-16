using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Toload : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //加载到加载界面
    public void ToLoadingScene()
    {
        SceneManager.LoadScene("LoadScene");
    }
}
