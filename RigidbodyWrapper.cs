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
			Init();
		}

		public ComponentWrapper(GameObject gameObject) : base(gameObject.GetComponent<Component>())
		{

		}
	}

	[System.Serializable]
	public class RigidbodyWrapper : ComponentWrapper<Rigidbody, Rigidbody2D>
	{
		public RigidbodyWrapper(Component component) : base(component)
		{

		}

		public RigidbodyWrapper(GameObject gameObject) : base(gameObject)
		{

		}

		public Vector3 velocity
		{
			get
			{
				Init();
				if (Is2D())
				{
					return _b.velocity;
				}
				else if (Is3D())
				{
					return _a.velocity;
				}
				else
				{
					Debug.LogError("初期化に失敗しているかもしれません。");
					return Vector3.zero;
				}
			}
			set
			{
				Init();
				if (Is2D())
				{
					_b.velocity = value;
				}
				else if (Is3D())
				{
					_a.velocity = value;
				}
				else
				{
					Debug.LogError("初期化に失敗しているかもしれません。");
				}
			}
		}

		public Vector3 position
		{
			get
			{
				Init();
				if (Is2D())
				{
					return _b.position;
				}
				else if (Is3D())
				{
					return _a.position;
				}
				else
				{
					Debug.LogError("初期化に失敗しているかもしれません。");
					return Vector3.zero;
				}
			}
			set
			{
				Init();
				if (Is2D())
				{
					_b.position = value;
				}
				else if (Is3D())
				{
					_a.position = value;
				}
				else
				{
					Debug.LogError("初期化に失敗しているかもしれません。");
				}
			}
		}

		public Quaternion rotation
		{
			get
			{
				Init();
				if (Is2D())
				{
					return Quaternion.Euler(0, 0, _b.rotation);
				}
				else if (Is3D())
				{
					return _a.rotation;
				}
				else
				{
					Debug.LogError("初期化に失敗しているかもしれません。");
					return Quaternion.identity;
				}
			}
			set
			{
				Init();
				if (Is2D())
				{
					_b.rotation = value.eulerAngles.z;
				}
				else if (Is3D())
				{
					_a.rotation = value;
				}
				else
				{
					Debug.LogError("初期化に失敗しているかもしれません。s");
				}
			}
		}
		
		public bool Is3D()
		{
			Init();
			return _a != null;
		}

		public bool Is2D()
		{
			Init();
			return _b != null;
		}

		public void AddForce(Vector3 force)
		{
			Init();

			if (Is2D())
			{
				_b.AddForce(force);
			}
			else if (Is3D())
			{
				_a.AddForce(force);
			}
			else
			{
				Debug.LogError("初期化に失敗しているかもしれません。s");
			}
		}

		public void Sleep()
		{
			Init();

			if (Is2D())
			{
				_b.Sleep();
			}
			else if (Is3D())
			{
				_a.Sleep();
			}
			else
			{
				Debug.LogError("初期化に失敗しているかもしれません。s");
			}
		}

		public void MovePosition(Vector3 position)
		{
			Init();

			if (Is2D())
			{
				_b.MovePosition(position);
			}
			else if (Is3D())
			{
				_a.MovePosition(position);
			}
			else
			{
				Debug.LogError("初期化に失敗しているかもしれません。s");
			}
		}

		public void MoveRotation(Quaternion rotation)
		{
			Init();

			if (Is2D())
			{
				_b.MoveRotation(rotation.eulerAngles.z);
			}
			else if (Is3D())
			{
				_a.MoveRotation(rotation);
			}
			else
			{
				Debug.LogError("初期化に失敗しているかもしれません。s");
			}
		}

	}
}