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

		protected virtual bool _runUpdate => false;

		protected virtual bool _runLateUpdate => true;

		protected virtual bool _runFixedUpdate => false;

		private void Update()
		{
			if (_runUpdate)
			{
				ProcessKeyDown();
			}
		}

		private void LateUpdate()
		{
			if (_runLateUpdate)
			{
				ProcessKeyDown();
			}
		}

		private void FixedUpdate()
		{
			if (_runFixedUpdate)
			{
				ProcessKeyDown();
			}
		}

		private void ProcessKeyDown()
		{
			if (Input.GetKeyDown(_key))
			{
				OnPressKey();
			}
		}

		protected abstract void OnPressKey();
	}
}
