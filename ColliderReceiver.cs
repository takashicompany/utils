namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class ColliderReceiver : MonoBehaviour
	{
		public delegate void CollisionDelegate(Collision collision);

		public delegate void TriggerDelegate(Collider other);

		public event CollisionDelegate onCollisionEnter;
		public event TriggerDelegate onTriggerEnter;

		private void OnCollisionEnter(Collision collision)
		{
			onCollisionEnter?.Invoke(collision);
		}

		private void OnTriggerEnter(Collider other)
		{
			onTriggerEnter?.Invoke(other);
		}
	}
}