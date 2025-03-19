namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class Localization : CSV
	{
		public enum Language
		{
			Japanese = 0,
			English = 1,
		}

		private Dictionary<string, int> _langIndex = new Dictionary<string, int>();
		private Dictionary<string, int> _keyIndex = new Dictionary<string, int>();
		public IEnumerable<string> keys => _keyIndex.Keys;

		private const string _prefsKey = "TC_Localization_Language";

		public Localization(TextAsset csvFile) : base(csvFile)
		{
			var langs = _raw[0].ToArray();

			for (var i = 0; i < langs.Length; i++)
			{
				var lang = langs[i];
				_langIndex[lang] = i;
			}

			for (var i = 0; i < _raw.Count; i++)
			{
				var row = _raw[i];
				var key = row[0];
				_keyIndex[key] = i;
			}
		}

		public string Get(string key, Language lang)
		{
			return Get(key, ((int)lang) + 1);   // 1列目はキーなので+1
		}

		public string Get(string key, string lang)
		{
			if (!_langIndex.ContainsKey(lang))
			{
				Debug.LogError("言語が見つかりません。" + lang);
				return $"Lang:{lang} not found.";
			}

			var langIndex = _langIndex[lang];

			return Get(key, langIndex);
		}

		public string Get(string key, int langIndex)
		{
			if (!_keyIndex.TryGetValue(key, out var keyIndex))
			{
				Debug.LogError("キーが見つかりません。" + key);
				return $"Key:[{key}] not found.";
			}

			return _raw[keyIndex][langIndex];
		}

		public bool HasKey(string key)
		{
			return _keyIndex.ContainsKey(key);
		}

		public static Language GetLanguage()
		{
			var lang = PlayerPrefs.GetInt(_prefsKey, (int)Language.English);
			return (Language)lang;
		}

		public static void SetLanguage(Language lang)
		{
			PlayerPrefs.SetInt(_prefsKey, (int)lang);
		}

#if UNITY_EDITOR
		private const string menuPath = "翻訳/言語を切り替え/";
		private const string menuPathJapanese = menuPath + "日本語";
		private const string menuPathEnglish = menuPath + "英語";

		[UnityEditor.MenuItem(menuPathJapanese, true)]
		private static bool JapaneseValidate()
		{
			UnityEditor.Menu.SetChecked(menuPathJapanese, GetLanguage() == Language.Japanese);
			return true;
		}

		[UnityEditor.MenuItem(menuPathJapanese, false, 1)]
		private static void Japanese()
		{
			SetLanguage(Language.Japanese);
		}

		[UnityEditor.MenuItem(menuPathEnglish, true)]
		private static bool EnglishValidate()
		{
			UnityEditor.Menu.SetChecked(menuPathEnglish, GetLanguage() == Language.English);
			return true;
		}

		[UnityEditor.MenuItem(menuPathEnglish, false, 2)]
		private static void English()
		{
			SetLanguage(Language.English);
		}
#endif
	}
}
