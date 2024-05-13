namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class AnimationKicker : KeyDownKicker
	{
		[SerializeField, Header("対象となるアニメーション(Legacy)。未設定なら自分→子供の順番で探す。")]
		private Animation _animation;
		
		[SerializeField, Header("再生したいAnimationの名前。空の場合はそのまま再生する。")]
		private string _animationName;

		protected override void OnPressKey()
		{
			if (_animation == null)
			{
				_animation = GetComponentsInChildren<Animation>().FirstOrDefault();
			}
			
			if (_animation != null)
			{
				if (string.IsNullOrEmpty(_animationName))
				{
					_animation.Play();
				}
				else
				{	
					_animation.Play(_animationName);
				}
			}
		}
	}
}
