using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;

public class MainPage : MonoBehaviour {

    private LuaState lua;
    private LuaFunction func = null;
    public GameObject MainPagePrefab;

    void Awake()
    {
        lua = new LuaState();    //lua解析器
        lua.Start();     //启动lua
        string luaPath = Application.dataPath + "\\Scripts/";
        lua.AddSearchPath(luaPath);
        lua.Require("MainPage");   //Require读取lua文件只执行一次
        func = lua.GetFunction("MainPage.Awake");
        if (func != null)
        {
            CallLuaFunction();
        }

        lua.CheckTop();
        lua.Dispose();
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void CallLuaFunction()
    {
        func.BeginPCall();
        func.Push(MainPagePrefab.transform);
        func.PCall();
        func.EndPCall();
    }
}
