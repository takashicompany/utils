namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class SoundKicker : KeyDownKicker
	{
		[System.Serializable]
		private class Sound
		{
			[Header("再生する音声ファイル。")]
			public AudioClip clip;

			[Header("再生する音量の係数。マイナスならデフォルトの設定を使う。")]
			public float volumeScale = -1f;

			[Header("キーを押してからSEが再生されるまでの遅延秒数")]
			public float delay = 0f;
		}

		private AudioSource _audioSource;

		[SerializeField, Header("デフォルトの音量の係数")]
		private float _volumeScale = 1f;

		[SerializeField, Header("鳴らすSEの設定。複数設定が可能")]
		private Sound[] _sounds;

		private void Awake()
		{
			if (!TryGetComponent(out _audioSource))
			{
				_audioSource = gameObject.AddComponent<AudioSource>();
			}
		}

		protected override void OnPressKey()
		{
			foreach (var sound in _sounds)
			{
				StartCoroutine(Co(sound));
			}

			IEnumerator Co(Sound sound)
			{
				if (sound.delay > 0f)
				{
					yield return new WaitForSeconds(sound.delay);
				}

				_audioSource.PlayOneShot(sound.clip, sound.volumeScale < 0f ? _volumeScale : sound.volumeScale);
			}
		}
	}
}
