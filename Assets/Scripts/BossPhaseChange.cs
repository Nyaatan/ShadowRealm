using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseChange : StateMachineBehaviour
{    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Boss>().invunerable = true;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Boss>().invunerable = false;
        animator.ResetTrigger("PhaseChange");
        animator.GetComponent<Boss>().NextPhase();
    }

}
