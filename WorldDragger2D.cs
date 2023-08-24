namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using DG.Tweening;

	/// <summary>
	/// ワールド座標で2次元上のドラッグを実現するコンポーネント。指定の軸を固定することができる。
	/// </summary>
	public class WorldDragger2D : MonoBehaviour,
							IPointerDownHandler,
							IBeginDragHandler,
							IDragHandler,
							IEndDragHandler,
							IPointerUpHandler
	{
		[SerializeField]
		private UnityEngine.Events.UnityEvent<WorldDragger2D> _onRelease;
		public UnityEngine.Events.UnityEvent<WorldDragger2D> onRelease => _onRelease;

		private enum Axis
		{
			X = 0,
			Y,
			Z
		}


		[SerializeField]
		private Axis _lockedAxis = Axis.Y;

		[SerializeField]
		private float _maxDragDistance = 0f;

		private Vector3 _dragOffset;

		public bool isLocked { get; private set; }

		public bool isDragging { get; private set; }

		private Vector3 _pointerDownPosition;

		public void SetLock(bool isLocked)
		{
			this.isLocked = isLocked;
		}

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			if (isLocked)
			{
				return;
			}

			_pointerDownPosition = transform.position;

			isDragging = true;

			var worldPoint = eventData.pointerCurrentRaycast.worldPosition;
			_dragOffset = worldPoint - transform.position;
		}

		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{

		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			if (isLocked)
			{
				return;
			}

			Move(eventData);
		}

		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{

		}

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			isDragging = false;
			if (isLocked) return;
			OnRelease();
		}

		protected virtual void Move(PointerEventData eventData)
		{
			if (eventData.TryGetWorldPositionFromPressCurrentRaycast((int)_lockedAxis, out var worldPoint))
			{
				var p = worldPoint - _dragOffset;
				p[(int)_lockedAxis] = transform.position[(int)_lockedAxis];

				if (_maxDragDistance > 0 && Vector3.Distance(_pointerDownPosition, p) > _maxDragDistance)
				{
					p = _pointerDownPosition + ((p - _pointerDownPosition).normalized * _maxDragDistance);
				}

				transform.position = p;
			}
		}

		protected virtual void OnRelease()
		{
			_onRelease?.Invoke(this);
		}

		public Tweener Back(float duration)
		{
			return transform.DOMove(_pointerDownPosition, duration);
		}
	}
}