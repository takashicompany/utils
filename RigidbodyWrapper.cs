namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public abstract class ComponentWrapper<T, A, B> where T : Component where A : Component where B : Component
	{
		[SerializeField]
		protected T _component;

		protected A _a;
		protected B _b;

		private bool _isInit;

		public ComponentWrapper(T component)
		{
			_component = component;
			Init();
		}

		protected void Init()
		{
			if (_isInit || _component == null)
			{
				return;
			}

			if (_a == null && _b == null)
			{
				_a = _component.GetComponent<A>();
				_b = _component.GetComponent<B>();
			}

			_isInit = true;
		}

		public bool HasInstance()
		{
			Init();
			return _a != null || _b != null;
		}
	}

	public class ComponentWrapper<A, B> : ComponentWrapper<Component, A, B> where A : Component where B : Component
	{
		public ComponentWrapper(Component component) : base(component)
		{

		}
	}

	[System.Serializable]
	public class RigidbodyWrapper : ComponentWrapper<Rigidbody, Rigidbody2D>
	{
		public RigidbodyWrapper(Component component) : base(component)
		{
			
		}
	}
}