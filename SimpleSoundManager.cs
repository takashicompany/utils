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

		public void PlayOneShot(string clipName, float pitch = -1f)
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

			var prevPitch = _source.pitch;
			
			if (pitch >= 0) _source.pitch = pitch;
			
			_source.PlayOneShot(_clips[clipName]);
		}

		public void PlayOneShot(AudioClip clip, float pitch = -1f)
		{
			var prevPitch = _source.pitch;
			
			if (pitch >= 0) _source.pitch = pitch;
			_source.PlayOneShot(clip);
		}
	}
	
	public class SoundManager<T> where T : System.Enum
	{
		private Dictionary<T, AudioClip> _clips = new Dictionary<T, AudioClip>();

		private AudioSource _source;

		private string _resourcesPath;

		public SoundManager(string resourcesPath = "")
		{
			_resourcesPath = resourcesPath;
			_source = GameObject.FindObjectOfType<AudioSource>();

			if (_source == null)
			{
				var go = new GameObject("SimpleSoundManager");
				_source = go.AddComponent<AudioSource>();
			}
		}

		public void PlayOneShot(T clipType, float pitch = -1f)
		{
			if (!_clips.ContainsKey(clipType))
			{
				var clip = Resources.Load<AudioClip>(_resourcesPath + clipType.ToString());

				if (clip == null)
				{
					Debug.LogError(clipType + " is not found.");
					return;
				}

				_clips.Add(clipType, clip);
			}

			var prevPitch = _source.pitch;
			
			if (pitch >= 0)
			{
				_source.pitch = pitch;
			}
			else
			{
				_source.pitch = 1;
			}
			
			_source.PlayOneShot(_clips[clipType]);
		}
	}
}