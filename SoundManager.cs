namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	
	public class SoundManager<T> /*where T : System.Enum*/
	{
		private Dictionary<T, AudioClip> _clips = new Dictionary<T, AudioClip>();

		private Dictionary<int, AudioSource> _sources = new ();

		private string _resourcesPath;

		public SoundManager(string resourcesPath = "")
		{
			_resourcesPath = resourcesPath;
			AddAudioSource(0);
		}

		public AudioSource AddAudioSource(int index)
		{
			if (_sources.ContainsKey(index))	// ここの実装、ちょっと冗長になっているな。
			{
				return _sources[index];
			}

			var go = new GameObject($"SoundManager{typeof(T).Name}_{index}");
			var source = go.AddComponent<AudioSource>();
			_sources.Add(index, source);
			GameObject.DontDestroyOnLoad(go);
			return source;
		}

		protected virtual string BuildPath(T clipType)
		{
			return _resourcesPath + clipType.ToString();
		}

		public void LoadClip(T clipType)
		{
			if (!_clips.ContainsKey(clipType))
			{
				var clip = Resources.Load<AudioClip>(BuildPath(clipType));

				if (clip == null)
				{
					Debug.LogError(clipType + " is not found.");
					return;
				}

				_clips.Add(clipType, clip);
			}
		}

		public void PlayOneShot(T clipType, float pitch = -1f, int index = 0)
		{
			LoadClip(clipType);
			PlayOneShot(_clips[clipType], pitch);
		}

		public void PlayOneShot(AudioClip clip, float pitch = -1f, int index = 0)
		{
			var source = GetAudioSource(index);
			var prevPitch = source.pitch;
			
			if (pitch >= 0)
			{
				source.pitch = pitch;
			}
			else
			{
				source.pitch = 1;
			}
			
			source.PlayOneShot(clip);
		}

		public void Play(T clipType, bool loop, int index = 0)
		{
			LoadClip(clipType);
			Play(_clips[clipType], loop, index);
		}

		public void Play(AudioClip clip, bool loop,  int index = 0)
		{
			var source = GetAudioSource(index);

			source.clip = clip;
			source.loop = loop;
			source.Play();
		}

		public void Stop(int index = 0)
		{
			if (_sources.ContainsKey(index))
			{
				var source = _sources[index];
				source.Stop();
			}
		}

		public AudioSource GetAudioSource(int index = 0)
		{
			if (!_sources.ContainsKey(index))
			{
				return AddAudioSource(index);
			}

			return _sources[index];
		}
	}

	public class SoundManagerEnum<T> : SoundManager<T> where T : System.Enum
	{
		public SoundManagerEnum(string resourcesPath = "") : base(resourcesPath)
		{

		}

		public void LoadAll()
		{
			var enumType = typeof(T);
			
			foreach (T val in System.Enum.GetValues(enumType))
			{
				LoadClip(val);
			}
		}
	}

	public class SoundManager : SoundManager<string>
	{

	}
}
