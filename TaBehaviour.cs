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

		protected Rigidbody _rigidbody => ReturnOrGet<Rigidbody>();
		
		protected Collider _collider => ReturnOrGet<Collider>();
		protected Collider2D _collider2D => ReturnOrGet<Collider2D>();

		protected Animator _animator => ReturnOrGetChildren<Animator>();

		private Dictionary<Type, Component> _componentDict = new Dictionary<Type, Component>();
		private Dictionary<Type, Component[]> _componentsDict = new Dictionary<Type, Component[]>();

		protected bool HasComponent<T>() where T : Component
		{
			return _componentDict.ContainsKey(typeof(T));
		}

		protected bool TryGetComponentByDict<T>(out T component) where T : Component
		{
			if (_componentDict.TryGetValue(typeof(T), out var c))
			{
				component = (T)c;
				return true;
			}

			component = null;
			return false;
		}

		protected T ReturnOrGet<T>() where T : Component
		{
			if (_componentDict.TryGetValue(typeof(T), out var component))
			{
				return (T)component;
			}

			T c = null;

			c = ReturnOrGet<T>(this, ref c);

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

			c = ReturnOrGetChildren<T>(this, ref c);

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

			c = ReturnOrGetParent<T>(this, ref c);

			_componentDict.Add(typeof(T), c);

			return c;
		}

		public static T ReturnOrGet<T>(Component c, ref T internalComponent) where T : Component
		{
			return internalComponent ?? (internalComponent = c.GetComponent<T>());
		}

		public static T ReturnOrGetChildren<T>(Component c, ref T internalComponent) where T : Component
		{
			return internalComponent ?? (internalComponent = c.GetComponentInChildren<T>());
		}

		public static T ReturnOrGetParent<T>(Component c, ref T internalComponent) where T : Component
		{
			return internalComponent ?? (internalComponent = c.GetComponentInParent<T>());
		}
	}
}
