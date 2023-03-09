namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class CoroutineStarter : MonoBehaviour
	{
		private static CoroutineStarter _instance;

		static CoroutineStarter()
		{
			var go = new GameObject("CoroutineStarter");
			_instance = go.AddComponent<CoroutineStarter>();
			GameObject.DontDestroyOnLoad(go);
		}

		public static Coroutine Start(IEnumerator task)
		{
			return _instance.StartCoroutine(task);
		}

		public static void Stop(Coroutine coroutine)
		{
			_instance.StopCoroutine(coroutine);
		}

		public static void Stop(IEnumerator task)
		{
			_instance.StopCoroutine(task);
		}
	}
}