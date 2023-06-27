namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class VelocityMover : MonoBehaviour
	{
		[SerializeField]
		private float _maxSpeed = 10;
		
		[SerializeField]
		private float _accelerationPerSeconds = 5;

		private Rigidbody _rigidbodyInternal;
		private Rigidbody _rigidbody => _rigidbodyInternal ?? (_rigidbodyInternal = GetComponent<Rigidbody>());
		
		private Vector3 _direction;
		private Vector3 _velocity;

		private Vector3 _force;
		private float _forceAddTime;
		private float _forceAddDuration;

		private void FixedUpdate()
		{
			var current = _rigidbody.velocity;
			var addSpeed = _direction.normalized * _accelerationPerSeconds * Time.fixedDeltaTime;

			current += addSpeed;

			if (current.magnitude > _maxSpeed)
			{
				current = current.normalized * _maxSpeed;
			}

			if (_forceAddTime + _forceAddDuration > Time.time)
			{
				var t = (Time.time - _forceAddTime) / _forceAddDuration;
				var force = Vector3.Lerp(_force, Vector3.zero, t) * Time.fixedDeltaTime;
				current += force;
			}

			_rigidbody.velocity = current;
		}

		public void AddForce(Vector3 force, float duration)
		{
			_force = force;
			_forceAddTime = Time.time;
			_forceAddDuration = duration;
		}

		public void SetDirection(Vector3 direction)
		{
			_direction = direction;
		}
	}
}