namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	/// <summary>
	/// 指定のTransformの位置を追跡する
	/// </summary>
	public class SyncPosition : MonoBehaviour
	{
		[SerializeField, Header("追跡したいTransform")]
		private Transform _target;

		[SerializeField, EnumFlag, Header("同期するタイミング。複数選択可能。")]
		private UpdateType _updateType = UpdateType.LateUpdate;

		private void Update()
		{
			if (_updateType.HasFlag(UpdateType.Update)) Sync();
		}

		private void LateUpdate()
		{
			if (_updateType.HasFlag(UpdateType.LateUpdate)) Sync();
		}

		private void FixedUpdate()
		{
			if (_updateType.HasFlag(UpdateType.FixedUpdate)) Sync();
		}

		private void Sync()
		{
			transform.position = _target.position;
		}
	}
}