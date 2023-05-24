namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class AnimationEventWithStringReceiver : AnimationEventReceiver<string>
	{
		private Animator _animator;
		public Animator animator => _animator ?? (_animator = GetComponent<Animator>());
	}
}