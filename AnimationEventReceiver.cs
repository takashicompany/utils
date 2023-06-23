namespace takashicompany.Unity
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
		private UnityEvent _callback;

		public event System.Action onEvent;

		private void OnEvent()
		{
			_callback?.Invoke();
			onEvent?.Invoke();
		}
	}

	/// <summary>
	/// パラメーター付きのAnimationEventを他のオブジェクトに伝達するコンポーネント
	/// どうもオーバーロードした関数を用意すると、引数なしの関数が優先されるみたいなので、個別で作る感じにする
	/// </summary>
	[RequireComponent(typeof(Animator))]
	public abstract class AnimationEventReceiver<T> : MonoBehaviour
	{
		[SerializeField]
		private UnityEvent<T> _callback;

		public event System.Action<T> onEvent;

		private void OnEvent(T arg)
		{
			_callback?.Invoke(arg);
			onEvent?.Invoke(arg);
		}
	}
}