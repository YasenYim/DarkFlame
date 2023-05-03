using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum PlayerState
{
    Move,
    Roll,
    Attack,
    SpecialAttack,
    SwitchWeapon,
    GetHit,
    Drink,
    Sit,
    Die
}

public class PlayerCharacter : MonoBehaviour
{
    // Components
    PlayerController controller;//角色控制器
    Animator animator;//动画控制器
    Rigidbody rigid;
    
    // Resources
    ParticleSystem lostSoulParticle;//死亡后魂掉落位置的特效

    // Editable params
    public Transform potion;//放血瓶模型的地方
    public int potionCount = 5;//血瓶数

    public float runSpeed; //移动速度
    public float rollSpeed; //翻滚速度

    public ParticleSystem specialAttackParticle;//特殊攻击的特效
    public ParticleSystem bloodParticle;//血液飞溅的特效

    public int maxHealth = 100;
    public int maxEnergy = 100;
    public float recoverSpeed = 30; //体力恢复速度
    public float recoverCD = 1f; //体力槽空后经过这些时间后才开始恢复体力值

    [HideInInspector]
    public int health; //血量
    [HideInInspector]
    public float energy; //体力值
    public int souls; //玩家身上的魂量

    float recoverTime = 0; //体力槽空后开始计时

    public PlayerState state;//玩家状态

    Vector3 move;
    
    void Awake()
    {

    }
    
    void Start()
    {
        rigid = GetComponent<Rigidbody>();

        lostSoulParticle = Resources.Load<ParticleSystem>("Prefabs/LostSoul");

        foreach (var weapon in WeaponManager.Instance.weaponList)
        {
            weapon.AddComponent<WeaponHit>();
            weapon.SetActive(false);
        }

        WeaponManager.Instance.weaponList[0].SetActive(true);

        // --------------
        health = maxHealth;
        energy = maxEnergy;

        controller = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();

        if(PlayerPrefs.HasKey("Death"))
        {
            //deathPos = new Vector3(PlayerPrefs.GetFloat("DeathPosX"), PlayerPrefs.GetFloat("DeathPosY"), PlayerPrefs.GetFloat("DeathPosZ"));
            //Instantiate(lostSoulParticle.gameObject, deathPos, lostSoulParticle.gameObject.transform.rotation);
        }
    }

    // Update里的代码全都被分散到各个动画的Update中了
	void Update()
    {
    }

    void StateMoveUpdate()
    {
        move = new Vector3(controller.h, 0, controller.v);
        if (energy > 0)
        {
            if (controller.roll)
            {
                CostEnergy(20);
                animator.ResetTrigger("GetHit");
                animator.SetTrigger("Roll");
            }
            else if (controller.attack)
            {
                if (energy > 0)
                {
                    animator.SetTrigger("Attack");
                }
            }
            else if (controller.specialAttack)
            {
                animator.SetTrigger("SpecialAttack");
                var hit = GetComponentInChildren<WeaponHit>();
                hit.SpecialParticle(true);
            }
            else if (controller.use)
            {
                if (potionCount > 0)
                {
                    AudioManager.Instance.AudioPlay("Drink");
                    animator.SetTrigger("Drink");
                }
                else
                {
                    animator.SetTrigger("EmptyDrink");
                }
            }
            else if (controller.switchWeapon)
            {
                animator.SetTrigger("SwitchWeapon");
            }
        }
    }


    // 喝血瓶动画帧事件
    public void DrinkingPotion()
    {
        //Debug.Log("DrinkingPotion");
        WeaponManager.Instance.Current.SetActive(false);
        potion.gameObject.SetActive(true);

        if (potionCount > 0)
        {
            potionCount--;
            potion.Find("Liquid").gameObject.SetActive(true);
            health += 30;
            if (health >= maxHealth)
            {
                health = maxHealth;
            }
        }
        else
        {
            potion.Find("Liquid").gameObject.SetActive(false);
        }
    }

    // 玩家坐篝火或离开篝火
    public void Sit(Transform firePlace, bool isSit)
    {
        animator.SetBool("Sit", isSit);
        if (isSit)
        {
            Vector3 p = firePlace.position;
            p.y = transform.position.y;
            transform.LookAt(p);

            GameMode.Instance.ResetByFire();
            GameMode.Instance.ShowMenu();
        }
    }

