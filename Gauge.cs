namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using takashicompany.Unity;
	using DG.Tweening;

	public class Gauge : MonoBehaviour
	{
		[SerializeField, Header("Pivotは左にしてください。")]
		private Image _bar;

		private Tweener _tweener;

		public void Appear()
		{
			transform.localScale = Vector3.one;
			gameObject.SetActive(true);
		}

		public Tweener Appear(float duration)
		{
			Appear();

			transform.localScale = Vector3.zero;

			transform.DOKill();

			return transform.DOScale(1f, duration);
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}

		public void Hide(float duration)
		{
			transform.DOKill();

			transform.DOScale(0f, duration).OnComplete(() =>
			{
				Hide();
			});
		}

		public void UpdateBar(int current, int max)
		{
			var r = Mathf.Clamp01((float)current / (float)max);
			_bar.rectTransform.SimpleHorizontalGauge(r);
		}

		public void UpdateBar(uint current, uint max)
		{
			var r = Mathf.Clamp01((float)current / (float)max);
			_bar.rectTransform.SimpleHorizontalGauge(r);
		}

		public void UpdateBar(double current, double max)
		{
			var r = Mathf.Clamp01((float)(current / max));
			_bar.rectTransform.SimpleHorizontalGauge(r);
		}

		public void UpdateBarColor(Color color)
		{
			_bar.color = color;
		}

		/// <summary>
		/// バーをアニメーションさせる。OnCompleteは予約済み
		/// </summary>
		public Tweener PlayBar(int from, int to, int max, float duration)
		{
			Utils.Kill(ref _tweener);

			var current = from;

			_tweener = DOTween.To(() => current, v => current = v, to, duration).OnUpdate(() =>
			{
				UpdateBar(current, max);
			});

			return _tweener;
		}

		public void SetColor(Color color)
		{
			_bar.color = color;
		}

		public Tweener PlayColor(Color from, Color to, float duration)
		{
			SetColor(from);
			_bar.DOKill();
			return _bar.DOColor(to, duration);
		}
	}
}
