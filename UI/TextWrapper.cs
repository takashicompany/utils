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
	public class TextWrapper : ITransform, IGameObject
	{
		[SerializeField]
		private MaskableGraphic _obj;

		private Text _uiText;
		private TextMeshProUGUI _tmText;

		private bool _isInit;

		public string text 
		{
			get
			{
				Init();
				return _uiText != null ? _uiText.text : _tmText.text;
			}

			set
			{
				Init();

				if (_uiText != null)
				{
					_uiText.text = value;
				}
				else
				{
					_tmText.text = value;
				}
			}
		}

		public Color color
		{
			get
			{
				Init();
				return _uiText != null ? _uiText.color : _tmText.color;
			}

			set
			{
				Init();

				if (_uiText != null)
				{
					_uiText.color = value;
				}
				else
				{
					_tmText.color = value;
				}
			}
		}

		public Transform transform => _obj.transform;
		public GameObject gameObject => _obj.gameObject;

		private void Init()
		{
			if (_isInit)
			{
				return;
			}

			if (_uiText == null && _tmText == null)
			{
				_uiText = _obj.GetComponent<Text>();
				_tmText = _obj.GetComponent<TextMeshProUGUI>();
			}

			
			_isInit = true;
		}

		public bool HasInstance()
		{
			Init();
			return _uiText != null || _tmText != null;
		}

		public Tweener DOFade(float a, float duration)
		{
			Init();

			if (_uiText != null)
			{
				return _uiText.DOFade(a, duration);
			}
			else
			{
				return _tmText.DOFade(a, duration);
			}
		}

		public void SetAlpha(float a)
		{
			Init();

			if (_uiText != null)
			{
				var c = _uiText.color;
				c.a = a;
				_uiText.color = c;
			}
			else
			{
				var c = _tmText.color;
				c.a = a;
				_tmText.color = c;
			}
		}
	}
}