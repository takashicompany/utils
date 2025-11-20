namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class Rotator : MonoBehaviour
	{
		[SerializeField]
		private Vector3 _rotatePerSecond = Vector3.forward * 360;

		[SerializeField]
		private bool _isFixedUpdate = false;

		private void Update()
		{
			 if (!_isFixedUpdate) transform.Rotate(_rotatePerSecond * Time.deltaTime);
		}

		private void FixedUpdate()
		{
			if (_isFixedUpdate) transform.Rotate(_rotatePerSecond * Time.fixedDeltaTime);
		}
	}
}