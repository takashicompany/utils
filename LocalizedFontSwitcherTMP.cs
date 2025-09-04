namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using TMPro;
	using UnityEngine;

	public abstract class LocalizedFontSwitcher : MonoBehaviour
	{
		protected bool _isInit;
		protected Localization.Language _language;

		protected virtual void OnEnable()
		{
			var current = Localization.GetLanguage();

			if (_isInit && _language != current)
			{
				_language = current;
				UpdateFont();
			}
			else
			{
				_language = current;
				UpdateFont();
				_isInit = true;
			}
		}

		protected virtual void LateUpdate()
		{
			var current = Localization.GetLanguage();

			if (_language != current)
			{
				_language = current;
				UpdateFont();
			}
		}

		protected abstract void UpdateFont();
	}
	
	// TextMeshPro用のLocalizedFontSwitcher。UGUI用でも使えるようにRequireComponentはしない。
	public class LocalizedFontSwitcherTMP : LocalizedFontSwitcher
	{
		[SerializeField]
		private SerializableDictionary<Localization.Language, TMP_FontAsset> _fontMap = new ();

		private TMP_Text _text;

		private void Awake()
		{
			_text = GetComponent<TMP_Text>();
			if (_text == null)
			{
				Debug.LogError($"{nameof(LocalizedFontSwitcherTMP)}はTextMeshProコンポーネントが必要です。 name:{this.name}");
				enabled = false;
				return;
			}
		}

		protected override void UpdateFont()
		{
			if (_fontMap != null && _fontMap.TryGetValue(_language, out var font))
			{
				_text.font = font;
			}
		}
	}
}
