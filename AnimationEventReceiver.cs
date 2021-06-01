namespace TakashiCompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// AnimationEventを他のオブジェクトに伝達するコンポーネント
	/// </summary>
	[RequireComponent(typeof(Animator))]
	public class AnimationEventReceiver : MonoBehaviour
	{
		[SerializeField]
		private UnityEvent<float?, int?, string, Object> _callback;

		public event System.Action<float?, int?, string, Object> onEvent;

		private void OnEvent()
		{
			CallEvent();
		}

		private void OnEvent(float v)
		{
			CallEvent(f: v);
		}

		private void OnEvent(int v)
		{
			CallEvent(i: v);
		}

		private void OnEvent(string str)
		{
			CallEvent(str: str);
		}

		private void OnEvent(Object obj)
		{
			CallEvent(obj: obj);
		}

		private void CallEvent(float? f = null, int? i = null, string str = null, Object obj = null)
		{
			_callback?.Invoke(f, i, str, obj);
			onEvent?.Invoke(f, i, str, obj);
		}
	}
}