namespace TakashiCompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class ColliderReceiver : MonoBehaviour
	{
		public delegate void CollisionDelegate(ColliderReceiver colliderReceiver, Collision collision);

		public delegate void TriggerDelegate(ColliderReceiver colliderReceiver, Collider other);

		public event CollisionDelegate onCollisionEnter;
		public event TriggerDelegate onTriggerEnter;

		private void OnCollisionEnter(Collision collision)
		{
			onCollisionEnter?.Invoke(this, collision);
		}

		private void OnTriggerEnter(Collider other)
		{
			onTriggerEnter?.Invoke(this, other);
		}
	}
}