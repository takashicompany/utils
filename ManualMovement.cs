namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class ManualMovement : MonoBehaviour
	{
		private Rigidbody _rigidbodyInternal;
		private Rigidbody _rigidbody => _rigidbodyInternal ?? (_rigidbodyInternal = GetComponent<Rigidbody>());

		[SerializeField]
		private float _maxSpeed = 10;

		[SerializeField]
		private float _accelerationPerSeconds = 10;

		private Vector3 _velocity;

		private void FixedUpdate()
		{
			
		}
	}
}