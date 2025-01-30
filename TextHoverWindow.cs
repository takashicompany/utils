namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.UI;

	public abstract class HoverWindow : MonoBehaviour
	{
		public RectTransform rectTransform => (RectTransform)transform;

		private GameObject _caller;

		public void Display(GameObject caller, Vector3 worldPosition, bool force = false)
		{
			if (!force && _caller != null)
			{
				return;
			}

			_caller = caller;
			gameObject.SetActive(true);
			rectTransform.position = worldPosition;
		}

		public void Hide(GameObject caller, bool force = false)
		{
			if (!force && _caller != caller)
			{
				return;
			}

			_caller = null;
			gameObject.SetActive(false);
		}

		private Vector2 _margin = Vector2.one * 50;

		public void AutoPlace()
		{
			// 画面からはみ出ないように位置を調整する
			
			var canvas = GetComponentInParent<Canvas>();
			var canvasRect = canvas.GetComponent<RectTransform>();

			var ap = rectTransform.anchoredPosition;
			var size = rectTransform.sizeDelta;

			if (ap.x < 0)
			{
				ap.x += size.x / 2;
			}
			else
			{
				ap.x -= size.x / 2;
			}

			if (ap.y < 0)
			{
				ap.y += size.y / 2;
			}
			else
			{
				ap.y -= size.y / 2;
			}

			rectTransform.anchoredPosition = ap;

			var left = rectTransform.anchoredPosition.x - rectTransform.sizeDelta.x / 2;

			if (left < -canvasRect.sizeDelta.x / 2 + _margin.x)
			{
				rectTransform.anchoredPosition += new Vector2(-canvasRect.sizeDelta.x / 2 - left + _margin.x, 0);
			}

			var right = rectTransform.anchoredPosition.x + rectTransform.sizeDelta.x / 2;

			if (right > canvasRect.sizeDelta.x / 2 - _margin.x)
			{
				rectTransform.anchoredPosition -= new Vector2(right - canvasRect.sizeDelta.x / 2 - _margin.x, 0);
			}

			var top = rectTransform.anchoredPosition.y + rectTransform.sizeDelta.y / 2;

			if (top > canvasRect.sizeDelta.y / 2 - _margin.y)
			{
				rectTransform.anchoredPosition -= new Vector2(0, top - canvasRect.sizeDelta.y / 2 - _margin.y);
			}

			var bottom = rectTransform.anchoredPosition.y - rectTransform.sizeDelta.y / 2;

			if (bottom < -canvasRect.sizeDelta.y / 2 + _margin.y)
			{
				rectTransform.anchoredPosition += new Vector2(0, -canvasRect.sizeDelta.y / 2 - bottom + _margin.y);
			}
		}
	}

	public class TextHoverWindow : HoverWindow
	{
		[SerializeField]
		private TextWrapper _text;

		public void SetText(string text)
		{
			_text.text = text;
			LayoutRebuilder.ForceRebuildLayoutImmediate(_text.rectTransform);
			LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
		}
	}
}
