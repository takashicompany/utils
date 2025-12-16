namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class FixedRotation : UpdateTypeSelector
	{
		[SerializeField]
		private Vector3 _fixedEulerAngles;

		[SerializeField, Header("Awake時の角度を使用する")]
		private bool _useAwakeRotation = true;

		private Quaternion _fixedRotation;

		private void Awake()
		{
			if (_useAwakeRotation)
			{
				_fixedRotation = transform.rotation;
			}
			else
			{
				_fixedRotation = Quaternion.Euler(_fixedEulerAngles);
			}
		}

		protected override void OnUpdate()
		{
			transform.rotation = _fixedRotation;
		}

		protected override void OnLateUpdate()
		{
			transform.rotation = _fixedRotation;
		}

		protected override void OnFixedUpdate()
		{
			transform.rotation = _fixedRotation;
		}
	}
}
