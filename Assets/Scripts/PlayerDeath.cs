using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : StateMachineBehaviour
{
    public float timer = 3f;
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer -= Time.deltaTime;
        if (timer <= 0) animator.SetBool("Death", false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameManager.Instance.dungeon.EnterTavern();
        GameManager.Instance.player.GetComponent<Player>().Revive();
    }
}
