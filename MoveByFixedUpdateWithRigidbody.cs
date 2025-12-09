namespace takashicompany.Unity
{
	using System.Collections.Generic;
	using UnityEngine;

	[DisallowMultipleComponent]
	public class MoveByFixedUpdateWithRigidbody : MonoBehaviour, IMove
	{
		private enum LoopMode
		{
			Restart,
			Yoyo,
		}

		[SerializeField]
		private Rigidbody _rigidbody;

		[SerializeField]
		private bool _isWorld;

		[SerializeField, Header("最初の点は現在地からスタートしますので設定不要。")]
		private Vector3[] _points;

		[SerializeField, Tooltip("false: 全区間の所要時間 / true: 速度(単位/秒)")]
		private bool _isSpeedBased = false;

		[SerializeField, Tooltip("所要時間または速度")]
		private float _durationOrSpeed = 1f;

		[SerializeField, Tooltip("-1 で無限ループ")]
		private int _loopCount = -1;

		[SerializeField]
		private LoopMode _loopMode = LoopMode.Yoyo;

		private Vector3 _startPosition;
		private readonly List<Vector3> _pathPoints = new List<Vector3>();
		private float _totalLength;
		private float _normalizedPosition;
		private int _direction = 1;
		private int _completedLoops;

		private void Reset()
		{
			if (_rigidbody == null)
			{
				_rigidbody = GetComponent<Rigidbody>();
			}
		}

		private void OnEnable()
		{
			if (_rigidbody == null)
			{
				_rigidbody = GetComponent<Rigidbody>();
			}

			if (_rigidbody == null)
			{
				Debug.LogWarning($"{nameof(MoveByFixedUpdateWithRigidbody)}: Rigidbody が設定されていません。", this);
				enabled = false;
				return;
			}

			_startPosition = _isWorld
				? _rigidbody.position
				: _rigidbody.transform.localPosition;

			BuildPathPoints();
			CalcTotalLength();

			_normalizedPosition = 0f;
			_direction = 1;
			_completedLoops = 0;

			ApplyPosition();
		}

		private void FixedUpdate()
		{
			if (_rigidbody == null)
			{
				return;
			}

			Move(Time.fixedDeltaTime);
		}

		private void Move(float deltaTime)
		{
			if (_pathPoints.Count < 2)
			{
				return;
			}

			float deltaNormalized = 0f;

			if (_isSpeedBased)
			{
				if (_durationOrSpeed <= 0f || _totalLength <= 0f)
				{
					return;
				}

				float distancePerSecond = _durationOrSpeed;
				deltaNormalized = (distancePerSecond * deltaTime / _totalLength) * _direction;
			}
			else
			{
				if (_durationOrSpeed <= 0f)
				{
					return;
				}

				deltaNormalized = (deltaTime / _durationOrSpeed) * _direction;
			}

			_normalizedPosition += deltaNormalized;

			if (_normalizedPosition >= 1f || _normalizedPosition <= 0f)
			{
				HandleLoop();
			}

			_normalizedPosition = Mathf.Clamp01(_normalizedPosition);
			ApplyPosition();
		}

		private void BuildPathPoints()
		{
			_pathPoints.Clear();
			_pathPoints.Add(_startPosition);

			if (_points != null && _points.Length > 0)
			{
				_pathPoints.AddRange(_points);
			}
		}

		private void CalcTotalLength()
		{
			_totalLength = 0f;

			for (int i = 0; i < _pathPoints.Count - 1; i++)
			{
				_totalLength += Vector3.Distance(_pathPoints[i], _pathPoints[i + 1]);
			}
		}

		private void HandleLoop()
		{
			if (_loopCount == 0)
			{
				_normalizedPosition = Mathf.Clamp01(_normalizedPosition);
				enabled = false;
				return;
			}

			if (_loopMode == LoopMode.Restart)
			{
				_normalizedPosition = _direction > 0 ? 0f : 1f;
			}
			else if (_loopMode == LoopMode.Yoyo)
			{
				_direction *= -1;
				_normalizedPosition = Mathf.Clamp01(_normalizedPosition);
			}

			if (_loopCount > 0)
			{
				_completedLoops++;

				if (_completedLoops >= _loopCount)
				{
					_normalizedPosition = Mathf.Clamp01(_normalizedPosition);
					enabled = false;
				}
			}
		}

		private void ApplyPosition()
		{
			if (_rigidbody == null)
			{
				return;
			}

			Vector3 pos = GetPositionOnPath(_normalizedPosition);

			if (_isWorld)
			{
				// ワールド座標のまま MovePosition
				_rigidbody.MovePosition(pos);
			}
			else
			{
				// ローカル座標 → ワールド座標に変換して MovePosition
				Transform t = _rigidbody.transform;
				Vector3 worldPos = t.parent != null
					? t.parent.TransformPoint(pos)
					: pos;

				_rigidbody.MovePosition(worldPos);
			}
		}

		private Vector3 GetPositionOnPath(float t)
		{
			if (_pathPoints.Count == 0)
			{
				if (_rigidbody == null)
				{
					return transform.position;
				}

				return _isWorld
					? _rigidbody.position
					: _rigidbody.transform.localPosition;
			}

			if (_pathPoints.Count == 1 || _totalLength <= 0f)
			{
				return _pathPoints[0];
			}

			float targetDistance = t * _totalLength;
			float accumulated = 0f;

			for (int i = 0; i < _pathPoints.Count - 1; i++)
			{
				Vector3 a = _pathPoints[i];
				Vector3 b = _pathPoints[i + 1];
				float segLength = Vector3.Distance(a, b);

				if (segLength <= 0f)
				{
					continue;
				}

				if (accumulated + segLength >= targetDistance)
				{
					float segT = (targetDistance - accumulated) / segLength;
					return Vector3.Lerp(a, b, segT);
				}

				accumulated += segLength;
			}

			return _pathPoints[_pathPoints.Count - 1];
		}

		private void OnDrawGizmosSelected()
		{
			if (_points == null || _points.Length == 0)
			{
				return;
			}

			Transform refTransform = _rigidbody != null ? _rigidbody.transform : transform;

			Vector3 startPos = _isWorld ? refTransform.position : refTransform.localPosition;
			var list = new List<Vector3> { startPos };
			list.AddRange(_points);

			Gizmos.color = Color.cyan;

			for (int i = 0; i < list.Count; i++)
			{
				Vector3 worldPoint = _isWorld
					? list[i]
					: (refTransform.parent != null ? refTransform.parent.TransformPoint(list[i]) : list[i]);

				Gizmos.DrawWireSphere(worldPoint, 0.1f);
			}

			for (int i = 0; i < list.Count - 1; i++)
			{
				Vector3 worldPoint1 = _isWorld
					? list[i]
					: (refTransform.parent != null ? refTransform.parent.TransformPoint(list[i]) : list[i]);
				Vector3 worldPoint2 = _isWorld
					? list[i + 1]
					: (refTransform.parent != null ? refTransform.parent.TransformPoint(list[i + 1]) : list[i + 1]);

				Gizmos.DrawLine(worldPoint1, worldPoint2);
			}
		}
	}
}
