namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using DG.Tweening;

	public class PointerScaleAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField]
		private Vector3 _pointerDownScale = new Vector3(1.25f, 1.25f, 1.25f);

		[SerializeField]
		private float _animationDuration = 0.35f;

		[SerializeField]
		private Ease _easeType = Ease.OutBounce;

		private Vector3 _defaultScale;

		private Tweener _tween;

		private void Awake()
		{
			_defaultScale = transform.localScale;
		}

		private void Start()	// enable用
		{

		}

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			if (!enabled) return;
			transform.localScale = _pointerDownScale;
		}

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			Utils.Kill(ref _tween);
			if (!enabled) return;
			_tween = transform.DOScale(_defaultScale, _animationDuration).SetEase(_easeType);
		}
	}
}
