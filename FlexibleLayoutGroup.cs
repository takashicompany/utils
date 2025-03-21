namespace takashicompany.Unity
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections.Generic;

	/// <summary>
	/// 子要素の元々のサイズを尊重しつつ、横方向に並べてスペースが足りなければ改行するレイアウトグループ。ChatGPTでつくった
	/// </summary>
	[AddComponentMenu("Layout/Flexible Grid Layout Group")]
	public class FlexibleGridLayoutGroup : LayoutGroup
	{
		[SerializeField]
		private float spacingX = 0f;  // 横方向のスペース
		[SerializeField]
		private float spacingY = 0f;  // 縦方向のスペース

		/// <summary>
		/// レイアウト計算結果を保持するクラス
		/// </summary>
		private class RowInfo
		{
			public float rowWidth;
			public float rowHeight;
			public List<RectTransform> elements = new List<RectTransform>();
		}

		// レイアウト結果の行リストをキャッシュしておき、
		// CalculateLayoutInputHorizontal/Vertical と SetLayoutHorizontal/Vertical の双方で使う
		private List<RowInfo> _rows = new List<RowInfo>();
		private float _totalPreferredWidth;
		private float _totalPreferredHeight;

		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();

			// レイアウト計算を一度行い、必要な幅を算出
			CalculateFlexibleLayout();

			// 横軸に対して必要な最小サイズ/推奨サイズを設定 (とりあえず同値として扱う例)
			float minWidth = padding.horizontal;
			float preferredWidth = _totalPreferredWidth + padding.horizontal;

			SetLayoutInputForAxis(minWidth, preferredWidth, -1, 0);
		}

		public override void CalculateLayoutInputVertical()
		{
			// 縦軸に対して必要な最小サイズ/推奨サイズを設定
			// このタイミングでも _rows の情報を使う
			float minHeight = padding.vertical;
			float preferredHeight = _totalPreferredHeight + padding.vertical;

			SetLayoutInputForAxis(minHeight, preferredHeight, -1, 1);
		}

		public override void SetLayoutHorizontal()
		{
			// 実際に子要素を横軸に配置する
			SetChildrenAlongAxis(0);
		}

		public override void SetLayoutVertical()
		{
			// 実際に子要素を縦軸に配置する
			SetChildrenAlongAxis(1);
		}

		/// <summary>
		/// 子要素の配置計算を行い、_rows に行ごとの情報を格納する。
		/// </summary>
		private void CalculateFlexibleLayout()
		{
			_rows.Clear();

			// コンテナ(自身のRectTransform)が実際に使える横幅
			float parentWidth = rectTransform.rect.width;
			float currentX = 0f;

			// 新しい行を作る
			RowInfo currentRow = new RowInfo
			{
				rowWidth = 0f,
				rowHeight = 0f
			};
			_rows.Add(currentRow);

			// 子要素を順番に並べていく
			for (int i = 0; i < rectChildren.Count; i++)
			{
				RectTransform child = rectChildren[i];
				if (child == null) continue;

				// 子要素の希望する幅/高さ
				float childWidth = LayoutUtility.GetPreferredWidth(child);
				float childHeight = LayoutUtility.GetPreferredHeight(child);

				// 次の要素を同じ行に入れられるかチェック
				// (currentX + child幅 + 余白) が (利用可能幅 - padding右) を超えるなら改行
				float neededWidth = (currentX == 0f ? 0f : spacingX) + childWidth;
				float availableWidth = parentWidth - padding.left - padding.right;

				// 改行判定
				if (currentX + neededWidth > availableWidth && currentRow.elements.Count > 0)
				{
					// 行を確定して次の行へ
					currentRow.rowWidth = currentX;
					// その行の高さは最大の childHeight で更新されている
					currentX = 0f;

					// 新しい行を作る
					currentRow = new RowInfo
					{
						rowWidth = 0f,
						rowHeight = 0f
					};
					_rows.Add(currentRow);
				}

				// 要素を現在の行に追加
				currentRow.elements.Add(child);
				// 横幅、行の高さを更新
				if (currentX > 0f) currentX += spacingX;
				currentX += childWidth;
				currentRow.rowHeight = Mathf.Max(currentRow.rowHeight, childHeight);
			}

			// 最後の行にも幅を記録
			if (currentRow != null)
			{
				currentRow.rowWidth = currentX;
			}

			// 全体の幅・高さを計算
			_totalPreferredWidth = 0f;
			_totalPreferredHeight = 0f;

			// 幅は最大行幅、高さは各行の合計
			float maxRowWidth = 0f;
			float sumRowHeights = 0f;

			foreach (var row in _rows)
			{
				maxRowWidth = Mathf.Max(maxRowWidth, row.rowWidth);
				sumRowHeights += row.rowHeight;
			}

			_totalPreferredWidth = maxRowWidth;
			_totalPreferredHeight = sumRowHeights + spacingY * (_rows.Count - 1);
		}

		/// <summary>
		/// 実際に子要素を配置する
		/// </summary>
		private void SetChildrenAlongAxis(int axis)
		{
			// CalculateLayoutInputHorizontal / Vertical ですでに行情報が作られている前提
			float startX = padding.left;
			float startY = padding.top;

			float xPos = 0f;
			float yPos = 0f;

			for (int r = 0; r < _rows.Count; r++)
			{
				RowInfo row = _rows[r];
				xPos = startX;

				// 行内の要素を配置
				for (int i = 0; i < row.elements.Count; i++)
				{
					RectTransform child = row.elements[i];

					// 子要素のサイズ(レイアウト計算済みの preferred サイズ)
					float childWidth = LayoutUtility.GetPreferredWidth(child);
					float childHeight = LayoutUtility.GetPreferredHeight(child);

					// 配置先
					// pivot や alignment に応じて調整してもOK。ここでは左上基準で配置する例
					SetChildAlongAxis(child, 0, xPos, childWidth);
					SetChildAlongAxis(child, 1, startY + yPos, childHeight);

					xPos += childWidth + spacingX;
				}

				// 次の行に進む準備(縦方向に row.rowHeight + spacingY)
				yPos += row.rowHeight + spacingY;
			}
		}
	}
}
