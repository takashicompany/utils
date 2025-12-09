namespace takashicompany.Unity
{
	using System.Collections.Generic;
	using DG.Tweening;
	using UnityEngine;

	public class MoveByDOTweenWithRigidbody : MonoBehaviour, IMove
	{
		[SerializeField]
		private Rigidbody _rigidbody;

		[SerializeField]
		private bool _isWorld;

		[SerializeField, Header("最初の点は現在地からスタートしますので設定不要。")]
		private Vector3[] _points;

		[SerializeField]
		private PathType _pathType = PathType.CatmullRom;

		[SerializeField]
		private Ease _easeType = Ease.Linear;

		[SerializeField]
		private bool _isSpeedBased = false;

		[SerializeField]
		private float _durationOrSpeed;

		[SerializeField]
		private int _loopCount = -1;

		[SerializeField]
		private LoopType _loopType = LoopType.Yoyo;

		private Vector3 _startPosition;

		private Tween _tween;

		private void Awake()
		{
			if (_rigidbody == null)
			{
				_rigidbody = GetComponent<Rigidbody>();
			}

			_startPosition = _isWorld ? transform.position : transform.localPosition;
		}

		private Vector3[] GetPathPoints(Vector3 startPos)
		{
			var list = new List<Vector3> { startPos };

			if (_points != null)
			{
				list.AddRange(_points);
			}

			return list.ToArray();
		}

		private void OnEnable()
		{
			if (_rigidbody == null)
			{
				Debug.LogError("MoveByDOTweenWithRigidbody: Rigidbody is not assigned.", this);
				return;
			}

			if (_isWorld)
			{
				_rigidbody.MovePosition(_startPosition);
			}
			else
			{
				Vector3 worldPos = transform.parent != null
					? transform.parent.TransformPoint(_startPosition)
					: _startPosition;
				_rigidbody.MovePosition(worldPos);
			}

			var pathPoints = GetPathPoints(_startPosition);

			_tween = _isWorld
				? _rigidbody.DOPath(pathPoints, _durationOrSpeed, _pathType, PathMode.Full3D, 10, Color.cyan)
				: _rigidbody.DOLocalPath(pathPoints, _durationOrSpeed, _pathType, PathMode.Full3D, 10, Color.cyan);

			_tween.SetEase(_easeType).SetSpeedBased(_isSpeedBased).SetLoops(_loopCount, _loopType);
		}

		private void OnDisable()
		{
			_tween?.Kill();
		}

		private void OnDrawGizmosSelected()
		{
			if (_points == null || _points.Length == 0) return;

			Vector3 startPos = _isWorld ? transform.position : transform.localPosition;
			var pathPoints = GetPathPoints(startPos);

			Gizmos.color = Color.cyan;

			foreach (var point in pathPoints)
			{
				Vector3 worldPoint = _isWorld ? point : (transform.parent != null ? transform.parent.TransformPoint(point) : point);
				Gizmos.DrawWireSphere(worldPoint, 0.1f);
			}

			for (int i = 0; i < pathPoints.Length - 1; i++)
			{
				Vector3 worldPoint1 = _isWorld ? pathPoints[i] : (transform.parent != null ? transform.parent.TransformPoint(pathPoints[i]) : pathPoints[i]);
				Vector3 worldPoint2 = _isWorld ? pathPoints[i + 1] : (transform.parent != null ? transform.parent.TransformPoint(pathPoints[i + 1]) : pathPoints[i + 1]);
				Gizmos.DrawLine(worldPoint1, worldPoint2);
			}
		}
	}
}
