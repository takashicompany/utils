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

			rectTransform.Clamp(canvasRect, _margin);
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
