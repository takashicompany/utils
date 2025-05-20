namespace takashicompany.Unity
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public static class LocalizationExtensions
	{
		public static string GetLabel(this Localization.Language lang)
		{
			switch (lang)
			{
				case Localization.Language.Japanese:
					return "日本語";

				case Localization.Language.English:
					return "English";
			}

			return lang.ToString();
		}
	}

	public class Localization : CSV
	{
		public enum Language
		{
			Japanese = 0,
			English = 1,
		}

		private Dictionary<string, int> _langIndex = new Dictionary<string, int>();
		private Dictionary<string, int> _keyIndex = new Dictionary<string, int>();
		private Dictionary<Language, int> _langIndexEnum = new Dictionary<Language, int>();
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

		/// <summary>
		/// enumのintではないものを使いたい場合に使う
		/// </summary>
		public void SetLanguageIndex(Language language, int index)
		{
			_langIndexEnum[language] = index;
		}

		public string Get(string key, Language lang)
		{
			if (_langIndexEnum.ContainsKey(lang))
			{
				return Get(key, _langIndexEnum[lang]);
			}

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
			PlayerPrefs.Save();
		}

#if UNITY_EDITOR

		private const string csvSeparator = "\t";

		public virtual System.Text.StringBuilder CopyLanguageKeysToClipboard(bool ignoreAlreadyCSV, IEnumerable<Type> enumTypes, IEnumerable<Type> formatTypes)
		{
			System.Text.StringBuilder sb = new();

			if (enumTypes != null)
			{
				var enumNames = new List<string>();

				foreach (var enumType in enumTypes)
				{
					var names = Enum.GetNames(enumType).Select(key => $"{key}{csvSeparator}").ToList(); // 2列目は空白
					enumNames.AddRange(names);
				}

				if (ignoreAlreadyCSV)
				{
					enumNames = enumNames.Where(k => !keys.Contains(k.Split(csvSeparator)[0])).ToList();
				}

				if (enumNames.Count > 0) sb.AppendLine(string.Join("\n", enumNames));
			}

			if (formatTypes != null)
			{
				foreach (var formatType in formatTypes)
				{
					var formatFields = formatType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
						.Where(f => f.FieldType == typeof(string))
						.Select(f => $"{f.Name}\t")
						.ToList();

					var formatProperties = formatType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
						.Where(p => p.PropertyType == typeof(string))
						.Select(p => $"{p.Name}\t")
						.ToList();

					if (ignoreAlreadyCSV)
					{
						formatFields = formatFields.Where(f => !keys.Contains(f.Split(csvSeparator)[0])).ToList();
						formatProperties = formatProperties.Where(p => !keys.Contains(p.Split(csvSeparator)[0])).ToList();
					}

					if (formatFields.Count > 0 || formatProperties.Count > 0) sb.AppendLine(string.Join("\n", formatFields.Concat(formatProperties)));
				}
			}


			// クリップボードにコピー
			UnityEditor.EditorGUIUtility.systemCopyBuffer = sb.ToString();

			Debug.Log("Languageのキー一覧をクリップボードにコピーしました。\n" + sb.ToString());

			return sb;
		}

		private const string menuPath = "翻訳/";
		private const string menuPathReset = menuPath + "言語設定をリセット";
		private const string menuPathSwitchLanguage = menuPath + "言語を切り替え/";
		private const string menuPathJapanese = menuPathSwitchLanguage + "日本語";
		private const string menuPathEnglish = menuPathSwitchLanguage + "英語";

		[UnityEditor.MenuItem(menuPathReset)]
		private static void ResetLanguage()
		{
			PlayerPrefs.DeleteKey(_prefsKey);
			PlayerPrefs.Save();
			Debug.Log("言語設定をリセットしました。");
		}

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
