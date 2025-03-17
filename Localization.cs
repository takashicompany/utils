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
			return Get(key, ((int)lang)+ 1);	// 1列目はキーなので+1
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
	}
}
