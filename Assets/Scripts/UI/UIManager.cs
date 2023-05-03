using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    PlayerCharacter player;
    TipActive tips;

    public Slider healthSlider;
    public Slider powerSlider;

    public Text potionAmount;
    public Text tipFireText;
    public Text soulsText;
    public Text tipText;
    public Text deathText;
    public Text reviveText;
    public Image tipContent;

    public bool isTipActive = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>();
        healthSlider = transform.Find("PlayerInfo/Health").GetComponent<Slider>();
        healthSlider.maxValue = player.maxHealth;
        powerSlider = transform.Find("PlayerInfo/Power").GetComponent<Slider>();
        powerSlider.maxValue = player.maxEnergy;

        potionAmount = transform.Find("Bg_BloodPotion/Img_BloodPotion/Txt_Amount").GetComponent<Text>();

        tipFireText = transform.Find("Tip_Fire").GetComponent<Text>();
        tipFireText.gameObject.SetActive(false);

        tipText = transform.Find("Txt_Tip").GetComponent<Text>();
        tipText.gameObject.SetActive(false);

        deathText = transform.Find("Txt_Death").GetComponent<Text>();
        deathText.gameObject.SetActive(false);

        reviveText = transform.Find("Txt_Revive").GetComponent<Text>();
        reviveText.gameObject.SetActive(false);

        tipContent = transform.Find("Img_Tip").GetComponent<Image>();
        tipContent.gameObject.SetActive(false);

        soulsText = transform.Find("Souls/Text").GetComponent<Text>();
    }

    public void SetPlayerDeathText(string text)
    {
        deathText.text = text;
        deathText.gameObject.SetActive(true);
    }

    public void ShowRevived()
    {
        reviveText.gameObject.SetActive(true);
        Color c = reviveText.color;
        reviveText.color = new Color(c.r, c.g, c.b, 0);

        Sequence seq = DOTween.Sequence();
        seq.Append(reviveText.DOFade(1, 1.0f));
        seq.AppendInterval(0.8f);
        seq.Append(reviveText.DOFade(0, 1.5f));
        seq.AppendCallback(() => {
            reviveText.gameObject.SetActive(false); 
        });
    }
	
	void Update()
    {
        healthSlider.value = player.health;
        powerSlider.value = player.energy;

        potionAmount.text = player.potionCount.ToString();
        soulsText.text = player.souls.ToString();
    }
}
