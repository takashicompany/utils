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
	public class ParticleSystemPlayQueue : MonoBehaviour
	{
		[SerializeField]
		private ParticleSystem _particleSystem;

		private Queue<Vector3> _positionQueue = new Queue<Vector3>();

		public void Play(Vector3 position)
		{
			_positionQueue.Enqueue(position);
		}

		private void LateUpdate()
		{
			if (_positionQueue.Count > 0)
			{
				var p = _positionQueue.Dequeue();

				_particleSystem.transform.position = p;
				_particleSystem.Play(true);
			}
		}
	}
}