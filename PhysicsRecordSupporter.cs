namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[RequireComponent(typeof(Rigidbody))]
	public class PhysicsRecordSupporter : RecordSupporter
	{
		[SerializeField]
		private Vector3 _minPower;

		[SerializeField]
		private Vector3 _maxPower;

		[SerializeField]
		private ForceMode _powerForceMode = ForceMode.Impulse;

		[SerializeField]
		private Vector3 _minTorque;

		[SerializeField]
		private Vector3 _maxTorque;

		[SerializeField]
		private ForceMode _torqueForceMode = ForceMode.Impulse;

		private RigidbodyWrapper _rigidbody;
		private Vector3 _startPosition;

		private void Awake()
		{
			_rigidbody = new RigidbodyWrapper(gameObject);
			_rigidbody.isKinematic = true;
			_startPosition = transform.position;
		}
		public override void StartRecord()
		{
			transform.position = _rigidbody.position = _startPosition;
			_rigidbody.isKinematic = false;

			_rigidbody.AddForce(new Vector3(
				Random.Range(_minPower.x, _maxPower.x),
				Random.Range(_minPower.y, _maxPower.y),
				Random.Range(_minPower.z, _maxPower.z)
			), ForceMode.Impulse);

			_rigidbody.AddTorque(new Vector3(
				Random.Range(_minTorque.x, _maxTorque.x),
				Random.Range(_minTorque.y, _maxTorque.y),
				Random.Range(_minTorque.z, _maxTorque.z)
			), ForceMode.Impulse);

			base.StartRecord();
		}

		public override void PlayLastClip()
		{
			_rigidbody.isKinematic = true;
			base.PlayLastClip();
		}
	}

#if UNITY_EDITOR
	[UnityEditor.CustomEditor(typeof(PhysicsRecordSupporter))]
	public class PhysicsRecordSupporterEditor : RecordSupporterEditor<PhysicsRecordSupporter>
	{
		
	}
#endif
}