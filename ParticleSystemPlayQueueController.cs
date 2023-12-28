namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// 1つのパーティクルを同じフレームに複数の位置で再生しようとすると、
	/// 最後に指定した位置でしか描画されないので
	/// 再生の要求をキューに積んで1フレームずつずらすコンポーネント
	/// </summary>
	public class ParticleSystemPlayQueueController : MonoBehaviour
	{
		[SerializeField]
		private ParticleSystemPlayQueue _particleSystemPlayQueue;

		private void Awake()
		{
			//
		}

		private void LateUpdate()
		{
			_particleSystemPlayQueue.Update();
		}
	}

	[System.Serializable]
	public class ParticleSystemPlayQueue
	{
		[SerializeField]
		private ParticleSystem _particleSystem;

		private ParticleSystem _main;

		private ParticleSystem[] _particles;

		private Queue<Param> _paramQueue = new Queue<Param>();

		private Dictionary<ParticleSystem, Color> _defaultColors = new Dictionary<ParticleSystem, Color>();

		private struct Param
		{
			public Vector3 position;
			public Color? color;
		}

		public void Init(Transform transform, ParticleSystem overrideParticle = null)
		{
			if (overrideParticle != null)
			{
				_particleSystem = overrideParticle;
			}

			if (_particleSystem != null)
			{
				if (_particleSystem.gameObject.IsPrefab())
				{
					_main = GameObject.Instantiate(_particleSystem, transform);
				}
				else
				{
					_main = _particleSystem;
				}

				_particles = _main.GetComponentsInChildren<ParticleSystem>();

				foreach (var p in _particles)
				{
					_defaultColors.Add(p, p.main.startColor.color);
				}
			}
		}


		public void Play(Vector3 position)
		{
			if (_main != null) _paramQueue.Enqueue(new Param { position = position });
		}

		public void Play(Vector3 position, Color color)
		{
			if (_main != null)
			{
				_paramQueue.Enqueue(new Param { position = position, color = color });
			}
		}

		public void Update()
		{
			if (_main != null && _paramQueue.Count > 0)
			{
				var p = _paramQueue.Dequeue();

				_main.transform.position = p.position;

				if (p.color.HasValue)
				{
					foreach (var ps in _particles)
					{
						var main = ps.main;
						main.startColor = p.color.Value;
					}
				}
				else
				{
					foreach (var ps in _particles)
					{
						var main = ps.main;
						main.startColor = _defaultColors[ps];
					}
				}

				_main.Play(true);
			}
		}
	}
}