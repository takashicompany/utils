namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class LoopSoundKicker : KeyDownKicker
	{
		[SerializeField, Header("再生する音声ファイル。")]
		private AudioClip _clip;

		[SerializeField, Header("再生する音量の係数")]
		private float _volumeScale = 1f;

		[SerializeField, Header("音声をループさせるか")]
		private bool _loop = false;

		[SerializeField, Header("停止用のキー")]
		private KeyCode _stopKey = KeyCode.Y;

		private AudioSource _audioSource;

		private void Awake()
		{
			_audioSource = gameObject.AddComponent<AudioSource>();
		}

		protected override bool ProcessKeyDown()
		{
			var a = base.ProcessKeyDown();
			
			var b = false;

			if (Input.GetKeyDown(_stopKey))
			{
				_audioSource.Stop();
				b = true;
			}

			return a || b;
		}

		public override bool TryEmulateKey(KeyCode key, UpdateType updateType)
		{
			var a = base.TryEmulateKey(key, updateType);

			var b = false;

			if (key == _stopKey && IsUpdateTiming(updateType))
			{
				Stop();
				b = true;
			}

			return a || b;
		}

		protected override void OnPressKey()
		{
			_audioSource.clip = _clip;
			_audioSource.loop = _loop;
			_audioSource.volume = _volumeScale;
			_audioSource.Play();
		}

		private void Stop()
		{
			_audioSource.Stop();
		}
	}
}
