namespace takashicompany.Unity.UI
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.UI;
	using DG.Tweening;

	public class CoinFlyAnimation : MonoBehaviour
	{
		[SerializeField]
		private takashicompany.Unity.ActivePoolingContainer<MaskableGraphic> _coinPool;

		[SerializeField]
		private float _minScatterDistance = 100;

		[SerializeField]
		private float _maxScatterDistance = 200;

		[SerializeField]
		private RectTransform _flyDestination;

		public void Play(int amount, Vector3 worldPosition, System.Action onComplete = null, Camera camera = null)
		{
			if (camera == null) camera = Camera.main;
			var sp = camera.WorldToScreenPoint(worldPosition);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(_coinPool.container.parent as RectTransform, sp, null, out var localPosition);
			
			for (int i = 0; i < amount; i++)
			{
				var coin = _coinPool.Get();

				coin.transform.localPosition = localPosition;

				var seq = DOTween.Sequence();

				var scatterPosition = takashicompany.Unity.Utils.RandomVector3(-1, 1);
				scatterPosition.z = 0;
				scatterPosition = scatterPosition * Random.Range(_minScatterDistance, _maxScatterDistance);

				seq.Append(coin.transform.DOLocalMove(coin.transform.localPosition + scatterPosition, 0.25f).SetEase(Ease.OutCubic));
				seq.AppendInterval(0.5f + 0.025f * i);
				seq.Append(coin.transform.DOLocalMove(_flyDestination.localPosition, 0.25f).SetEase(Ease.InCubic));
				seq.AppendCallback(() =>
				{
					coin.gameObject.SetActive(false);
				});

				if (i == amount - 1)
				{
					seq.OnComplete(() =>
					{
						onComplete?.Invoke();
					});
				}
			}
		}
	}
}