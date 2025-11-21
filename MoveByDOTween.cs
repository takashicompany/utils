namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
    using DG.Tweening;
    using UnityEngine;

	public class MoveByDOTween : MonoBehaviour
	{
		[SerializeField]
		private bool _isWorld;

		[SerializeField,Header("最初の点は現在地からスタートしますので設定不要。")]
		private Vector3 [] _points;

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
			_startPosition = _isWorld ? transform.position : transform.localPosition;
		}

		private void OnEnable()
		{
			if (_isWorld)
			{
				transform.position = _startPosition;
			}
			else
			{
				transform.localPosition = _startPosition;
			}

			var list = new List<Vector3>
			{
				_startPosition
			};

			list.AddRange(_points);

			_tween = _isWorld ?
				transform.DOPath(list.ToArray(), _durationOrSpeed, _pathType, PathMode.Full3D, 10, Color.cyan) :
				transform.DOLocalPath(list.ToArray(), _durationOrSpeed, _pathType, PathMode.Full3D, 10, Color.cyan);

			_tween.SetEase(_easeType).SetSpeedBased(_isSpeedBased).SetLoops(_loopCount, _loopType);
		}

		private void OnDisable()
		{
			_tween.Kill();
		}
	}
}