    // 检测是否攻击到了敌人（帧事件）
    public void HitTest()
    {
        var hit = GetComponentInChildren<WeaponHit>();
        CostEnergy(hit.powerCost);
        hit.WeaponAttack(false);
    }

    public void HitTestSpecial()
    {
        var hit = GetComponentInChildren<WeaponHit>();
        CostEnergy(40);
        hit.WeaponAttack(true);
    }

    // 体力自动回复
    public void RecoverEnergy() 
    {
        if (recoverTime <= Time.time)
        {
            energy += recoverSpeed * Time.deltaTime;
        }
        if (energy > maxEnergy)
        {
            energy = maxEnergy;
        }
    }

    public void CostEnergy(int e)
    {
        energy -= e;
        if (energy <= 0)
        {
            energy = 0;
            recoverTime = Time.time + recoverCD;
        }
    }
    
    // 被敌人攻击
    public void GetHit(int damage)
    {
        if (state == PlayerState.Roll || state == PlayerState.Die)
        {
            return;
        }

        health -= damage;
        //Debug.Log($"Damage:{damage} Health:{health}");
        if (health <= 0)
        {
            health = 0;
        }

        bloodParticle.Play();
        AudioManager.Instance.AudioPlay("PlayerHitScream");
        if (state != PlayerState.GetHit)
        {
            animator.SetTrigger("GetHit");
        }

        if (state == PlayerState.Drink)
        {
            WeaponManager.Instance.Current.SetActive(true);
            potion.gameObject.SetActive(false);
            AudioManager.Instance.StopSound("Drink");
        }
    }

    // 玩家死亡
    void Die()
    {
        animator.SetTrigger("Die");

        AudioManager.Instance.AudioPlay("PlayerDeathAudio");
        GameMode.Instance.PlayerDie();
    }

    // 更换武器
    public void SwitchWeapon()
    {
        animator.SetTrigger("SwitchWeapon");
    }

    // 更换武器时显示或隐藏武器（帧事件）
    public void HideWeapon()
    {
        WeaponManager.Instance.Current.SetActive(false);
        WeaponManager.Instance.ChangeNext().SetActive(true);
    }

    public void SpecialAttackParticle()
    {
        specialAttackParticle.Play();
    }

