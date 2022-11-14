namespace takashicompany.Unity
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class TaBehaviour : MonoBehaviour
	{
		protected Renderer _renderer => ReturnOrGetChildren<Renderer>();
		protected Renderer[] _renderers => ReturnsOrGetChildren<Renderer>();

		private Rigidbody _rigidbodyInternal;

		protected Rigidbody _rigidbody => ReturnOrGet(ref _rigidbodyInternal); // _rigidbodyInternal ?? (_rigidbodyInternal = GetComponent<Rigidbody>());

		private Collider _colliderInternal;

		protected Collider _collider => ReturnOrGet(ref _colliderInternal);

		private Animator _animatorInternal;

		protected Animator _animator => ReturnOrGetChildren(ref _animatorInternal);

		private Dictionary<Type, Component> _componentDict = new Dictionary<Type, Component>();
		private Dictionary<Type, Component[]> _componentsDict = new Dictionary<Type, Component[]>();

		protected T ReturnOrGet<T>() where T : Component
		{
			if (_componentDict.TryGetValue(typeof(T), out var component))
			{
				return (T)component;
			}

			T c = null;

			c = ReturnOrGet<T>(ref c);

			_componentDict.Add(typeof(T), c);

			return c;
		}

		protected T ReturnOrGetChildren<T>() where T : Component
		{
			if (_componentDict.TryGetValue(typeof(T), out var component))
			{
				return (T)component;
			}

			T c = null;

			c = ReturnOrGetChildren<T>(ref c);

			_componentDict.Add(typeof(T), c);

			return c;
		}

		protected T[] ReturnsOrGetChildren<T>() where T : Component
		{
			if (_componentsDict.TryGetValue(typeof(T), out var components))
			{
				return (T[])components;
			}

			var result = GetComponentsInChildren<T>();

			_componentsDict.Add(typeof(T), result);

			return result;
		}

		protected T ReturnOrGetParent<T>() where T : Component
		{
			if (_componentDict.TryGetValue(typeof(T), out var component))
			{
				return (T)component;
			}

			T c = null;

			c = ReturnOrGetParent<T>(ref c);

			_componentDict.Add(typeof(T), c);

			return c;
		}

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
