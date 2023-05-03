using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveBoss : MonoBehaviour
{
    [SerializeField]
    BossCharacter boss;
    [SerializeField]
    Slider bossHUD;

    AudioManager audioManager;
    PlayerCharacter player;

	void Start ()
    {
        boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossCharacter>();
        bossHUD = boss.transform.Find("Page_Boss/Sld_BossHealth").GetComponent<Slider>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && boss.state != EnemyState.Die)
        {
            bossHUD.gameObject.SetActive(true);
            boss.state = EnemyState.Move;
        }
    }
}
