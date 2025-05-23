namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// 仮想の平面を用意して、その平面上でのタッチイベントを取得する
	/// </summary>
	public class TouchToPlane :
		MonoBehaviour,
		IPointerEnterHandler,
		IPointerMoveHandler,
		IPointerExitHandler,
		IPointerDownHandler,
		IBeginDragHandler,
		IDragHandler,
		IEndDragHandler,
		IPointerUpHandler,
		IPointerClickHandler
	{

		public class EventData
		{
			public PointerEventData pointerEvent;

			/// <summary>
			/// タッチ開始時のスクリーン座標を平面座標に変換したもの。カメラの位置がタッチ開始時と変わっていると座標はズレる。
			/// </summary>
			public Vector3 pressWorldPosition;

			/// <summary>
			/// 現在のタッチのスクリーン座標を平面座標に変換したもの。
			/// </summary>
			public Vector3 worldPosition;

			public Vector3 GetWorldOffset()
			{
				return worldPosition - pressWorldPosition;
			}
		}

		[Header("スクリーン座標からRayを生成するカメラ。未指定の場合はCamera.mainを使う")]
		public new Camera camera;

		public int vector3Index = 1;

		public float targetPoint = 0;

		public delegate void EventDelegate(EventData eventData);

		public event EventDelegate onPointerEnter;
		public event EventDelegate onPointerMove;
		public event EventDelegate onPointerExit;
		public event EventDelegate onPointerDown;
		public event EventDelegate onBeginDrag;
		public event EventDelegate onDrag;
		public event EventDelegate onEndDrag;
		public event EventDelegate onPointerUp;
		public event EventDelegate onPointerClick;

		void IPointerEnterHandler.OnPointerEnter(PointerEventData pointerEvent)
		{
			if (TryCreateEvent(pointerEvent, out var eventData))
			{
				onPointerEnter?.Invoke(eventData);
			}
		}

		void IPointerMoveHandler.OnPointerMove(PointerEventData pointerEvent)
		{
			if (TryCreateEvent(pointerEvent, out var eventData))
			{
				onPointerMove?.Invoke(eventData);
			}
		}

		void IPointerExitHandler.OnPointerExit(PointerEventData pointerEvent)
		{
			if (TryCreateEvent(pointerEvent, out var eventData))
			{
				onPointerExit?.Invoke(eventData);
			}
		}

		void IPointerDownHandler.OnPointerDown(PointerEventData pointerEvent)
		{
			if (TryCreateEvent(pointerEvent, out var eventData))
			{
				onPointerDown?.Invoke(eventData);
			}
		}

		void IBeginDragHandler.OnBeginDrag(PointerEventData pointerEvent)
		{
			if (TryCreateEvent(pointerEvent, out var eventData))
			{
				onBeginDrag?.Invoke(eventData);
			}
		}

		void IDragHandler.OnDrag(PointerEventData pointerEvent)
		{
			if (TryCreateEvent(pointerEvent, out var eventData))
			{
				onDrag?.Invoke(eventData);
			}
		}

		void IEndDragHandler.OnEndDrag(PointerEventData pointerEvent)
		{
			if (TryCreateEvent(pointerEvent, out var eventData))
			{
				onEndDrag?.Invoke(eventData);
			}
		}

		void IPointerUpHandler.OnPointerUp(PointerEventData pointerEvent)
		{
			if (TryCreateEvent(pointerEvent, out var eventData))
			{
				onPointerUp?.Invoke(eventData);
			}
		}

		void IPointerClickHandler.OnPointerClick(PointerEventData pointerEvent)
		{
			if (TryCreateEvent(pointerEvent, out var eventData))
			{
				onPointerClick?.Invoke(eventData);
			}
		}

		private bool TryCreateEvent(PointerEventData pointerEvent, out EventData eventData)
		{
			var camera = this.camera;

			if (camera == null) camera = Camera.main;

			if (
				camera.ScreenPointToRay(pointerEvent.pressPosition).TryGetPositionOnRay(vector3Index, targetPoint, out var pressWorldPosition) &&
				camera.ScreenPointToRay(pointerEvent.position).TryGetPositionOnRay(vector3Index, targetPoint, out var worldPosition)
			)
			{
				eventData = new EventData();
				eventData.pointerEvent = pointerEvent;
				eventData.pressWorldPosition = pressWorldPosition;
				eventData.worldPosition = worldPosition;

				return true;
			}

			eventData = null;
			return false;
		}
	}
}
