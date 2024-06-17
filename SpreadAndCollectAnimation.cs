namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.UI;
	using DG.Tweening;

	public class SpreadAndCollectAnimation : MonoBehaviour
	{
		[SerializeField]
		private ActivePoolingContainer<Image> _container;

		public Sequence Play(Sprite sprite, Vector2 fromScreenPosition, int amount, float minSpreadDistance, float maxSpreadDistance, Vector3 worldDestination, Vector2? imageSize = default)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(_container.container as RectTransform, fromScreenPosition, null, out var localPosition);

			var seq = DOTween.Sequence();

			var list = new List<Image>();

			for (int i = 0; i < amount; i++)
			{
				var image = _container.Get();
				list.Add(image);

				image.sprite = sprite;

				if (imageSize.HasValue)
				{
					image.rectTransform.sizeDelta = imageSize.Value;
				}

				image.transform.localPosition = localPosition;

				Vector3 scatterPosition =  Random.insideUnitCircle.normalized * Random.Range(minSpreadDistance, maxSpreadDistance);

				var tween = image.transform.DOLocalMove(image.transform.localPosition + scatterPosition, 0.25f).SetEase(Ease.OutCubic);

				if (i == 0)
				{
					seq.Append(tween);
				}
				else
				{
					seq.Join(tween);
				}
			}

			seq.AppendInterval(0.5f);

			
			foreach (var image in list)
			{
				var tween = image.transform.DOMove(worldDestination, 0.25f).SetEase(Ease.InCubic).OnComplete(() =>
				{
					image.gameObject.SetActive(false);
				});
				
				if (image == list.First())
				{
					seq.Append(tween);
				}
				else
				{
					seq.Join(tween);
				}
			}

			// for (int i = 0; i < amount; i++)
			// {
			// 	var image = _container.Get();

			// 	image.sprite = sprite;

			// 	if (imageSize.HasValue)
			// 	{
			// 		image.rectTransform.sizeDelta = imageSize.Value;
			// 	}

			// 	image.transform.localPosition = localPosition;

			// 	Vector3 scatterPosition =  Random.insideUnitCircle.normalized * Random.Range(minSpreadDistance, maxSpreadDistance);

			// 	seq.Append(image.transform.DOLocalMove(image.transform.localPosition + scatterPosition, 0.25f).SetEase(Ease.OutCubic));
			// 	seq.AppendInterval(0.5f + 0.025f * i);
			// 	seq.Append(image.transform.DOMove(worldDestination, 0.25f).SetEase(Ease.InCubic));
			// 	seq.AppendCallback(() =>
			// 	{
			// 		image.gameObject.SetActive(false);
			// 	});
			//}

			return seq;
		}
	}
}
