namespace takashicompany.Unity.UI
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;
	
	[System.Serializable]
	public class TextWrapper
	{
		[SerializeField]
		private MaskableGraphic _obj;

		private Text _uiText;
		private TextMeshPro _tmText;

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

		private void Init()
		{
			if (_isInit)
			{
				return;
			}

			if (_uiText == null && _tmText == null)
			{
				_uiText = _obj.GetComponent<Text>();
				_tmText = _obj.GetComponent<TextMeshPro>();
			}

			
			_isInit = true;
		}

		public bool HasInstance()
		{
			Init();
			return _uiText != null || _tmText != null;
		}
	}
}