    public void Rolling()
    {
        CostEnergy(20);
        animator.SetTrigger("Roll");
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case PlayerState.Move:
                {
                    if (move.magnitude > 0.1f)
                    {
                        Vector3 dir = move.normalized; //角色面朝目标的向量
                        Quaternion faceToQuat = Quaternion.LookRotation(dir); //角色面朝目标方向的四元数

                        Quaternion q = Quaternion.Slerp(transform.rotation, faceToQuat, 0.5f);

                        rigid.MoveRotation(q);

                        // 朝自身前方移动
                        rigid.velocity = transform.forward * runSpeed;

                        animator.SetBool("Run", true);
                    }
                    else
                    {
                        rigid.velocity = Vector3.zero;
                        animator.SetBool("Run", false);
                    }
                }
                break;
            case PlayerState.Roll:
                {
                    rigid.velocity = transform.forward * rollSpeed;
                }
                break;
        }

    }

    // -- 由动画状态机驱动核心状态机 -- 
    #region 核心状态机

    delegate void FuncStateEnter();
    delegate void FuncStateUpdate(int n);
    delegate void FuncStateExit(int n);

    Dictionary<PlayerState, FuncStateEnter> dictStateEnter;
    Dictionary<PlayerState, FuncStateUpdate> dictStateUpdate;
    Dictionary<PlayerState, FuncStateExit> dictStateExit;

    public void OnAnimStateEnter(PlayerState s)
    {
        //Debug.Log("进入状态" + s);
        if (dictStateEnter == null)
        {
            dictStateEnter = new Dictionary<PlayerState, FuncStateEnter>
            {
                {PlayerState.Move, MoveEnter},
                {PlayerState.Roll, RollEnter},
                {PlayerState.Attack, AttackEnter},
                {PlayerState.SpecialAttack, SpecialAttackEnter},
                {PlayerState.SwitchWeapon, SwitchWeaponEnter},
                {PlayerState.GetHit, GetHitEnter},
                {PlayerState.Drink, DrinkEnter},
                {PlayerState.Sit, SitEnter},
                {PlayerState.Die, DieEnter},
            };
        }

        if (dictStateEnter.ContainsKey(s) && dictStateEnter[s] != null)
        {
            dictStateEnter[s]();
        }
    }

    public void OnAnimStateUpdate(PlayerState s, int n)
    {
        if (dictStateUpdate == null)
        {
            dictStateUpdate = new Dictionary<PlayerState, FuncStateUpdate>
            {
                {PlayerState.Move, MoveUpdate},
                {PlayerState.Attack, AttackUpdate},
                {PlayerState.SpecialAttack, SpecialAttackUpdate},
                {PlayerState.Roll, RollUpdate},
                {PlayerState.SwitchWeapon, SwitchWeaponUpdate},
                {PlayerState.GetHit, GetHitUpdate},
                {PlayerState.Drink, DrinkUpdate},
            };
        }


        if (dictStateUpdate.ContainsKey(s) && dictStateUpdate[s] != null)
        {
            dictStateUpdate[s](n);
        }
    }

    public void OnAnimStateExit(PlayerState s, int n)
    {
        if (dictStateExit == null)
        {
            dictStateExit = new Dictionary<PlayerState, FuncStateExit>
            {
                {PlayerState.Move, MoveExit},
                {PlayerState.Attack, AttackExit},
                {PlayerState.SpecialAttack, SpecialAttackExit},
                {PlayerState.Roll, RollExit},
                {PlayerState.SwitchWeapon, SwitchWeaponExit},
                {PlayerState.GetHit, GetHitExit},
                {PlayerState.Drink, DrinkExit},
            };
        }

        if (dictStateExit.ContainsKey(s) && dictStateExit[s] != null)
        {
            dictStateExit[s](n);
        }
    }

    // ----------------Enter-----------------------------------

    public void MoveEnter()
    {
        state = PlayerState.Move;
    }

    public void AttackEnter()
    {
        state = PlayerState.Attack;
    }

    public void SpecialAttackEnter()
    {
        state = PlayerState.SpecialAttack;
    }

    public void RollEnter()
    {
        state = PlayerState.Roll;
    }

    public void SwitchWeaponEnter()
    {
        state = PlayerState.SwitchWeapon;
    }

    public void GetHitEnter()
    {
        state = PlayerState.GetHit;
    }

    public void DrinkEnter()
    {
        state = PlayerState.Drink;
    }

    public void SitEnter()
    {
        state = PlayerState.Sit;
    }

    public void DieEnter()
    {
        state = PlayerState.Die;
    }

    // ----------------Update-----------------------------------

    public void MoveUpdate(int n)
    {
        RecoverEnergy();
        StateMoveUpdate();
    }

    public void AttackUpdate(int n)
    {
        if (controller.attack && energy > 0)
        {
            animator.SetTrigger("Attack");
        }
    }

    public void SpecialAttackUpdate(int n)
    {

    }

    public void RollUpdate(int n)
    {

    }

    public void SwitchWeaponUpdate(int n)
    {

    }

    public void GetHitUpdate(int n)
    {

    }

    public void DrinkUpdate(int n)
    {
        if (controller.attack && energy > 0)
        {
            animator.SetTrigger("Attack");
        }
    }

    // ----------------Finish-----------------------------------

    public void MoveExit(int n)
    {
        //rigid.velocity = Vector3.zero;
    }

    public void AttackExit(int n)
    {
        if (n == 3)
        {
            animator.ResetTrigger("Attack");
        }
    }

    public void SpecialAttackExit(int _)
    {
        var hit = GetComponentInChildren<WeaponHit>();
        hit.SpecialParticle(false);
    }

    public void RollExit(int _)
    {
        rigid.velocity = Vector3.zero;
    }

    public void SwitchWeaponExit(int _)
    {
    }

    public void GetHitExit(int _)
    {
        if (health <= 0)
        {
            Die();
        }
    }

    public void DrinkExit(int _)
    {
        potion.gameObject.SetActive(false);
        WeaponManager.Instance.Current.SetActive(true);
    }

    #endregion
}
