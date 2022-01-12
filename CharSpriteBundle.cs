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
			var suffix = PowerOf2.suffix.Where(s => !string.IsNullOrEmpty(s)).ToArray();
			_paramList = new Param[10 + suffix.Length];

			for (int i = 0; i < 10; i++)
			{
				var p = new Param(i.ToString()[0], null);

				_paramList[i] = p;
			}

			for (int i = 0; i < suffix.Length; i++)
			{
				var p = new Param(suffix[i][0], null);
				_paramList[i + 10] = p;
			}
		}
	}
}