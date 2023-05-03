using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireActive : MonoBehaviour
{
    PlayerCharacter player;

    bool near = false;

	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>();
    }

    private void Update()
    {
        if (player.GetComponent<PlayerController>().enabled && Input.GetButtonDown("React"))
        {
            if (near && player.state == PlayerState.Move)
            {
                AudioManager.Instance.AudioPlayOnce("Fire" ,transform.position);
                player.Sit(transform, true);
                UIManager.Instance.tipFireText.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            near = true;
            if (player.state != PlayerState.Sit)
            {
                UIManager.Instance.tipFireText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            near = false;
            UIManager.Instance.tipFireText.gameObject.SetActive(false);
        }
    }

}
