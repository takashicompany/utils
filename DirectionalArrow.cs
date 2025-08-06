namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.EventSystems;

	public class DirectionalArrow : TaBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
	{
		[SerializeField, Header("画面の何割以上の移動量で表示するか")]
		private float _normalizedMinDistance = 0.05f;

		[SerializeField]
		private float _anchoredPositionDistance = 200f;

		[SerializeField]
		private bool _hideWhenPointerUp = true;

		[SerializeField]
		private bool _autoInit = true;

		private RectTransform _rectTransform => ReturnOrGet<RectTransform>();

		private bool _isInit;

		private void Start()
		{
			if (_autoInit) Init();
		}

		public void Init()
		{
			if (_isInit) return;
			TouchControlListener.InitializeListener(gameObject, out var controller);
			_isInit = true;
		}

		private void OnDestroy()
		{
			TouchControlListener.DeinitializeListener(gameObject);
		}

		public void Show()
		{
			gameObject.SetActiveIfNot(true);
		}

		public void Hide()
		{
			gameObject.SetActiveIfNot(false);
		}

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			
		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			var normalizedDistance = eventData.CalcNormalizedMovement();

			if (normalizedDistance.magnitude < _normalizedMinDistance)
			{
				Hide();
				return;
			}

			Show();

			var angle = Utils.GetAngle(eventData.pressPosition, eventData.position) * -1;
			// TODO 距離を使う場合は、画面の座標(PointerEventData.position等)と、RectTransformの座標(RectTransform.anchoredPosition等)の違いを考慮する必要がある
			// var distance = Vector2.Distance(eventData.pressPosition, eventData.position);
			
			_rectTransform.anchoredPosition = Vector2.zero;

			var rot = Quaternion.Euler(0, 0, angle);

			_rectTransform.localRotation = rot;

			_rectTransform.anchoredPosition = rot * (Vector2.up * _anchoredPositionDistance);
		}

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			if (_hideWhenPointerUp) Hide();
		}
	}
}
