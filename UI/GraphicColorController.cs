namespace takashicompany.Unity.UI
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	// 似たクラスが既にありそうな気もするけど...
	[ExecuteAlways]
	public class GraphicColorController : MonoBehaviour
	{
		[SerializeField, Header("一括で色を変えたいUI要素")]
		private UnityEngine.UI.Graphic[] _targets;

		[SerializeField]
		private Color _color = Color.black;

		private void Update()
		{
			if (!Application.isPlaying)
			{
				UpdateColor();
			}
		}

		public void SetColor(Color color)
		{
			_color = color;
			UpdateColor();
		}

		public void UpdateColor()
		{
			if (_targets == null) return;

			foreach (var g in _targets)
			{
				if (g == null) continue;
				g.color = _color;
			}
		}

		[ContextMenu("Find self and children")]
		public void FindSelfAndChildren()
		{
			_targets = GetComponentsInChildren<UnityEngine.UI.Graphic>(true);
		}
	}
}