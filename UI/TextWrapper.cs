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
	
	[System.Serializable]
	public class TextWrapper : ComponentWrapper<MaskableGraphic, Text, TextMeshProUGUI>, ITransform, IGameObject
	{
		public MaskableGraphic maskableGraphic => _component;

		private string _cachedText;

		public string text 
		{
			get
			{
				Init();
				return _a != null ? _a.text : _b.text;
			}

			set
			{
				Init();

				if (_cachedText == value)
				{
					return;
				}

				if (_a != null)
				{
					_a.text = value;
				}
				else
				{
					_b.text = value;
				}

				_cachedText = value;
			}
		}

		public Color color
		{
			get
			{
				Init();
				return _a != null ? _a.color : _b.color;
			}

			set
			{
				Init();

				if (_a != null)
				{
					_a.color = value;
				}
				else
				{
					_b.color = value;
				}
			}
		}

		public Transform transform => _component.transform;
		public GameObject gameObject => _component.gameObject;

		public RectTransform rectTransform => _component.rectTransform;

		public TextWrapper(MaskableGraphic component) : base(component)
		{
			
		}

		public Tweener DOFade(float a, float duration)
		{
			Init();

			if (_a != null)
			{
				return _a.DOFade(a, duration);
			}
			else
			{
				return _b.DOFade(a, duration);
			}
		}

		public void SetAlpha(float a)
		{
			Init();

			if (_a != null)
			{
				var c = _a.color;
				c.a = a;
				_a.color = c;
			}
			else
			{
				var c = _b.color;
				c.a = a;
				_b.color = c;
			}
		}
		
	}
}