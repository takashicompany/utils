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

		[SerializeField, Header("再生回数。")]
		private int _playCount = 1;

		[SerializeField, Header("再生する間隔")]
		private float _interval = 0.1f;

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

				if (_playCount > 1)
				{
					StartCoroutine(CoPlay());
				}

				IEnumerator CoPlay()
				{
					for (int i = 1; i < _playCount; i++)
					{
						yield return new WaitForSeconds(_interval);
						_particleSystem.Play();
					}
				}
			}
		}
	}
}
