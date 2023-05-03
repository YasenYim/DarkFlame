using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimState : StateMachineBehaviour
{
    public EnemyState state;
    public int n;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyCharacter enemy = animator.GetComponent<EnemyCharacter>();
        enemy.OnAnimStateEnter(state);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyCharacter enemy = animator.GetComponent<EnemyCharacter>();
        enemy.OnAnimStateUpdate(state, n);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyCharacter enemy = animator.GetComponent<EnemyCharacter>();
        enemy.OnAnimStateExit(state, n);
    }
}
