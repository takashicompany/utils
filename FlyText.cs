namespace takashicompany.Unity
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

			public Sequence Appear(Vector2 screenPosition, float duration = 0.35f)
			{
				var localPoint = Utils.ScreenPointToRectTransformPoint(screenPosition, instance.rectTransform, instance.maskableGraphic.canvas);
				instance.rectTransform.anchoredPosition = localPoint;
				var seq = DOTween.Sequence();

				seq.Append(instance.rectTransform.DOScale(1f, duration).SetEase(Ease.OutBack).From(0));
				seq.Join(instance.rectTransform.DOAnchorPosY(localPoint.y + 100, 2f).SetEase(Ease.Linear));
				seq.Join(instance.DOFade(0, 2f).SetEase(Ease.InExpo));
				return seq;
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
			instance.color = Color.white;
			instance.rectTransform.SetAsLastSibling();

			return new Text(this, instance);
		}

		public void CollectAll()
		{
			_pool.CollectAll();
		}
	}
}
