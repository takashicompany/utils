namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class RandomState : StateMachineBehaviour 
	{
		[SerializeField]
		private string _toHashString = "random";

		[SerializeField]
		private int _min = 0;

		[SerializeField]
		private int _max = 3;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
// override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
		{
			Debug.Log("Random");
			var hashRandom = Animator.StringToHash(_toHashString);
			animator.SetInteger(hashRandom,  Random.Range (_min, _max));
		}
	}
}