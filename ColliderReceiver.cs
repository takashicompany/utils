namespace TakashiCompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class ColliderReceiver : MonoBehaviour
	{
		public delegate void CollisionDelegate(ColliderReceiver collisionReceiver, Collision collision);

		public event CollisionDelegate onCollisionEnter;

		private void OnCollisionEnter(Collision collision)
		{
			onCollisionEnter?.Invoke(this, collision);
		}
	}
}