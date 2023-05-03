using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Sleeping,
    Move,
    Attack,
    GetHit,
    Die
}

public class EnemyCharacter : MonoBehaviour
{
    Animator animator;
    Rigidbody rigid;

    public EnemyState state = EnemyState.Sleeping;

    PlayerCharacter player;

    public int health = 100;    //生命值
    public int attack = 5;    //攻击力
    public int souls = 100;     //掉落魂量
    public int moveSpeed = 2;       // 移动速度
    public Transform damagePoint;   // 攻击判定的原点（物体）
    public float damageRadius = 2.0f;   //攻击判定半径
    public int attackAngle = 360;   // 攻击角度范围

    float disappearTime;    //到尸体消失的时间
    float alertDistance = 10f;  //发现玩家的最大距离
    float stopDistance = 2f;   //移动到据玩家该距离时停止移动

    public ParticleSystem deathParticle;
    public ParticleSystem bloodParticle;
    Transform soulsParticle;
    Transform soulsClone;
    Transform soulsParent;

    EnemyAudio enemyAudio;

    Vector3 move;
    float dieTime = -1;

	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>();
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        enemyAudio = GetComponent<EnemyAudio>();

        deathParticle = transform.Find("Death_Particle").GetComponent<ParticleSystem>();
        bloodParticle = transform.Find("BloodParticle").GetComponent<ParticleSystem>();
        soulsParticle = Resources.Load<Transform>("Prefabs/SoulsTrailer");

