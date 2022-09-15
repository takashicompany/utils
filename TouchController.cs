namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;

	public class TouchController :
		MonoBehaviour,
		IPointerDownHandler,
		IBeginDragHandler,
		IDragHandler,
		IEndDragHandler,
		IPointerUpHandler
	{
		private List<PointerEventData> _pointerEvents = new List<PointerEventData>();

		public delegate void PointerEventDelegate(PointerEventData eventData);

		public event PointerEventDelegate onPointerDownEvent;
		public event PointerEventDelegate onBeginDragEvent;
		public event PointerEventDelegate onDragEvent;
		public event PointerEventDelegate onEndDragEvent;
		public event PointerEventDelegate onPointerUpEvent;

		private Dictionary<int, PointerEventData> _pinchPointers = new Dictionary<int, PointerEventData>();

		

		private bool _virtualMultiTouch;

		[SerializeField]
		private KeyCode _virtualMultiTouchKeyCode = KeyCode.G;

		[SerializeField]
		private float _pinchClearance = 0.1f;

		public delegate void PinchEvent(float offsetDistance);

		private float _pinchDistance;
		public event PinchEvent onPinchEvent;

		private void Update()
		{
			if (_pointerEvents.Count >= 2)
			{
				var a = _pointerEvents[0];
				var b = _pointerEvents[1];
				
				if (_pinchPointers.ContainsKey(a.pointerId) && _pinchPointers.ContainsKey(b.pointerId))
				{
					var currentDistance = Vector2.Distance(a.CalcNormalizedPosition(), b.CalcNormalizedPosition());

					if ((Mathf.Abs(_pinchDistance) > _pinchClearance &&_pinchDistance != 0))
					{
						var offset = currentDistance - _pinchDistance;
						onPinchEvent(offset);
					}

					_pinchDistance = currentDistance;

					// 別に入れ替えなくても良い気はする。というかHashSet<int>でいけるんじゃね？
					_pinchPointers[a.pointerId] = a;
					_pinchPointers[b.pointerId] = b;
				}
				else
				{
					_pinchPointers.Clear();
					_pinchPointers.Add(a.pointerId, a);
					_pinchPointers.Add(b.pointerId, b);
				}
			}
			else
			{
				if (_pinchPointers.Count != 0)
				{
					_pinchPointers.Clear();
					_pinchDistance = 0;
				}
			}

			_virtualMultiTouch = Input.GetKey(_virtualMultiTouchKeyCode);
		}
		

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			AddPointerEvent(eventData);

			onPointerDownEvent?.Invoke(eventData);
		}

		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			AddPointerEvent(eventData);
			onBeginDragEvent?.Invoke(eventData);
		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			AddPointerEvent(eventData);
			onDragEvent?.Invoke(eventData);
		}

		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
			// EndDragはどうぜPointerUpと同じく呼ばれるので、ここでピンチ系の処理は入れない
			onEndDragEvent?.Invoke(eventData);
		}

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			RemovePointerEvent(eventData.pointerId);

			_pinchPointers.Remove(eventData.pointerId);

			var dummyId = GenerateVirtualPointerId(eventData.pointerId);
			RemovePointerEvent(dummyId);
			_pinchPointers.Remove(dummyId);

			onPointerUpEvent?.Invoke(eventData);
		}

		private bool RemovePointerEvent(int pointerId)
		{
			return _pointerEvents.RemoveAll(m => m.pointerId == pointerId) > 0;
		}

		public void AddPointerEvent(PointerEventData pointer)
		{
			RemovePointerEvent(pointer.pointerId);
			_pointerEvents.Add(pointer);

			if (_virtualMultiTouch)
			{
				var dummy = CreateVirtualPointer(pointer);
				RemovePointerEvent(dummy.pointerId);
				_pointerEvents.Add(dummy);
			}
		}

		private PointerEventData CreateVirtualPointer(PointerEventData pointer)
		{
			var dummy = new PointerEventData(EventSystem.current);

			dummy.position = new Vector2(pointer.position.x - (pointer.position.x - Screen.width / 2) * 2, pointer.position.y - (pointer.position.y - Screen.height / 2) * 2);
			dummy.pointerId = GenerateVirtualPointerId(pointer.pointerId);

			return dummy;
		}

		private static int GenerateVirtualPointerId(int pointerId)
		{
			return int.MinValue / 2 + pointerId;
		}
	}
}
