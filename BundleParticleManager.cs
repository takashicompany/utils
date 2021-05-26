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

		private Dictionary<T, Queue<Vector3>> _queues = new Dictionary<T, Queue<Vector3>>();

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

				_queues.Add(b.particleType, new Queue<Vector3>());
			}
		}

		private void LateUpdate()
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

			_queues[particleType].Enqueue(position);
		}

		private void PlayInternal(T particleType, Vector3 position)
		{
			var particle = _dict[particleType].particle;

			particle.transform.position = position;
			particle.Play(true);
		}

	}
}