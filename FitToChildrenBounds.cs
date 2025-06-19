namespace takashicompany.Unity
{
	using UnityEngine;
	using TMPro;

#if UNITY_EDITOR
	using UnityEditor;
#endif

	/// <summary>
	/// 子 RectTransform の境界で親を包むシンプルなフィッター。
	/// LayoutGroup が不要で、ピボットがどこにあっても OK。
	/// </summary>
	[ExecuteAlways]
	[RequireComponent(typeof(RectTransform))]
	public class FitToChildrenBounds : MonoBehaviour
	{
		[Tooltip("非アクティブの子も含めるか")]
		[SerializeField] private bool _includeInactive = false;

		[Tooltip("左右・上下に足したい余白")]
		[SerializeField] private Vector2 _padding = Vector2.zero;

		[Tooltip("毎フレーム更新するか（動的 UI 用）")]
		[SerializeField] private bool _everyFrame = false;

		private RectTransform rect;

		void Awake() => rect = transform as RectTransform;
		void OnEnable() => UpdateSize();
		void LateUpdate() { if (_everyFrame) UpdateSize(); }

		/// <summary>子の境界を集計し、自身のサイズ＆位置を更新</summary>
		public void UpdateSize()
		{
			if (rect == null) rect = transform as RectTransform;
			if (rect == null || transform.childCount == 0) return;

			Vector2 min = new(float.MaxValue, float.MaxValue);
			Vector2 max = new(float.MinValue, float.MinValue);
			var worldCorners = new Vector3[4];

			foreach (RectTransform child in transform)
			{
				if (!_includeInactive && !child.gameObject.activeInHierarchy) continue;

				if (child.TryGetComponent(out TextMeshProUGUI tmp))
					tmp.ForceMeshUpdate(); // TextMeshPro はメッシュを最新化

				child.GetWorldCorners(worldCorners);
				for (int i = 0; i < 4; i++)
				{
					Vector2 local = rect.InverseTransformPoint(worldCorners[i]);
					min = Vector2.Min(min, local);
					max = Vector2.Max(max, local);
				}
			}

			min -= _padding;
			max += _padding;
			Vector2 size = max - min;

			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);

			Vector2 pivot = rect.pivot; // (0,0)=左下, (1,1)=右上
			Vector2 newLocalPos = min + Vector2.Scale(size, pivot);
			rect.localPosition = new Vector3(newLocalPos.x, newLocalPos.y, rect.localPosition.z);
		}
	}

#if UNITY_EDITOR
	/// <summary>
	/// FitToChildrenBounds 用のカスタムインスペクター。
	/// UpdateSize をボタンで実行できる。
	/// </summary>
	[CustomEditor(typeof(FitToChildrenBounds))]
	public class FitToChildrenBoundsEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			var fitter = (FitToChildrenBounds)target;
			GUILayout.Space(8);

			if (GUILayout.Button("Update Size Now"))
			{
				Undo.RecordObject(fitter.transform, "FitToChildrenBounds Update Size");
				fitter.UpdateSize();
				EditorUtility.SetDirty(fitter);
			}
		}
	}
#endif
}
