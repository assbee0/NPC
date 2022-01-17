using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBehavior : StateMachineBehaviour
{
	int hashRandom = Animator.StringToHash("random");

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
    }

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        //animator.SetInteger(hashRandom, Random.Range(0, 4)); // 最大6種類の動作
        animator.SetInteger(hashRandom,  -1); // Wave
    }
}
