namespace takashicompany.Unity
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.UI;
	using DG.Tweening;

#if UNITY_EDITOR
	using UnityEditor;

	[CustomPropertyDrawer(typeof(ComponentWrapper), true)]
	public class ComponentWrapperDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			// _component フィールドを描画します。
			SerializedProperty componentProperty = property.FindPropertyRelative("_component");
			EditorGUI.PropertyField(
				new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
				componentProperty,
				label,
				true
			);

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			// _component フィールドのみ表示するため、単一行の高さを返します。
			return EditorGUIUtility.singleLineHeight;
		}
	}
#endif

	public abstract class ComponentWrapper
	{
		[SerializeField]
		protected Component _component;
	}

	public abstract class ComponentWrapper<T, A, B> : ComponentWrapper where T : Component where A : Component where B : Component
	{
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

		protected bool IsA()
		{
			Init();
			return _a != null;
		}

		protected bool IsB()
		{
			Init();
			return _b != null;
		}

		public GameObject gameObject => _component.gameObject;
		public Transform transform => _component.transform;

		protected V Get<V>(Func<V> funcA, Func<V> funcB)
		{
			if (IsA())
			{
				return funcA();
			}
			else if (IsB())
			{
				return funcB();
			}
			else
			{
				Debug.LogError("初期化に失敗しているかもしれません。");
				return default(V);
			}
		}

		protected void Set<V>(Action<V> setA, Action<V> setB, V value)
		{
			if (IsA())
			{
				setA(value);
			}
			else if (IsB())
			{
				setB(value);
			}
			else
			{
				Debug.LogError("初期化に失敗しているかもしれません。");
			}
		}

		protected void Execute(Action executeA, Action executeB)
		{
			if (IsA())
			{
				executeA();
			}
			else if (IsB())
			{
				executeB();
			}
			else
			{
				Debug.LogError("初期化に失敗しているかもしれません。");
			}
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
			get => Get<Vector3>(() => _a.velocity, () => _b.velocity);
			set => Set(v => _a.velocity = v, v => _b.velocity = v, value);
		}

		public Vector3 position
		{
			get => Get<Vector3>(() => _a.position, () => _b.position);
			set => Set(v => _a.position = v, v => _b.position = v, value);
		}
		
		public Quaternion rotation
		{
			get => Get<Quaternion>(() => _a.rotation, () => Quaternion.Euler(0, 0, _b.rotation));
			set => Set(v => _a.rotation = v, v => _b.rotation = v.eulerAngles.z, value);
		}
		
		public bool Is3D()
		{
			return IsA();
		}

		public bool Is2D()
		{
			return IsB();
		}

		public Rigidbody d3 => _a;
		public Rigidbody2D d2 => _b;

		public void AddForce(Vector3 force)
		{
			Execute(() => _a.AddForce(force), () => _b.AddForce(force));
		}

		public void Sleep()
		{
			Execute(() => _a.Sleep(), () => _b.Sleep());
		}

		public void MovePosition(Vector3 position)
		{
			Execute(() => _a.MovePosition(position), () => _b.MovePosition(position));
		}

		public void MoveRotation(Quaternion rotation)
		{
			Execute(() => _a.MoveRotation(rotation), () => _b.MoveRotation(rotation.eulerAngles.z));
		}

	}

	[System.Serializable]
	public class ColliderWrapper : ComponentWrapper<Collider, Collider2D>
	{
		public ColliderWrapper(Component component) : base(component)
		{

		}

		public ColliderWrapper(GameObject gameObject) : base(gameObject)
		{

		}

		public bool Is3D()
		{
			return IsA();
		}

		public bool Is2D()
		{
			return IsB();
		}

		public bool isTrigger
		{
			get => Get(() => _a.isTrigger, () => _b.isTrigger);
			set => Set(v => _a.isTrigger = v, v => _b.isTrigger = v, value);
		}

		public bool enabled
		{
			get => Get(() => _a.enabled, () => _b.enabled);
			set => Set(v => _a.enabled = v, v => _b.enabled = v, value);
		}

		public Bounds bounds
		{
			get => Get(() => _a.bounds, () => _b.bounds);
		}
	}

	[System.Serializable]
	public class SpriteWrapper : ComponentWrapper<SpriteRenderer, Image>
	{
		public SpriteWrapper(Component component) : base(component)
		{
			
		}

		public SpriteWrapper(GameObject gameObject) : base(gameObject)
		{
			
		}

		public Sprite sprite
		{
			get => Get(() => _a.sprite, () => _b.sprite);
			set => Set(v => _a.sprite = v, v => _b.sprite = v, value);
		}

		public Color color
		{
			get => Get(() => _a.color, () => _b.color);
			set => Set(v => _a.color = v, v => _b.color = v, value);
		}

		public float alpha
		{
			get => Get(() => _a.color.a, () => _b.color.a);
			set => Set(v => _a.color = new Color(_a.color.r, _a.color.g, _a.color.b, v), v => _b.color = new Color(_b.color.r, _b.color.g, _b.color.b, v), value);
		}

		public void DOKill()
		{
			Execute(() => _a.DOKill(), () => _b.DOKill());
		}

		public DG.Tweening.Core.TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions> DOFade(float alpha, float duration)
		{
			return Get(() => _a.DOFade(alpha, duration), () => _b.DOFade(alpha, duration));
		}

		public DG.Tweening.Core.TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions> DOColor(Color color, float duration)
		{
			return Get(() => _a.DOColor(color, duration), () => _b.DOColor(color, duration));
		}
	}
}