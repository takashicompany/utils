namespace TakashiCompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public static class PowerOf2
	{
		static PowerOf2()
		{
			var p2s = new ulong[65];

			ulong n = 1;

			for (int i = 0; i < p2s.Length; i++)
			{
				if (i == 0)
				{
					p2s[i] = 0;
					continue;
				}

				// 2 ^ 64は18,446,744,073,709,551,616なのだけど、
				// オーバーフローで0に戻ってしまうので、最大値は18,446,744,073,709,551,615とする(-1した値を最大値とする)
				if (i == p2s.Length - 1)
				{
					var m = n - 1;

					n = m + n;
				}
				else
				{
					n *= 2;
				}

				p2s[i] = n;
			}

			pow2s = p2s;
		}

		public static ulong[] pow2s { get; private set; }

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

		/// <summary>
		/// 2048系のゲームで使われる色を列挙
		/// </summary>
		public static readonly Color[] colors = new Color[]
		{
			new Color32(255, 232, 18, 255),
			new Color32(15, 184, 217, 255),
			new Color32(159, 202, 0, 255),
			new Color32(200, 14, 104, 255),
			new Color32(38, 96, 172, 255),
			new Color32(234, 84, 25, 255),
			new Color32(0, 132, 66, 255),
			new Color32(240, 138, 55, 255),
			new Color32(0, 145, 248, 255),
			new Color32(103, 0, 204, 255)
		};

		public static Color GetColor(int pow)
		{
			return colors[(pow + colors.Length - 1) % colors.Length];
		}

		/// <summary>
		/// そのまま色を入れるとくすむケースがあるらしいので、エミッションを設定すると良いらしい
		/// </summary>
		public static Color GetEmmisionColor(int pow)
		{
			var color = GetColor(pow);

			Vector3 HSV = Vector3.zero;
			Color.RGBToHSV(color, out HSV.x, out HSV.y, out HSV.z);
			return Color.HSVToRGB(HSV.x, HSV.y, 1, false);
		}
		
	}
}