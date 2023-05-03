using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimState : StateMachineBehaviour
{
    public PlayerState state;
    public int n;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerCharacter player = animator.GetComponent<PlayerCharacter>();
        player.OnAnimStateEnter(state);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerCharacter player = animator.GetComponent<PlayerCharacter>();
        player.OnAnimStateUpdate(state, n);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerCharacter player = animator.GetComponent<PlayerCharacter>();
        player.OnAnimStateExit(state, n);
    }
}
