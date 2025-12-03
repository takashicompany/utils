namespace takashicompany.Unity
{
	using DG.Tweening;
	using UnityEngine;

	public class RotateByDOTweenWithRigidbody : MonoBehaviour
	{
		[SerializeField]
		private Rigidbody _rigidbody;

		[SerializeField, Header("回転量（度）")]
		private Vector3 _rotation;

		[SerializeField]
		private float _duration;

		[SerializeField]
		private float _delay = 0f;

		[SerializeField, Header("true: ローカル軸 / false: ワールド軸")]
		private bool _isLocalAxis = true;

		[SerializeField]
		private Ease _easeType = Ease.Linear;

		[SerializeField]
		private int _loopCount = -1;

		[SerializeField]
		private LoopType _loopType = LoopType.Restart;

		private Tween _tween;

		private void Awake()
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
				Debug.LogError("RotateByDOTweenWithRigidbody: Rigidbody is not assigned.", this);
				return;
			}

			var rotateMode = _isLocalAxis ? RotateMode.LocalAxisAdd : RotateMode.WorldAxisAdd;

			_tween = _rigidbody.DORotate(_rotation, _duration, rotateMode)
				.SetDelay(_delay)
				.SetEase(_easeType)
				.SetLoops(_loopCount, _loopType);
		}

		private void OnDisable()
		{
			_tween?.Kill();
		}
	}
}
