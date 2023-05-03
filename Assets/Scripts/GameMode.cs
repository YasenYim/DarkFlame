using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMode : MonoBehaviour
{
    public static GameMode Instance { get; private set; }
    List<GameObject> allEnemies;
    PlayerCharacter player;
    public MenuUI menuUI;

    // 激活Boss
    Collider bossGate;
    BossCharacter boss;
    Slider bossHUD;

    static int deathCount = 0;

    private void Awake()
    {
        Instance = this;
        allEnemies = new List<GameObject>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>();
        // 保存所有敌人，并隐藏
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject go in enemies)
        {
            go.tag = "EnemyCopy";
            go.transform.position += new Vector3(10000, 0, 0);
            go.SetActive(false);
            allEnemies.Add(go);
        }
        CreateEnemies();

        // 激活boss用的字段·
        bossGate = GameObject.Find("BossGate").GetComponent<Collider>();
        boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossCharacter>();
        bossHUD = boss.transform.Find("Page_Boss/Sld_BossHealth").GetComponent<Slider>();

        // 如果死亡次数大于0，则显示“revived”
        if (deathCount > 0)
        {
            UIManager.Instance.ShowRevived();
            AudioManager.Instance.AudioPlay("Revive");
        }
    }

    void ClearEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject go in enemies)
        {
            Destroy(go);
        }
    }

    void CreateEnemies()
    {
        foreach (GameObject go in allEnemies)
        {
            GameObject enemy = Instantiate(go, go.transform.parent);
            enemy.transform.position -= new Vector3(10000, 0, 0);
            enemy.tag = "Enemy";
            enemy.SetActive(true);
        }
    }

    public void EnterBossArea()
    {
        bossGate.isTrigger = false;
        AudioManager.Instance.PlayBGM("BossBGM");
        bossHUD.gameObject.SetActive(true);
        boss.state = EnemyState.Move;
    }

    public void PlayerDie()
    {
        UIManager.Instance.SetPlayerDeathText("You Died");
        deathCount++;
        AudioManager.Instance.StopBGM();
        StartCoroutine(CoRestart());
    }

    public void BossDie()
    {
        bossGate.isTrigger = true;
        AudioManager.Instance.StopBGM();
        UIManager.Instance.SetPlayerDeathText("You Defeated");
    }

    IEnumerator CoRestart()
    {
        yield return new WaitForSeconds(6f);
        SceneManager.LoadScene("Game");
    }

    public void ResetByFire()
    {
        // 坐火，所有敌人重生
        player.health = player.maxHealth;
        player.energy = player.maxEnergy;
        player.potionCount = 5;

        ClearEnemies();
        CreateEnemies();
    }

    public void ShowMenu()
    {
        menuUI.gameObject.SetActive(true);
        player.GetComponent<PlayerController>().enabled = false;
    }

    public void ResumeGame()
    {
        menuUI.gameObject.SetActive(false);
        player.GetComponent<PlayerController>().enabled = true;

        player.Sit(null, false);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("StartPage");
    }
}
