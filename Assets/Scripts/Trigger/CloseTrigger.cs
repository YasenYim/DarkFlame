using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseTrigger : MonoBehaviour
{
    GameObject enterTrigger;

	void Start ()
    {
        enterTrigger = GameObject.Find("Trigger03");
    }
	
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameMode.Instance.EnterBossArea();
        }
        // 隐藏自身，保证只关闭一次boss的大门
        gameObject.SetActive(false);
    }
}
