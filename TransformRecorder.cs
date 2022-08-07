namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class TransformRecorder : MonoBehaviour
	{
		private struct Param
		{
			public float time;
			public Vector3 position;
			public Quaternion rotation;
			public Vector3 scale;

			public static Param Lerp(Param from, Param to, float normalized)
			{
				return new Param()
				{
					time = Mathf.Lerp(from.time, to.time, normalized),
					position = Vector3.Lerp(from.position, to.position, normalized),
					rotation = Quaternion.Lerp(from.rotation, to.rotation, normalized),
					scale = Vector3.Lerp(from.scale, to.scale, normalized)
				};
			}
		}

		private List<Param> _history = new List<Param>();

		private int _lastPlayIndex;

		private void Record(float time)
		{
			var log = new Param()
			{
				time = time,
				position = transform.localPosition,
				rotation = transform.localRotation,
				scale = transform.localScale
			};

			_history.Add(log);
		}

		private void Clear()
		{
			_lastPlayIndex = 0;
			_history.Clear();
		}

		private void StartPlay(bool forceMode)
		{
			_lastPlayIndex = 0;

			if (forceMode)
			{
				if (TryGetComponent<Rigidbody>(out var rigidbody))
				{
					rigidbody.isKinematic = true;
				}

				if (TryGetComponent<Rigidbody2D>(out var rigidbody2D))
				{
					rigidbody2D.isKinematic = true;
				}

				if (TryGetComponent<Animator>(out var animator))
				{
					animator.enabled = false;
				}

				if (TryGetComponent<Animation>(out var animation))
				{
					animation.enabled = false;
				}
			}
		}

		private void Play(float time)
		{
			TryGetParam(time, _lastPlayIndex, out var current, out _lastPlayIndex);
			Apply(current);
		}

		private void Apply(Param p)
		{
			transform.localPosition = p.position;
			transform.localRotation = p.rotation;
			transform.localScale = p.scale;
		}

		private bool TryGetParam(float time, int index, out Param result, out int newIndex)
		{
			index = Mathf.Clamp(index, 0, _history.Count - 1);

			if (_history.Count == 0)
			{
				result = new Param();
				newIndex = -1;
				return false;
			}

			if (_history[0].time > time)
			{
				result = GetFirst();
				newIndex = 0;
				return false;
			}

			if (_history[_history.Count - 1].time < time)
			{
				result = GetLast();
				newIndex = _history.Count;
				return false;
			}

			if (_history[index].time > time)
			{
				index--;

				for (; 1 < index; index--)
				{
					if (_history[index].time < time)
					{
						break;
					}

					// 上のbreakに入らずfor構文が終わった場合はindexが1で且つ、その[1].timeは[0].time ~ [1].timeということになる
				}
			}

			for (;0 < index && index < _history.Count; index++)
			{
				var next = _history[index];
				if (time <= next.time)
				{
					var from = _history[index - 1];

					Debug.Assert(from.time <= time && time < next.time, "保存されている時間が不正です。");

					var 最大 = next.time - time;
					var 現在 = time - from.time;
					var 区間 = Mathf.InverseLerp(0, 最大, 現在);
					var 値0から1 = 0f;
					if (区間 > 0)
					{
						値0から1 = 区間 / 最大;
					}
					
					newIndex = index - 1;

					result = Param.Lerp(from, next, 値0から1);
					return true;
				}
			}

			result = GetLast();
			newIndex = _history.Count - 1;
			return false;

			Param GetFirst()
			{
				return _history[0];
			}

			Param GetLast()
			{
				return _history[_history.Count - 1];
			}
		}

		public class Manager
		{
			private HashSet<TransformRecorder> _recorders = new HashSet<TransformRecorder>();

			public bool AddRecorder(TransformRecorder recorder)
			{
				return _recorders.Add(recorder);
			}

			public int FindRecorders(Transform root, bool includeInactive = false)
			{
				var count = 0;

				foreach (var r in root.GetComponentsInChildren<TransformRecorder>(includeInactive))
				{
					if (AddRecorder(r)) count++;
				}

				return count;
			}

			public void Record(float time)
			{
				foreach (var r in  _recorders)
				{
					r.Record(time);
				}
			}

			public void StartPlay(bool forceMode)
			{
				foreach (var r in _recorders)
				{
					r.StartPlay(forceMode);
				}
			}

			public void Play(float time)
			{
				foreach (var r in _recorders)
				{
					r.Play(time);
				}
			}

			public void ClearHistory()
			{
				foreach (var r in _recorders)
				{
					r.Clear();
				}
			}

			public void Format()
			{
				_recorders.Clear();
			}
		}


	}
}
