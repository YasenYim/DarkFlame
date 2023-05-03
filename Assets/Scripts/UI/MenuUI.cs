using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    Image imageSelect;
    CommonUISelection selection;

    public List<Transform> listSelections;

    private void Start()
    {
        imageSelect = transform.Find("ImageSelect").GetComponent<Image>();

        selection = GetComponent<CommonUISelection>();
        selection.Init(listSelections, OnSelect, OnConfirm);
    }

    private void OnSelect(int index)
    {
        imageSelect.gameObject.SetActive(true);
        imageSelect.transform.position = listSelections[index].position;
    }

    private void OnConfirm(int index)
    {
        if (index == 0)
        {
            GameMode.Instance.ResumeGame();
        }
        else if (index == 1)
        {
            GameMode.Instance.ReturnToMainMenu();
        }
    }
}
