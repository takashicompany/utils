namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class ActiveChangeKicker : MonoBehaviour
	{
		[SerializeField, Header("発動するキー。このオブジェクトが無効状態だと入力を受け付けない。")]
		private KeyCode _key = KeyCode.A;

		[SerializeField, Header("切り替えたい対象")]
		private GameObject _target;

		private enum ChangeType
		{
			[InspectorName("有効にする")]
			ToActive,
			
			[InspectorName("無効にする")]
			ToEnactive,

			[InspectorName("有効なら無効にして、無効なら有効にする")]
			ToToggle
		}

		[SerializeField, Header("変更方式")]
		private ChangeType _changeType = ChangeType.ToToggle;

		private void LateUpdate()
		{
			if (Input.GetKeyDown(_key))
			{
				if (_target == null)
				{
					Debug.LogError(name + "のtargetが設定されていませんぞ。");
					return;
				}

				switch (_changeType)
				{
					case ChangeType.ToActive:
						_target.SetActive(true);
						break;
					case ChangeType.ToEnactive:
						_target.SetActive(false);
						break;
					case ChangeType.ToToggle:
						_target.SetActive(!_target.activeSelf);
						break;
				}
			}
		}
	}
}
