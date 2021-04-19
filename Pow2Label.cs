namespace TakashiCompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class Pow2Label : MonoBehaviour
	{
		[SerializeField]
		private SpriteLabel _spriteLabel;

		[SerializeField]
		private ulong _number;

		[ContextMenu("Update Label")]
		public void UpdateLabel()
		{
			var str = _number.ToPowerOf2Str();
			_spriteLabel.UpdateImages(str);
		}

		private void UpdateLabelBySendMessage(int count)
		{
			ulong n = 2;

			for (int i = 0; i < count; i++)
			{
				n *= 2;
			}
			var str = n.ToPowerOf2Str();
			_spriteLabel.UpdateImages(str);
		}
	}
}