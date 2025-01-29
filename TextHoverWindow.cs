namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.UI;

	public abstract class HoverWindow : MonoBehaviour
	{
		private RectTransform _internalRectTransform;
		public RectTransform rectTransform => _internalRectTransform ?? (_internalRectTransform = GetComponent<RectTransform>());

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
