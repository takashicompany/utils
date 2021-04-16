namespace TakashiCompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public static class PowerOf2
	{
		public static readonly string[] suffix = new string[]
		{
			"",
			"k",
			"M",
			"G",
			"T",
			"P",
			"E",

		};

		public static string ToPowerOf2Str(this ulong num)
		{
			// ulongの最大値は18,446,744,073,709,551,615

			var str = num.ToString();	// 文字列で変換するの賢くない気がするけど...
			
			var comma = str.Length > 3 ? (str.Length - 1) / 3 : 0;	// str.Lengthが1の時、0除算のエラーが出ちゃうからちょっと力技で。

			var result = str.Length > 3 && num > 2048 ? str.Substring(0, str.Length -  3 * comma) + suffix[comma] : str;

			return result;
		}
	}
}