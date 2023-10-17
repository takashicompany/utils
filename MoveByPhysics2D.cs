namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[RequireComponent(typeof(Rigidbody2D))]
	public class MoveByPhysics2D : MonoBehaviour
	{
		private Rigidbody2D _rigidbody2DInternal;
		private Rigidbody2D _rigidbody2D => _rigidbody2DInternal ? _rigidbody2DInternal : (_rigidbody2DInternal = GetComponent<Rigidbody2D>());

		[SerializeField]
		private float _speed = 1f;

		[SerializeField]
		private Transform _target;
		
		private Vector3? _destination;

		private void FixedUpdate()
		{
			if (_destination == null && _target == null) return;

			Vector2 desiredVelocity = (GetDestination() - _rigidbody2D.position).normalized  * _speed;
			Vector2 forceToAdd = desiredVelocity - _rigidbody2D.velocity;
			_rigidbody2D.AddForce(forceToAdd, ForceMode2D.Force);
		}

		private Vector2 GetDestination()
		{
			return _target ? _target.position : _destination.Value;
		}

		public void SetTarget(Transform target)
		{
			_target = target;
		}

		public void RemoveTarget()
		{
			_target = null;
		}

		public void SetDestination(Vector3 destination)
		{
			_destination = destination;
		}

		public void RemoveDestination()
		{
			_destination = null;
		}
	}
}