namespace Monopolis
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class AnimationStateKicker : MonoBehaviour
	{
		[SerializeField, Header("対象となるAnimator。未設定なら自分→子供の順番で探す。")]
		private Animator _animator;

		[SerializeField, Header("発動するキー。このオブジェクトが無効状態だと入力を受け付けない。")]
		private KeyCode _key = KeyCode.T;

		[SerializeField, Header("再生したいAnimationControllerのステート名")]
		private string _stateName;

		private  void LateUpdate()
		{
			if (Input.GetKeyDown(_key))
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
}
