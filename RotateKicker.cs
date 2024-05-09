namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class RotateKicker : MonoBehaviour
	{
		[SerializeField, Header("発動するキー。このオブジェクトが無効状態だと入力を受け付けない。")]
		private KeyCode _key = KeyCode.T;

		[SerializeField, Header("回転の対象。指定しないとこのコンポーネントがついたオブジェクトが対象になる。")]
		private Transform _target;

		[SerializeField, Header("ワールド回転か")]
		private bool _isWorldRotation = false;

		[SerializeField, Header("ランダム回転角の最小値")]
		private Vector3 _minRotation = Vector3.forward * -45;

		[SerializeField, Header("ランダム回転角の最大値")]
		private Vector3 _maxRotation = Vector3.forward * 45;

		private void FixedUpdate()
		{
			if (Input.GetKeyDown(_key))
			{
				var target = _target == null ? transform : _target;

				if (_isWorldRotation)
				{
					target.rotation = Quaternion.Euler(
						Random.Range(_minRotation.x, _maxRotation.x),
						Random.Range(_minRotation.y, _maxRotation.y),
						Random.Range(_minRotation.z, _maxRotation.z)
					);
				}
				else
				{
					target.localRotation = Quaternion.Euler(
						Random.Range(_minRotation.x, _maxRotation.x),
						Random.Range(_minRotation.y, _maxRotation.y),
						Random.Range(_minRotation.z, _maxRotation.z)
					);
				}
			}
		}
	}
}
