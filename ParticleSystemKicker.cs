namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class ParticleSystemKicker : KeyDownKicker
	{
		[SerializeField, Header("対象となるパーティクル。未設定なら自分→子供の順番で探す。")]
		[Header("PlayOnAwakeが有効ならオブジェクトの有効/無効で再生をする")]
		private ParticleSystem _particleSystem;

		protected override void OnPressKey()
		{
			if (_particleSystem == null)
			{
				_particleSystem = GetComponentsInChildren<ParticleSystem>().FirstOrDefault();
			}
			
			if (_particleSystem != null)
			{
				if (_particleSystem.main.playOnAwake)
				{
					_particleSystem.gameObject.SetActive(!_particleSystem.gameObject.activeSelf);
				}
				else
				{
					_particleSystem.Play();
				}
			}
		}
	}
}