        Transform soulsBirthPlace = transform.Find("SoulsBirthPlace");
        soulsClone = Instantiate(soulsParticle, soulsBirthPlace);
        soulsClone.transform.position = soulsBirthPlace.position;
        soulsClone.gameObject.AddComponent<ParticleMove>();
        soulsClone.gameObject.SetActive(false);
        soulsParent = GameObject.Find("ParticleParent").GetComponent<Transform>();
    }

    // Update的代码全都分散到各个动画状态Update中了
    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (state == EnemyState.Move)
        {
            rigid.MovePosition(transform.position + move * moveSpeed * Time.fixedDeltaTime);
        }
    }

    // 敌人转向目标方向
    internal void Rotate(Vector3 dir)
    {
        Quaternion faceToQuat = Quaternion.LookRotation(dir); //角色面朝目标方向的四元数
        Quaternion slerp = Quaternion.Slerp(transform.rotation, faceToQuat, 0.2f);
        transform.rotation = slerp;
    }


    // 敌人攻击
    void Attack()
    {
        int ran = Random.Range(1, 4);
        animator.SetInteger("Attack", ran);
    }

    public void GetHit(int damage)
    {
        if (state == EnemyState.Die)
        {
            return;
        }
        health -= damage;
        enemyAudio.GetHitAudio();
        bloodParticle.Play();

        animator.SetTrigger("GetHit");

        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    public void Die()
    {
        animator.SetTrigger("Die");
        deathParticle.Play();
        transform.GetComponent<Collider>().isTrigger = true;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public void HitTest()
    {
        var checkPos = damagePoint.position;
        var colliders = Physics.OverlapSphere(checkPos, damageRadius, LayerMask.GetMask("Player"));

        if (colliders.Length > 0)
        {
            foreach (var col in colliders)
            {
                var target = col.gameObject.transform.position - transform.position;
                var angle = Vector3.Angle(transform.forward, target);

                if (angle <= attackAngle && player.state != PlayerState.Die)
                {
                    //Debug.Log("Enemy Hit Player " + attack);
                    player.GetHit(attack);
                }
            }
        }
    }

    // -- 由动画状态机驱动核心状态机 -- 
    #region 核心状态机

    delegate void FuncStateEnter();
    delegate void FuncStateUpdate(int n);
    delegate void FuncStateFinish(int n);

    public void OnAnimStateEnter(EnemyState s)
    {
        Debug.Log("进入状态" + s);
        Dictionary<EnemyState, FuncStateEnter> dict = new Dictionary<EnemyState, FuncStateEnter>
        {
            {EnemyState.Move, MoveEnter},
            {EnemyState.Attack, AttackEnter},
            {EnemyState.Sleeping, SleepingEnter},
            {EnemyState.GetHit, GetHitEnter},
            {EnemyState.Die, DieEnter},
        };

        if (dict.ContainsKey(s) && dict[s] != null)
        {
            dict[s]();
        }
    }

    public void OnAnimStateUpdate(EnemyState s, int n)
    {
        Dictionary<EnemyState, FuncStateUpdate> dict = new Dictionary<EnemyState, FuncStateUpdate>
        {
            {EnemyState.Move, MoveUpdate},
            {EnemyState.Attack, AttackUpdate},
            {EnemyState.Sleeping, SleepingUpdate},
            {EnemyState.GetHit, null},
            {EnemyState.Die, DieUpdate},
        };

        if (dict.ContainsKey(s) && dict[s] != null)
        {
            dict[s](n);
        }
    }

    public void OnAnimStateExit(EnemyState s, int n)
    {
        Dictionary<EnemyState, FuncStateFinish> dict = new Dictionary<EnemyState, FuncStateFinish>
        {
            {EnemyState.Move, null},
            {EnemyState.Attack, AttackExit},
            {EnemyState.Sleeping, null},
            {EnemyState.GetHit, GetHitExit},
            {EnemyState.Die, null},
        };

        if (dict.ContainsKey(s) && dict[s] != null)
        {
            dict[s](n);
        }
    }

    // ----------------Enter-----------------------------------

    public void MoveEnter()
    {
        state = EnemyState.Move;
    }

    public void AttackEnter()
    {
        state = EnemyState.Attack;
    }

    public void GetHitEnter()
    {
        state = EnemyState.GetHit;
    }

    public void DieEnter()
    {
        state = EnemyState.Die;
    }

    public void SleepingEnter()
    {
        state = EnemyState.Sleeping;
    }

    // ----------------Update-----------------------------------
    public void MoveUpdate(int n)
    {
        if (player.state == PlayerState.Die)
        {
            move = Vector3.zero;
            animator.SetBool("Move", false);
            return;
        }

        Vector3 playerPos = player.transform.position;
        Vector3 to = playerPos - transform.position;
        float dist = to.magnitude; //敌人距玩家的距离

        if (dist > alertDistance)
        {
            move = Vector3.zero;
            animator.SetBool("Move", false);
            return;
        }
        Rotate(to);
        move = transform.forward;
        animator.SetBool("Move", true);

        if (dist <= stopDistance)
        {
            animator.SetBool("Move", false);
            Attack();
        }
    }

    public void AttackUpdate(int n)
    {
    }

    public void SleepingUpdate(int n)
    {
        Vector3 playerPos = player.transform.position;
        Vector3 to = playerPos - transform.position;
        float dist = to.magnitude; //敌人距玩家的距离

        if (dist < alertDistance)
        {
            animator.SetTrigger("WakeUp");
        }
    }

    public void DieUpdate(int n)
    {
        if (dieTime < 0)
        {
            dieTime = Time.time;
        }
        if (Time.time > dieTime + 3 && soulsClone != null)
        {
            soulsClone.gameObject.SetActive(true);
            soulsClone.GetComponentInChildren<ParticleSystem>().Play();
            soulsClone.SetParent(soulsParent);
            ParticleMove pm = soulsClone.GetComponent<ParticleMove>();
            pm.numSouls = souls;
            pm.StartMove(player.transform);
            soulsClone = null;
        }
        if (Time.time > dieTime + 6)
        {
            Destroy(gameObject);
        }
    }

    // ----------------Finish-----------------------------------

    public void AttackExit(int n)
    {
        animator.SetInteger("Attack", 0);
    }

    public void GetHitExit(int n)
    {
        animator.ResetTrigger("GetHit");
    }

    #endregion
}
