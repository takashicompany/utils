namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.UI;
#if UNITY_EDITOR
	using UnityEditor;
	using UnityEditor.UI;
#endif

	// https://techblog.kayac.com/unity_advent_calendar_2018_13
	[RequireComponent(typeof(CanvasRenderer))]
	public class DynamicMask : Graphic
	{
		public Vector2[] verts = new Vector2[]
		{
			new Vector2(-100, -100),
			new Vector2(-100, 100),
			new Vector2(100, 100),
			new Vector2(100, -100),
		};

		public Vector3Int[] tris = new Vector3Int[]
		{
			new Vector3Int(0, 1, 2),
			new Vector3Int(2, 3, 0),
		};

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			base.OnPopulateMesh(vh);

			vh.Clear();

			for (int i = 0; i < verts.Length; i++)
			{
				vh.AddVert(verts[i], color, Vector2.zero);
			}

			for (int i = 0; i < tris.Length; i++)
			{
				vh.AddTriangle(tris[i].x, tris[i].y, tris[i].z);
			}
		}

		// 頂点の情報がアニメーションによる変更が行われたときのコールバック
		protected override void OnDidApplyAnimationProperties()
		{
			base.OnDidApplyAnimationProperties();

			SetVerticesDirty();
		}

		[ContextMenu("SetVerticesDirty")]
		private void MenuSetVerticesDirty()
		{
			SetVerticesDirty();
		}
	}
}
