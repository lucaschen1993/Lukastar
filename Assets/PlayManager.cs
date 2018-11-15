using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour {

    [SerializeField]
    public Text ScoreText;
    [SerializeField]
    public  Text _stepsText;
    private int ScoreCount;
    public int StepsCount;

    // Use this for initialization
    void Start () {
        ScoreText.text = "0";
        _stepsText.text = "20";
        ScoreCount = 0;
        StepsCount = 20;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StepAndScore(int param)
    {


        ScoreCount += param;
        ScoreText.text = ScoreCount.ToString();
        StepsCount--;
        _stepsText.text = StepsCount.ToString();
    }
}
