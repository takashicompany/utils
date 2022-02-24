namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;

	public class TouchControlListener : MonoBehaviour
	{
		private TouchController _controller;

		void Start()
		{
			_controller = GameObject.FindObjectOfType<TouchController>();

			if (_controller == null)
			{
				return;
			}

			if (TryGetComponent<IPointerDownHandler>(out var pointerDownHandler))
			{
				_controller.onPointerDownEvent += pointerDownHandler.OnPointerDown;
			}

			if (TryGetComponent<IBeginDragHandler>(out var beginDragHandler))
			{
				_controller.onBeginDragEvent += beginDragHandler.OnBeginDrag;
			}

			if (TryGetComponent<IDragHandler>(out var dragHandler))
			{
				_controller.onDragEvent += dragHandler.OnDrag;
			}

			if (TryGetComponent<IEndDragHandler>(out var endDragHandler))
			{
				_controller.onEndDragEvent += endDragHandler.OnEndDrag;
			}

			if (TryGetComponent<IPointerUpHandler>(out var pointerUpHandler))
			{
				_controller.onPointerUpEvent += pointerUpHandler.OnPointerUp;
			}
		}

		void OnDestroy()
		{
			if (_controller == null)
			{
				return;
			}

			if (TryGetComponent<IPointerDownHandler>(out var pointerDownHandler))
			{
				_controller.onPointerDownEvent -= pointerDownHandler.OnPointerDown;
			}

			if (TryGetComponent<IBeginDragHandler>(out var beginDragHandler))
			{
				_controller.onBeginDragEvent -= beginDragHandler.OnBeginDrag;
			}

			if (TryGetComponent<IDragHandler>(out var dragHandler))
			{
				_controller.onDragEvent -= dragHandler.OnDrag;
			}

			if (TryGetComponent<IEndDragHandler>(out var endDragHandler))
			{
				_controller.onEndDragEvent -= endDragHandler.OnEndDrag;
			}

			if (TryGetComponent<IPointerUpHandler>(out var pointerUpHandler))
			{
				_controller.onPointerUpEvent -= pointerUpHandler.OnPointerUp;
			}
		}
	}
}
