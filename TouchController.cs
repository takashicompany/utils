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
		public delegate void PointerEventDelegate(PointerEventData eventData);

		public event PointerEventDelegate onPointerDownEvent;
		public event PointerEventDelegate onBeginDragEvent;
		public event PointerEventDelegate onDragEvent;
		public event PointerEventDelegate onEndDragEvent;
		public event PointerEventDelegate onPointerUpEvent;
		

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			onPointerDownEvent?.Invoke(eventData);
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			onBeginDragEvent?.Invoke(eventData);
		}

		public void OnDrag(PointerEventData eventData)
		{
			onDragEvent?.Invoke(eventData);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			onEndDragEvent?.Invoke(eventData);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			onPointerUpEvent?.Invoke(eventData);
		}
	}
}
