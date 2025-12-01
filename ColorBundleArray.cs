namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[CreateAssetMenu(fileName = "ColorBundleArray", menuName = "takashicompany/ColorBundle/ColorBundleArray", order = 1)]
	public class ColorBundleArray : ColorBundle
	{
		[System.Serializable]
		private class Array
		{
			[SerializeField]
			private List<Color32> _colors = new ();
			public IReadOnlyList<Color32> colors => _colors;
		}

		[SerializeField]
		private List<Array> _colorArrays = new ();

		public IReadOnlyList<Color32> GetColorArray(int index)
		{
			index = index % _colorArrays.Count;
			return _colorArrays[index].colors;
		}
	}
}