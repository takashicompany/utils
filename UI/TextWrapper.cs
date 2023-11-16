namespace takashicompany.Unity.UI
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;
	using takashicompany.Unity;
	using DG.Tweening;
	using DG.Tweening.Core;
	using DG.Tweening.Plugins.Options;
	
	[System.Serializable]
	public class TextWrapper : ComponentWrapper<Text, TextMeshProUGUI>, ITransform, IGameObject
	{
		private string _cachedText;

		public string text
		{
			get => Get(() => _a.text, () => _b.text);
			set => Set(v => _a.text = v, v => _b.text = v, value);
		}

		public Color color
		{
			get => Get(() => _a.color, () => _b.color);
			set => Set(v => _a.color = v, v => _b.color = v, value);
		}

		public RectTransform rectTransform => Get(() => _a.rectTransform, () => _b.rectTransform);

		public TextWrapper(MaskableGraphic component) : base(component)
		{
			
		}

		public TweenerCore<Color, Color, ColorOptions> DOFade(float a, float duration)
		{
			return Get(() => _a.DOFade(a, duration), () => _b.DOFade(a, duration));
		}

		public TweenerCore<Vector2, Vector2, VectorOptions> DOAnchorPos(Vector2 endValue, float duration)
		{
			return Get(() => _a.rectTransform.DOAnchorPos(endValue, duration), () => _b.rectTransform.DOAnchorPos(endValue, duration));
		}

		public void SetAlpha(float a)
		{
			Set(v => _a.color = new Color(_a.color.r, _a.color.g, _a.color.b, v), v => _b.color = new Color(_b.color.r, _b.color.g, _b.color.b, v), a);
		}
	}
}