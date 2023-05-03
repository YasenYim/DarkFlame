using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CommonUISelection : MonoBehaviour
{
    public string upKey;
    public string downKey;
    public string confirmKey;

    [HideInInspector]
    public UnityEvent<int> onSelect;
    [HideInInspector]
    public UnityEvent<int> onConfirm;

    [HideInInspector]
    public int curIndex = 0;
    [HideInInspector]
    public int numSelections = 1;

    List<Transform> listSelections;

    public void Init(List<Transform> selections, UnityAction<int> onSel, UnityAction<int> onConf)
    {
        onSelect.RemoveAllListeners();
        onConfirm.RemoveAllListeners();

        numSelections = selections.Count;
        onSelect.AddListener(onSel);
        onConfirm.AddListener(onConf);

        listSelections = selections;

        for (int i = 0; i < listSelections.Count; i++)
        {
            EventTrigger et = listSelections[i].GetComponent<EventTrigger>();
            if (!et)
            {
                et = listSelections[i].gameObject.AddComponent<EventTrigger>();
            }
            et.triggers = new List<EventTrigger.Entry>();
            var entry1 = new EventTrigger.Entry();
            entry1.eventID = EventTriggerType.PointerEnter;
            entry1.callback.AddListener(OnPointerEnter);
            var entry2 = new EventTrigger.Entry();
            entry2.eventID = EventTriggerType.PointerClick;
            entry2.callback.AddListener(OnPointerClick);
            et.triggers.Add(entry1);
            et.triggers.Add(entry2);
        }
    }

    public void OnPointerEnter(BaseEventData _evt)
    {
        PointerEventData evt = (PointerEventData)_evt;
        Debug.Log("Pointer Enter " + evt.pointerEnter.gameObject.name);
        int index = listSelections.IndexOf(evt.pointerEnter.transform);
        onSelect.Invoke(index);
    }

    public void OnPointerClick(BaseEventData _evt)
    {
        PointerEventData evt = (PointerEventData)_evt;
        Debug.Log("Pointer Click " + evt.pointerClick.gameObject.name);
        int index = listSelections.IndexOf(evt.pointerClick.transform);
        onConfirm.Invoke(index);
    }

    void Update()
    {
        int lastIndex = curIndex;

        if (Input.GetButtonDown(upKey))
        {
            curIndex++;
        }
        else if (Input.GetButtonDown(downKey))
        {
            curIndex--;
        }
        else if (Input.GetButtonDown(confirmKey))
        {
            onConfirm.Invoke(curIndex);
            return;
        }
        else
        {
            return;
        }

        // loop index
        if (curIndex < 0)
        {
            curIndex += numSelections;
        }
        else if (curIndex >= numSelections)
        {
            curIndex = 0;
        }

        if (curIndex != lastIndex)
        {
            onSelect.Invoke(curIndex);
        }
    }
}
