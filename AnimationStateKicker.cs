namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class AnimationStateKicker : KeyDownKicker
	{
		[SerializeField, Header("対象となるAnimator。未設定なら自分→子供の順番で探す。")]
		private Animator _animator;

		[SerializeField, Header("再生したいAnimationControllerのステート名")]
		private string _stateName;

		protected override void OnPressKey()
		{
			if (_animator == null)
			{
				_animator = GetComponentsInChildren<Animator>().FirstOrDefault();
			}
			
			if (_animator != null)
			{
				_animator.Play(_stateName);
			}
		}
	}
}
