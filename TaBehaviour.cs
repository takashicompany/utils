namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class TaBehabviour : MonoBehaviour
	{
		private Rigidbody _rigidbodyInternal;

		protected Rigidbody _rigidbody => ReturnOrGet(ref _rigidbodyInternal); // _rigidbodyInternal ?? (_rigidbodyInternal = GetComponent<Rigidbody>());

		private Collider _colliderInternal;

		protected Collider _collider => ReturnOrGet(ref _colliderInternal);

		private Animator _animatorInternal;

		protected Animator _animator => ReturnOrGetChildren(ref _animatorInternal);

		protected T ReturnOrGet<T>(ref T internalComponent) where T : Component
		{
			return internalComponent ?? (internalComponent = GetComponent<T>());
		}

		protected T ReturnOrGetChildren<T>(ref T internalComponent) where T : Component
		{
			return internalComponent ?? (internalComponent = this.GetSelfOfChildrenGetComponent<T>());
		}

		protected T ReturnOrGetParent<T>(ref T internalComponent) where T : Component
		{
			return internalComponent ?? (internalComponent = this.GetSelfOrParentComponent<T>());
		}
	}

	public static class TaBehaviourExtensions
	{
		public static T GetSelfOrParentComponent<T>(this Component self)
		{
			if (self.TryGetComponent<T>(out var result))
			{
				return result;
			}
			
			return self.GetComponentInParent<T>();
		}

		public static bool TryGetSelfOrParentComponent<T>(this Component self, out T result)
		{
			result = self.GetSelfOrParentComponent<T>();

			return result != null;
		}

		public static T GetSelfOfChildrenGetComponent<T>(this Component self)
		{
			if (self.TryGetComponent<T>(out var result))
			{
				return result;
			}
			
			return self.GetComponentInChildren<T>();
		}
	}
}
