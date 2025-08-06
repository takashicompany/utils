namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;


	/// <summary>
	/// XXXKickerの抽象クラス。このコンポーネントは取り付けられません。
	/// </summary> <summary>
	public abstract class KeyDownKicker : MonoBehaviour
	{
		[SerializeField, Header("発動するキー。このオブジェクトが無効状態だと入力を受け付けない。")]
		protected KeyCode _key = KeyCode.T;

		public KeyCode key => _key;

		protected virtual bool _managed => true;

		protected virtual bool _runUpdate => false;

		protected virtual bool _runLateUpdate => true;

		protected virtual bool _runFixedUpdate => false;

		private static HashSet<KeyDownKicker> _kickersInternal;

		protected static IReadOnlyCollection<KeyDownKicker> _kickers => _kickersInternal;

		public enum UpdateType
		{
			Update,
			LateUpdate,
			FixedUpdate,
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Init()
		{
			_kickersInternal = new HashSet<KeyDownKicker>();
		}

		protected virtual void Start()
		{
			if (_managed) _kickersInternal.Add(this);
		}

		protected virtual void OnDestroy()
		{
			if (_managed) _kickersInternal.Remove(this);
		}

		protected virtual void Update()
		{
			if (_runUpdate)
			{
				ProcessKeyDown();
			}
		}

		protected virtual void LateUpdate()
		{
			if (_runLateUpdate)
			{
				ProcessKeyDown();
			}
		}

		protected virtual void FixedUpdate()
		{
			if (_runFixedUpdate)
			{
				ProcessKeyDown();
			}
		}

		protected virtual bool ProcessKeyDown()
		{
			if (Input.GetKeyDown(_key))
			{
				OnPressKey();
				return true;
			}

			return false;
		}

		public virtual bool TryEmulateKey(KeyCode key, UpdateType updateType)
		{
			if (_key == key && IsUpdateTiming(updateType))
			{
				OnPressKey();
				return true;
			}

			return false;
		}

		protected abstract void OnPressKey();

		protected bool IsUpdateTiming(UpdateType updateType)
		{
			switch (updateType)
			{
				case UpdateType.Update:
					return _runUpdate;
				case UpdateType.LateUpdate:
					return _runLateUpdate;
				case UpdateType.FixedUpdate:
					return _runFixedUpdate;
				default:
					return false;
			}
		}
	}
}
