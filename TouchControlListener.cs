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
			InitializeListener(gameObject, out _controller);
		}

		void OnDestroy()
		{
			DeinitializeListener(gameObject, _controller);
		}

		public static void InitializeListener(GameObject gameObject, out TouchController controller)
		{
			controller = GameObject.FindObjectOfType<TouchController>();

			if (controller == null)
			{
				return;
			}

			if (gameObject.TryGetComponent<IPointerDownHandler>(out var pointerDownHandler))
			{
				controller.onPointerDownEvent += pointerDownHandler.OnPointerDown;
			}

			if (gameObject.TryGetComponent<IBeginDragHandler>(out var beginDragHandler))
			{
				controller.onBeginDragEvent += beginDragHandler.OnBeginDrag;
			}

			if (gameObject.TryGetComponent<IDragHandler>(out var dragHandler))
			{
				controller.onDragEvent += dragHandler.OnDrag;
			}

			if (gameObject.TryGetComponent<IEndDragHandler>(out var endDragHandler))
			{
				controller.onEndDragEvent += endDragHandler.OnEndDrag;
			}

			if (gameObject.TryGetComponent<IPointerUpHandler>(out var pointerUpHandler))
			{
				controller.onPointerUpEvent += pointerUpHandler.OnPointerUp;
			}
		}

		public static void DeinitializeListener(GameObject gameObject, TouchController controller = null)
		{
			if (controller == null)
			{
				controller = GameObject.FindObjectOfType<TouchController>();

				if (controller == null)
				{
					return;
				}
			}

			if (gameObject.TryGetComponent<IPointerDownHandler>(out var pointerDownHandler))
			{
				controller.onPointerDownEvent -= pointerDownHandler.OnPointerDown;
			}

			if (gameObject.TryGetComponent<IBeginDragHandler>(out var beginDragHandler))
			{
				controller.onBeginDragEvent -= beginDragHandler.OnBeginDrag;
			}

			if (gameObject.TryGetComponent<IDragHandler>(out var dragHandler))
			{
				controller.onDragEvent -= dragHandler.OnDrag;
			}

			if (gameObject.TryGetComponent<IEndDragHandler>(out var endDragHandler))
			{
				controller.onEndDragEvent -= endDragHandler.OnEndDrag;
			}

			if (gameObject.TryGetComponent<IPointerUpHandler>(out var pointerUpHandler))
			{
				controller.onPointerUpEvent -= pointerUpHandler.OnPointerUp;
			}
		}
	}
}
