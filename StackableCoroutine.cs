namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class StackableCoroutine
	{
		private MonoBehaviour _kicker;

		private Queue<IEnumerator> _queue = new Queue<IEnumerator>();

		public StackableCoroutine(MonoBehaviour kicker)
		{
			_kicker = kicker;

			_kicker.StartCoroutine(Co());

			IEnumerator Co()
			{
				while (_kicker.gameObject.activeSelf)
				{
					if (_queue.Count > 0)
					{
						var coroutine = _queue.Dequeue();

						yield return _kicker.StartCoroutine(coroutine);
					}

					yield return null;
				}
			}
		}

		public void Enqueue(IEnumerator coroutine)
		{
			_queue.Enqueue(coroutine);
		}
	}
}