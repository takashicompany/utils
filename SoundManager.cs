namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	
	public class SoundManager<T> /*where T : System.Enum*/
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
				var go = new GameObject($"SoundManager{typeof(T).Name}");
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

			PlayOneShot(_clips[clipType], pitch);
		}

		public void PlayOneShot(AudioClip clip, float pitch = -1f)
		{
			var prevPitch = _source.pitch;
			
			if (pitch >= 0)
			{
				_source.pitch = pitch;
			}
			else
			{
				_source.pitch = 1;
			}
			
			_source.PlayOneShot(clip);
		}

		public void Play(AudioClip clip)
		{
			_source.clip = clip;
			_source.Play();
		}
	}

	public class SoundManager : SoundManager<string>
	{

	}
}