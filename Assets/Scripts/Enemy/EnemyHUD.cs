using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHUD : MonoBehaviour
{
    public EnemyCharacter enemy;
    public Image fillImage;
    Color foreColor;
    public Slider healthSlider;
    Transform head;
    Camera mainCamera;
    RectTransform rectTran;
    public float height = 2.5f;
    float distance; //

    void Start()
    {
        enemy = GetComponentInParent<EnemyCharacter>();
        healthSlider = GetComponentInChildren<Slider>();
        rectTran = healthSlider.GetComponent<RectTransform>();

        var headPos = new Vector3(enemy.gameObject.transform.position.x, enemy.gameObject.transform.position.y + height, enemy.gameObject.transform.position.z);
        distance = Vector3.Distance(headPos, Camera.main.transform.position);

        fillImage = gameObject.transform.Find("Sld_Health/Fill Area/Fill").GetComponent<Image>();
        fillImage.color = Color.red;
    }

    void Update()
    {
        var headPos = new Vector3(enemy.gameObject.transform.position.x, enemy.gameObject.transform.position.y + height, enemy.gameObject.transform.position.z);
        var newDistance = distance / Vector3.Distance(headPos, Camera.main.transform.position);
        //Debug.Log("headPos:" + headPos);
        Vector2 pos = Camera.main.WorldToScreenPoint(headPos);
        //Debug.Log("2d pos:" + pos);
        
        rectTran.position = pos;
        //rectTran.localScale = Vector3.one * newDistance;

        if (enemy.state != EnemyState.Die)
        {
            healthSlider.gameObject.SetActive(true);
        }
        else
        {
            healthSlider.gameObject.SetActive(false);
        }

        healthSlider.value = enemy.health;
    }
}
