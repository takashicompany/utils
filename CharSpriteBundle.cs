namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[CreateAssetMenu(fileName = "SpriteBundle", menuName = "Sprite Bundle")]
	public class CharSpriteBundle : ObjectBundle<char, Sprite>
	{
		[ContextMenu("Setup Number Bundle")]
		private void NumberBundle()
		{
			_dict = new SerializableDictionary<char, Sprite>();

			for (int i = 0; i < 10; i++)
			{
				_dict.TryAdd(i.ToString()[0], null);
			}

			var suffix = PowerOf2.suffix.Where(s => !string.IsNullOrEmpty(s)).ToArray();

			for (int i = 0; i < suffix.Length; i++)
			{
				_dict.TryAdd(suffix[i][0], null);
			}
		}
	}
}