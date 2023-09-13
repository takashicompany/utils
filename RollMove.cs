namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class RollMove : MonoBehaviour
	{
		[SerializeField]
		private float _size = 1f;

		[SerializeField]
		private bool _is3D = true;

		private Quaternion _defaultRotation;
		private Vector3 _previousPosition;

		private void Awake()
		{
			_defaultRotation = transform.rotation;
		}

		private void Start()
		{
			_previousPosition = transform.position;
		}

		private void LateUpdate()
		{
			transform.Rotate(GetRollAxis(_previousPosition, transform.position, _is3D), Vector3.Distance(_previousPosition, transform.position) / _size * 90, Space.World);
			_previousPosition = transform.position;
		}

		public void Reset()
		{
			_previousPosition = transform.position;
			transform.rotation = _defaultRotation;
		}

		public static Vector3 GetRollAxis(Vector3 previousPosition, Vector3 currentPosition, bool is3D = true)
		{
			// 実際の移動方向ベクトルを計算
			Vector3 movementDirection = (currentPosition - previousPosition).normalized;

			// 基準となる方向を設定
			Vector3 referenceDirection = Vector3.up;

			if (is3D)
			{
				// 3Dの場合
				if (Mathf.Abs(movementDirection.y) > Mathf.Abs(movementDirection.x) && Mathf.Abs(movementDirection.y) > Mathf.Abs(movementDirection.z))
				{
					referenceDirection = -Vector3.forward;
				}
			}
			else
			{
				// 2Dの場合
				referenceDirection = -Vector3.forward;
			}

			// 基準方向との外積を取ることで、移動方向に対して垂直な軸を計算
			Vector3 rollAxis = Vector3.Cross(referenceDirection, movementDirection);

			return rollAxis;
		}

	}
}