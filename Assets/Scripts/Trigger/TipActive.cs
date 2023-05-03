using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipActive : MonoBehaviour
{
    PlayerCharacter player;

    public string tip;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>();
    }

    private void Update()
    {
        UIManager ui = UIManager.Instance;
        if (ui.isTipActive)
        {
            ui.tipText.gameObject.SetActive(true);
            if (Input.GetButtonDown("React") && player.state == PlayerState.Move) 
            {
                ui.tipContent.gameObject.SetActive(true);
            }
        }
        else
        {
            ui.tipText.gameObject.SetActive(false);
            ui.tipContent.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        UIManager ui = UIManager.Instance;
        if(other.gameObject.tag == "Player")
        {
            ui.isTipActive = true;
            tip = tip.Replace("\\n", "\n");
            ui.tipContent.GetComponentInChildren<Text>().text = tip;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        UIManager ui = UIManager.Instance;
        if (other.gameObject.tag == "Player")
        {
            ui.isTipActive = false;
        }
    }
}
