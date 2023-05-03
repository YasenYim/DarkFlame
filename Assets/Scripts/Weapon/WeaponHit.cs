using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHit : MonoBehaviour
{
    PlayerCharacter player;

    public Transform damagePoint;       // 产生伤害判定的原点
    public float normalDamageRadious; //普通攻击伤害半径
    public float specialDamageRadious; //特殊攻击伤害半径
    public float normalDamageAngle; //普通攻击角度范围
    public float specialDamageAngle; //特殊攻击角度范围
    public int normalAttackDamage; //普通攻击伤害
    public int specialAttackDamage; //特殊攻击伤害
    public int powerCost; //攻击时消耗的体力值

    ParticleSystem specialParticle;

    private void Awake()
    {
        player = GetComponentInParent<PlayerCharacter>();

        var transParticle = transform.Find("SpecialParticle");
        if (transParticle)
        {
            specialParticle = transParticle.GetComponent<ParticleSystem>();
            specialParticle.gameObject.SetActive(false);
        }
    }

    public void SpecialParticle(bool play)
    {
        if (!specialParticle)
        {
            return;
        }
        if (play)
        {
            specialParticle.gameObject.SetActive(true);
            specialParticle.Play();
        }
        else
        {
            specialParticle.gameObject.SetActive(false);
        }
    }

    // 检测是否攻击到了敌人（帧事件）
    public void WeaponAttack(bool special)
    {
        var colliders = Physics.OverlapSphere(player.transform.position, specialDamageRadious, LayerMask.GetMask("Enemy")); //探测敌人的相交球

        foreach (var col in colliders)
        {
            var target = col.gameObject.transform.position - player.transform.position; //角色位置到敌人位置的目标向量
            var angle = Vector3.Angle(player.transform.forward, target); //角色forward向量与目标向量的夹角

            if (!special) //普通攻击
            {
                if (angle <= normalDamageAngle && target.magnitude <= normalDamageRadious)
                {
                    // Debug.Log(col.gameObject.name);
                    if (col.CompareTag("Boss"))
                    {
                        BossCharacter boss = col.GetComponent<BossCharacter>();
                        boss.GetHit(normalAttackDamage);
                    }
                    else
                    {
                        EnemyCharacter enemy = col.GetComponent<EnemyCharacter>();
                        enemy.GetHit(normalAttackDamage);
                    }
                }
            }
            else 
            {
                if (angle <= specialDamageAngle && target.magnitude <= specialDamageRadious)
                {
                    if (col.gameObject.tag == "Boss")
                    {
                        BossCharacter boss = col.GetComponent<BossCharacter>();
                        boss.GetHit(specialAttackDamage);
                    }
                    else
                    {
                        EnemyCharacter enemy = col.GetComponent<EnemyCharacter>();
                        enemy.GetHit(specialAttackDamage);
                    }
                }
            }
        }
    }
}
