namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class Chaser : MonoBehaviour
	{
		public class Updater
		{
			private Transform _chaser;
			private Transform _target;
			private Vector3 _offsetTargetLocalPosition;
			private float _speed;

			private Vector3 _prevPosition;

			public Updater(Transform chaser, Transform target, Vector3 offsetTargetLocalPosition, float speed)
			{
				_chaser = chaser;
				_target = target;
				_offsetTargetLocalPosition = offsetTargetLocalPosition;
				ChangeSpeed(speed);

				_prevPosition = _chaser.position;
			}

			public void ChangeSpeed(float speed)
			{
				_speed = speed;
			}

			public bool Update(float deltaTime)
			{
				var destination = _target.TransformPoint(_offsetTargetLocalPosition);

				var distance = Vector3.Distance(_chaser.position, destination);

				var speedPerFrame = _speed * deltaTime;

				if (distance < speedPerFrame)
				{
					_chaser.position = destination;
					return true;
				}

				var direction = (destination - _prevPosition).normalized;

				_chaser.position = _prevPosition + direction * speedPerFrame;

				_prevPosition = _chaser.position;

				return false;
			}
		}
	}
}