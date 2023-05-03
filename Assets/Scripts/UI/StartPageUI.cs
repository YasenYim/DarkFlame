using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class StartPageUI : MonoBehaviour
{
    Text title; //游戏标题

    CommonUISelection selection;

    Image bonfire;
    Outline titleOutline;

    public List<Transform> selectionList;

    void Start()
    {
        selection = GetComponent<CommonUISelection>();
        selection.Init(selectionList, OnSelectChange, OnConfirm);

        title = gameObject.transform.Find("Txt_Title").GetComponent<Text>();
        bonfire = gameObject.transform.Find("Img_BonFire").GetComponent<Image>();

        Init();

        AudioManager.Instance.PlayBGM("StartBGM");
    }
	
    // 初始化
    void Init()
    {
        for(int i = 0; i < selectionList.Count; i++)
        {
            selectionList[i].gameObject.SetActive(false);
        }

        titleOutline = title.gameObject.GetComponent<Outline>();
        titleOutline.enabled = false;

        title.color *= new Color(1,1,1,0);
        bonfire.color *= new Color(1,1,1,0);

        Sequence seq = DOTween.Sequence();
        seq.Append(title.DOFade(1, 2));
        seq.Append(bonfire.DOFade(1, 2));
        seq.AppendCallback(
            () => {
                titleOutline.enabled = true;
            for(int i = 0; i < selectionList.Count; i++)
            {
                selectionList[i].gameObject.SetActive(true);
            }
            });
    }

    // 改变选项
    void OnSelectChange(int index)
    {
        Debug.Log("OnSelectChange " + index);
        AudioManager.Instance.AudioPlay("ChangeSelection");
        for (int i=0; i<selectionList.Count; i++)
        {
            selectionList[i].transform.GetChild(0).gameObject.SetActive(i == index);
        }
    }

    void OnConfirm(int index)
    {
        Debug.Log("OnConfirm " + index);
        if (index == 0)
        {
            SceneManager.LoadScene("Loading");
            AudioManager.Instance.StopBGM();
        }
        else if (index == 1)
        {
            Application.Quit();
        }
    }
}
