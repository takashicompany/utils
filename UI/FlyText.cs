namespace takashicompany.Unity.UI
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using DG.Tweening;
	using UnityEngine;
	using UnityEngine.UI;

	public class FlyText : MonoBehaviour
	{
		[SerializeField]
		private TextWrapper _textPrefab;

		private TextWrapper.PoolingContainer _pool;

		public class Text
		{
			private FlyText _flyText;
			public TextWrapper instance { get; private set; }
			
			public Text(FlyText flyText, TextWrapper instance)
			{
				_flyText = flyText;
				this.instance = instance;
			}

			public Tweener Appear(Vector2 screenPosition, float duration = 0.35f)
			{
				var localPoint = ScreenPointToRectTransformPoint(screenPosition, instance.rectTransform, instance.maskableGraphic.canvas);
				instance.rectTransform.anchoredPosition = localPoint;
				return instance.rectTransform.DOScale(1f, duration).SetEase(Ease.OutBack).From(0);
			}

			public Sequence MoveAndDisappear(Vector3 worldPosition, float duration = 0.25f)
			{
				var seq = DOTween.Sequence();
				seq.Append(instance.rectTransform.DOMove(worldPosition, duration).SetEase(Ease.InOutQuad));
				seq.Join(instance.rectTransform.DOScale(0, duration).SetEase(Ease.InOutQuad));
				return seq;
			}
		}

		private void Awake()
		{
			_pool = new TextWrapper.PoolingContainer(_textPrefab, transform);
		}

		public Text GetInstance(string message = null)
		{
			var instance = _pool.Get();
			instance.text = message;

			return new Text(this, instance);
		}
		
		// ToastMessage.csからコピペしてきた
		private static Vector2 ScreenPointToRectTransformPoint(Vector2 screenPoint, RectTransform rectTransform, Canvas canvas)
		{
			Vector2 localPoint;

			switch (canvas.renderMode)
			{
				case RenderMode.ScreenSpaceOverlay:
				case RenderMode.ScreenSpaceCamera:
					RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, canvas.worldCamera, out localPoint);
					break;
				case RenderMode.WorldSpace:
					Vector3 worldPoint = canvas.worldCamera.ScreenToWorldPoint(screenPoint);
					rectTransform.InverseTransformPoint(worldPoint);
					localPoint = new Vector2(worldPoint.x, worldPoint.y);
					break;
				default:
					localPoint = Vector2.zero;
					break;
			}

			return localPoint;
		}
	}
}
