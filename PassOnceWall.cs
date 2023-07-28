namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class PassOnceWall : MonoBehaviour
	{
		[SerializeField]
		private LayerMask _targetLayers = int.MaxValue;

		private Collider _collider;
		private Collider _trigger;
		private Collider2D _collider2D;
		private Collider2D _trigger2D;

		private HashSet<Collider> _colliders = new HashSet<Collider>();
		private HashSet<Collider2D> _colliders2D = new HashSet<Collider2D>();

		private void Awake()
		{
			_collider	= GetComponents<Collider>().FirstOrDefault(c => c.isTrigger == false);
			_trigger	= GetComponents<Collider>().FirstOrDefault(c => c.isTrigger == true);

			_collider2D = GetComponents<Collider2D>().FirstOrDefault(c => c.isTrigger == false);
			_trigger2D	= GetComponents<Collider2D>().FirstOrDefault(c => c.isTrigger == true);
		}

		public void ResetPassedColliders()
		{
			_colliders.Clear();
			_colliders2D.Clear();
		}

		public void IgnoreCollision(Collider collider)
		{
			Physics.IgnoreCollision(_collider, collider);
		}

		public void IgnoreCollision(Collider2D collider)
		{
			Physics2D.IgnoreCollision(_collider2D, collider);
		}

		// private void OnCollisionEnter(Collision collision)
		// {
		// 	if (_targetLayers.Contains(collision.gameObject.layer) && !_colliders.Contains(collision.collider))
		// 	{
		// 		Physics.IgnoreCollision(_collider, collision.collider);
		// 	}
		// }

		// private void OnCollisionEnter2D(Collision2D collision)
		// {
		// 	if (_targetLayers.Contains(collision.gameObject.layer) && !_colliders2D.Contains(collision.collider))
		// 	{
		// 		Physics2D.IgnoreCollision(_collider2D, collision.collider);
		// 	}
		// }

		private void OnTriggerExit(Collider collider)
		{
			if (_targetLayers.Contains(collider.gameObject.layer) && !_colliders.Contains(collider))
			{
				_colliders.Add(collider);
				Physics.IgnoreCollision(_collider, collider, false);
			}
		}

		private void OnTriggerExit2D(Collider2D collider)
		{
			if (_targetLayers.Contains(collider.gameObject.layer) && !_colliders2D.Contains(collider))
			{
				_colliders2D.Add(collider);
				Physics2D.IgnoreCollision(_collider2D, collider, false);
			}
		}
	}
}