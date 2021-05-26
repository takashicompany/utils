namespace TakashiCompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class BundleParticleManager<T> : ParticleManager<T> where T : System.Enum
	{
		[System.Serializable]
		private class Bundle
		{
			[SerializeField]
			private T _particleType;

			[SerializeField]
			private ParticleSystem _particle;

			public T particleType => _particleType;
			
			public ParticleSystem particle => _particle;
		}

		[SerializeField]
		private Bundle[] _bundles;

		private Dictionary<T, Bundle> _dict = new Dictionary<T, Bundle>();

		private Dictionary<T, Queue<PRS>> _queues = new Dictionary<T, Queue<PRS>>();

		private void Awake()
		{
			foreach (var b in _bundles)
			{
				if (b == null || b.particle == null)
				{
					Debug.LogError("設定が不正です");
					continue;
				}

				_dict.Add(b.particleType, b);

				_queues.Add(b.particleType, new Queue<PRS>());
			}
		}

		private void LateUpdate()	// LateUpdateじゃない方法でParticleを再生する方法があればMonoBehaviour使わなくて良くなる
		{
			foreach (var kvp in _queues)
			{
				var queue = kvp.Value;
				
				if (queue.Count == 0)
				{
					continue;
				}

				var p = queue.Dequeue();

				PlayInternal(kvp.Key, p);
			}
		}

		public void Play(T particleType, Vector3 position)
		{
			if (!_dict.ContainsKey(particleType))
			{
				Debug.LogError("設定されていないParticleTypeです:" + particleType);
				return;
			}

			_queues[particleType].Enqueue(new PRS(position));
		}

		public void Play(T particleType, Vector3 position, Quaternion rotation)
		{
			if (!_dict.ContainsKey(particleType))
			{
				Debug.LogError("設定されていないParticleTypeです:" + particleType);
				return;
			}

			_queues[particleType].Enqueue(new PRS(position, rotation));
		}

		public void Play(T particleType, Vector3 position, Quaternion rotation, Vector3 scale)
		{
			if (!_dict.ContainsKey(particleType))
			{
				Debug.LogError("設定されていないParticleTypeです:" + particleType);
				return;
			}

			_queues[particleType].Enqueue(new PRS(position, rotation, scale));
		}

		private void PlayInternal(T particleType, PRS param)
		{
			var particle = _dict[particleType].particle;

			param.Set(particle.transform);
			
			particle.Play(true);
		}

	}
}