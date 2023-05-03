using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public Slider loadingSlider;
    private AsyncOperation async; //异步读取
    private int processNow; //现阶段读取进度

	void Start ()
    {
        loadingSlider = GameObject.Find("Sld_Loading").GetComponent<Slider>();
        processNow = 0;
        StartCoroutine(LoadScene());

    }
	
	void Update ()
    {
		if(async == null)
        {
            return;
        }

        int process;

        if(async.progress < 0.9f)
        {
            process = (int)(async.progress * 100);
        }
        else
        {
            process = 100;
        }

        if(processNow < process)
        {
            processNow++;
        }

        loadingSlider.value = processNow;

        if(processNow == 100)
        {
            async.allowSceneActivation = true;
        }
    }

    IEnumerator LoadScene()
    {
        async = SceneManager.LoadSceneAsync("Game");
        async.allowSceneActivation = false;
        yield return async;
    }
}
