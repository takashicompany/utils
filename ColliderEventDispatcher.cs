namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.Events;

	public class ColliderEventDispatcher : MonoBehaviour
	{
		private Collider _colliderInternal;
		private Collider _collider => _colliderInternal ?? (_colliderInternal = GetComponent<Collider>());
		public new Collider collider => _collider;

		[SerializeField]
		private UnityEvent<Collision> _onCollisionEnter = new ();
		public UnityEvent<Collision> onCollisionEnter => _onCollisionEnter;

		[SerializeField]
		private UnityEvent<Collision> _onCollisionStay = new ();
		public UnityEvent<Collision> onCollisionStay => _onCollisionStay;

		[SerializeField]
		private UnityEvent<Collision> _onCollisionExit = new ();
		public UnityEvent<Collision> onCollisionExit => _onCollisionExit;

		[SerializeField]
		private UnityEvent<Collider> _onTriggerEnter = new ();
		public UnityEvent<Collider> onTriggerEnter => _onTriggerEnter;

		[SerializeField]
		private UnityEvent<Collider> _onTriggerStay = new ();
		public UnityEvent<Collider> onTriggerStay => _onTriggerStay;

		[SerializeField]
		private UnityEvent<Collider> _onTriggerExit = new ();
		public UnityEvent<Collider> onTriggerExit => _onTriggerExit;


		private void OnCollisionEnter(Collision collision)
		{
			_onCollisionEnter?.Invoke(collision);
		}

		private void OnCollisionStay(Collision collision)
		{
			_onCollisionStay?.Invoke(collision);
		}

		private void OnCollisionExit(Collision collision)
		{
			_onCollisionExit?.Invoke(collision);
		}

		private void OnTriggerEnter(Collider other)
		{
			_onTriggerEnter?.Invoke(other);
		}

		private void OnTriggerStay(Collider other)
		{
			_onTriggerStay?.Invoke(other);
		}

		private void OnTriggerExit(Collider other)
		{
			_onTriggerExit?.Invoke(other);
		}
	}
}