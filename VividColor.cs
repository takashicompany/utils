namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public static class VividColor
	{
		public static Color red =>			C(255, 0, 64);
		public static Color green =>		C(32, 192, 64);
		public static Color blue =>			C(64, 128, 255);
		public static Color yellow =>		C(255, 192, 64);
		public static Color pink =>			C(255, 0, 160);
		public static Color orange =>		C(255, 128, 32);
		public static Color lightGreen =>	C(128, 255, 128);
		public static Color lightBlue =>	C(128, 224, 255);
		public static Color purple =>		C(128, 64, 255);
		public static Color navy =>			C(64, 32, 160);

		public static Color[] colors = new Color[]
		{
			red,
			green,
			blue,
			yellow,
			pink,
			lightGreen,
			navy,
			orange,
			purple,
			lightBlue
		};

		private static Color C(byte red, byte green, byte blue)
		{
			return new Color32(red, green, blue, 255);
		}

		public static Color GetVividColor(int index)
		{
			return colors[(index + colors.Length) % colors.Length];
		}
	}
}