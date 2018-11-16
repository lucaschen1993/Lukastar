using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncLoad : MonoBehaviour
{

    public Slider LoadingSlider;
    public Text LoadingText;

    private float _loadingSpeed = 1;
    private float _targetValue;
    private AsyncOperation operation;

	// Use this for initialization
	void Start ()
	{
	    LoadingSlider.value = 0.0f;
	    if (SceneManager.GetActiveScene().name == "LoadScene")
	    {
	        StartCoroutine(AsyncLoading());
	    }

	}

    private IEnumerator AsyncLoading()
    {
        operation = SceneManager.LoadSceneAsync("GameScene");
        operation.allowSceneActivation = false;

        yield return operation;
    }

    // Update is called once per frame
    void Update ()
    {
        _targetValue = operation.progress;
        if (_targetValue >= 0.9f)
        {
            _targetValue = 1.0f;
        }

        if (_targetValue != LoadingSlider.value)
        {
            LoadingSlider.value = Mathf.Lerp(LoadingSlider.value, _targetValue, Time.deltaTime * _loadingSpeed);
            if(Mathf.Abs(LoadingSlider.value-_targetValue)<0.01f)
            {
                LoadingSlider.value = _targetValue;
            }
        }

        LoadingText.text = ((int) (LoadingSlider.value * 100)).ToString() + "%";

        if ((int) (LoadingSlider.value * 100) == 100)
        {
            operation.allowSceneActivation = true;
        }

    }
}
