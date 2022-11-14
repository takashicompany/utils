namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using DG.Tweening;

	public class Spinner : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		private Tweener _tweener;

		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			Utils.Kill(ref _tweener);
		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			transform.Rotate(new Vector2(eventData.delta.y, -eventData.delta.x), Space.World);
		}

		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
			Utils.Kill(ref _tweener);
			_tweener = transform.DORotate(new Vector2(eventData.delta.y, -eventData.delta.x) * eventData.delta.magnitude * 10, eventData.delta.magnitude * 0.1f).SetEase(Ease.OutSine).SetRelative(true);
		}

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			Utils.Kill(ref _tweener);
		}
	}
}