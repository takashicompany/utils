namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class SimpleSoundManager
	{
		private Dictionary<string, AudioClip> _clips = new Dictionary<string, AudioClip>();

		private AudioSource _source;

		public SimpleSoundManager()
		{
			_source = GameObject.FindObjectOfType<AudioSource>();

			if (_source == null)
			{
				var go = new GameObject("SimpleSoundManager");
				_source = go.AddComponent<AudioSource>();
			}
		}

		public void PlayOneShot(string clipName)
		{
			if (!_clips.ContainsKey(clipName))
			{
				var clip = Resources.Load<AudioClip>(clipName);

				if (clip == null)
				{
					Debug.LogError(clipName + " is not found.");
					return;
				}

				_clips.Add(clipName, clip);
			}

			_source.PlayOneShot(_clips[clipName]);
		}
	}
}