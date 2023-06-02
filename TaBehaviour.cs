namespace takashicompany.Unity
{
	using System;
	using System.Linq;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class TaBehaviour : MonoBehaviour
	{
		protected Renderer _renderer => ReturnOrGetChildren<Renderer>();
		protected Renderer[] _renderers => ReturnsOrGetChildren<Renderer>();

		protected Rigidbody _rigidbody => ReturnOrGet<Rigidbody>();
		
		protected Collider _collider => ReturnOrGet<Collider>();
		protected Collider2D _collider2D => ReturnOrGet<Collider2D>();

		protected Animator _animator => ReturnOrGetChildren<Animator>();

		private Dictionary<Type, object> _componentDict = new Dictionary<Type, object>();
		private Dictionary<Type, object[]> _componentsDict = new Dictionary<Type, object[]>();

		protected bool HasComponent<T>() 
		{
			return _componentDict.ContainsKey(typeof(T));
		}

		protected bool TryGetComponentByDict<T>(out T component) 
		{
			if (_componentDict.TryGetValue(typeof(T), out var c))
			{
				component = (T)c;
				return true;
			}

			component = default;
			return false;
		}

		protected T ReturnOrGet<T>() 
		{
			if (_componentDict.TryGetValue(typeof(T), out var component))
			{
				return (T)component;
			}

			T c = default;

			c = ReturnOrGet<T>(this, ref c);

			_componentDict.Add(typeof(T), c);

			return c;
		}

		protected T ReturnOrGetChildren<T>() 
		{
			if (_componentDict.TryGetValue(typeof(T), out var component))
			{
				return (T)component;
			}

			T c = default;

			c = ReturnOrGetChildren<T>(this, ref c);

			_componentDict.Add(typeof(T), c);

			return c;
		}

		protected T[] ReturnsOrGetChildren<T>() 
		{
			if (_componentsDict.TryGetValue(typeof(T), out var components))
			{
				return components.ToArray() as T[];
			}

			var result = GetComponentsInChildren<T>();

			_componentsDict.Add(typeof(T), result.ToArray() as object[]);

			return result;
		}

		protected T ReturnOrGetParent<T>() 
		{
			if (_componentDict.TryGetValue(typeof(T), out var component))
			{
				return (T)component;
			}

			T c = default;

			c = ReturnOrGetParent<T>(this, ref c);

			_componentDict.Add(typeof(T), c);

			return c;
		}

		public static T ReturnOrGet<T>(Component c, ref T internalComponent) 
		{
			return internalComponent ?? (internalComponent = c.GetComponent<T>());
		}

		public static T ReturnOrGetChildren<T>(Component c, ref T internalComponent) 
		{
			return internalComponent ?? (internalComponent = c.GetComponentInChildren<T>());
		}

		public static T ReturnOrGetParent<T>(Component c, ref T internalComponent) 
		{
			return internalComponent ?? (internalComponent = c.GetComponentInParent<T>());
		}
	}
}
