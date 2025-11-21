namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class FixedPosition : MonoBehaviour
	{
		[SerializeField]
		private bool _isLocal = false;

		private Vector3 _position;

		private void OnEnable()
		{
			_position = _isLocal ? transform.localPosition : transform.position;
		}

		private void Update()
		{
			FixPosition();
		}

		private void LateUpdate()
		{
			FixPosition();
		}

		private void FixedUpdate()
		{
			FixPosition();
		}

		private void FixPosition()
		{
			if (_isLocal)
			{
				transform.localPosition = _position;
			}
			else
			{
				transform.position = _position;
			}
		}
	}
}