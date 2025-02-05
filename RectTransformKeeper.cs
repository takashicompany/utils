namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using DG.Tweening;

	public class RectTransformKeeper : TransformKeeper
	{
		public new class Record
		{
			[SerializeField]
			private RectTransform _rectTransform;
			public RectTransform rectTransform => _rectTransform;

			[SerializeField]
			private Vector2 _anchoredPosition;
			public Vector2 anchoredPosition => _anchoredPosition;

			[SerializeField]
			private Vector2 _sizeDelta;
			public Vector2 sizeDelta => _sizeDelta;

			public Record(RectTransform rectTransform) : this(rectTransform, rectTransform.anchoredPosition, rectTransform.sizeDelta)
			{

			}

			public Record(RectTransform rectTransform, Vector2 anchoredPosition, Vector2 sizeDelta)
			{
				_rectTransform = rectTransform;
				_anchoredPosition = anchoredPosition;
				_sizeDelta = sizeDelta;
			}

			public void Apply()
			{
				_rectTransform.anchoredPosition = _anchoredPosition;
				_rectTransform.sizeDelta = _sizeDelta;
			}

			public Tweener MoveAnchorPosition(float duration)
			{
				return _rectTransform.DOAnchorPos(_anchoredPosition, duration);
			}

			public Tweener PlaySizeDelta(float duration)
			{
				return _rectTransform.DOSizeDelta(_sizeDelta, duration);
			}
		}
	}

	public static class RectTransformKeeperExtension
	{
		public static RectTransformKeeper.Record GenerateRecord(this RectTransform rectTransform)
		{
			return new RectTransformKeeper.Record(rectTransform);
		}
	}
}
