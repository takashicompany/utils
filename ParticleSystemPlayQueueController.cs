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

		private ParticleSystem _p;

		private Queue<Vector3> _positionQueue = new Queue<Vector3>();

		public void Init(Transform transform)
		{
			if (_particleSystem != null)
			{
				if (_particleSystem.gameObject.IsPrefab())
				{
					_p = GameObject.Instantiate(_particleSystem, transform);
				}
				else
				{
					_p = _particleSystem;
				}
			}
		}


		public void Play(Vector3 position)
		{
			_positionQueue.Enqueue(position);
		}

		public void Update()
		{
			if (_positionQueue.Count > 0)
			{
				var p = _positionQueue.Dequeue();

				_p.transform.position = p;
				_p.Play(true);
			}
		}
	}
}