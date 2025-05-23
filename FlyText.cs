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

		public enum AnimationType
		{
			AppearAndUp,
			AppearAndAlpha,
		}

		public class Text
		{
			private FlyText _flyText;
			public TextWrapper instance { get; private set; }
			
			public Text(FlyText flyText, TextWrapper instance)
			{
				_flyText = flyText;
				this.instance = instance;
			}

			public Sequence Appear(Vector2 screenPosition, AnimationType moveType, float duration = 0.35f)
			{
				switch (moveType)
				{
					case AnimationType.AppearAndUp:
						return AppearAndUp(screenPosition, duration);
					case AnimationType.AppearAndAlpha:
						return AppearAndAlpha(screenPosition, duration);
					default:
						throw new System.NotImplementedException();
				}
			}

			public Sequence AppearAndUp(Vector2 screenPosition, float duration = 0.35f)
			{
				
				var localPoint = Utils.ScreenPointToRectTransformPoint(screenPosition, instance.rectTransform.parent as RectTransform, instance.maskableGraphic.canvas);
				instance.rectTransform.anchoredPosition = localPoint;
				var seq = DOTween.Sequence();
				var c = instance.color;
				c.a = 0.5f;
				seq.Append(instance.rectTransform.DOScale(1f, duration).SetEase(Ease.OutBack).From(0));
				seq.Join(instance.rectTransform.DOAnchorPosX(localPoint.x + Random.Range(-50, 50), duration).SetEase(Ease.Linear));
				seq.Join(instance.rectTransform.DOAnchorPosY(localPoint.y + 100, 2f).SetEase(Ease.Linear));
				seq.Join(instance.DOFade(0, 2f).SetEase(Ease.InExpo));
				seq.AppendCallback(() =>
				{
					instance.gameObject.SetActive(false);
				});
				return seq;
			}

			public Sequence AppearAndAlpha(Vector2 screenPosition, float duration = 0.35f)
			{
				var localPoint = Utils.ScreenPointToRectTransformPoint(screenPosition, instance.rectTransform.parent as RectTransform, instance.maskableGraphic.canvas);
				instance.rectTransform.anchoredPosition = localPoint;
				var seq = DOTween.Sequence();

				seq.Append(instance.rectTransform.DOScale(1f, duration).SetEase(Ease.OutBack).From(0));
				seq.Join(instance.rectTransform.DOAnchorPosX(localPoint.x + Random.Range(-50, 50), 0.5f).SetEase(Ease.Linear));
				seq.Join(instance.rectTransform.DOAnchorPosY(localPoint.y + Random.Range(50, 100), 0.5f).SetEase(Ease.Linear));
				seq.AppendInterval(Random.Range(0.3f, 0.7f));
				seq.Append(instance.rectTransform.DOScale(3f, 0.2f));
				seq.Join(instance.DOFade(0, 0.25f).SetEase(Ease.InQuad));
			
				seq.AppendCallback(() =>
				{
					instance.gameObject.SetActive(false);
				});
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

		[SerializeField]
		private ActivePoolingContainer<Image> _imagePool;

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

		public void CollectTextAll()
		{
			_pool.CollectAll();
		}

		public Sequence FlyImage(Image from, Vector3 to, float duration = 0.25f)
		{
			var image = _imagePool.Get();
			image.sprite = from.sprite;
			image.rectTransform.position = from.rectTransform.position;
			image.rectTransform.localScale = from.rectTransform.localScale;
			image.rectTransform.SetAsLastSibling();

			var seq = DOTween.Sequence();
			
			seq.Append(image.rectTransform.DOMove(to, 0.25f));
			seq.AppendCallback(() =>
			{
				image.gameObject.SetActive(false);
			});

			return seq;
		}


	}
}
