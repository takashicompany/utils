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

		private TMP_FontAsset _defaultFont;

		private TMP_Text _text;
		
		private Material _originalMaterial;
		private Dictionary<TMP_FontAsset, Material> _materialCache = new ();

		private void Awake()
		{
			_text = GetComponent<TMP_Text>();
			if (_text == null)
			{
				Debug.LogError($"{nameof(LocalizedFontSwitcherTMP)}はTextMeshProコンポーネントが必要です。 name:{this.name}");
				enabled = false;
				return;
			}

			_defaultFont = _text.font;
			_originalMaterial = _text.fontSharedMaterial;
		}

		protected override void UpdateFont()
		{
			TMP_FontAsset targetFont = null;
			
			if (_fontMap != null && _fontMap.TryGetValue(_language, out var font))
			{
				targetFont = font;
			}
			else
			{
				targetFont = _defaultFont;
			}
			
			if (_text.font != targetFont)
			{
				_text.font = targetFont;
				
				// フォント変更時にマテリアルを設定
				if (_originalMaterial != null)
				{
					if (!_materialCache.TryGetValue(targetFont, out var material))
					{
						// 初回の場合、マテリアルを複製してキャッシュ
						material = new Material(_originalMaterial);
						_materialCache[targetFont] = material;
					}
					
					_text.fontSharedMaterial = material;
				}
			}
		}
		
		private void OnDestroy()
		{
			// 作成したマテリアルを破棄
			foreach (var kvp in _materialCache)
			{
				if (kvp.Value != null)
				{
					Destroy(kvp.Value);
				}
			}
			_materialCache.Clear();
		}
	}
}
