namespace takashicompany.Unity.Editor
{
	using UnityEngine;
	using UnityEditor;
	using TMPro;

	public class TMP_SpriteAssetEditor : MonoBehaviour
	{
		[MenuItem("TC Utils/TMP Sprite AssetのメトリクスのBXを0、BYをHの0.8倍に設定する")]
		private static void UpdateSpriteAssetMetrics()
		{
			// TMP Sprite Assetを選択したオブジェクトから取得
			TMP_SpriteAsset spriteAsset = Selection.activeObject as TMP_SpriteAsset;

			if (spriteAsset != null)
			{
				// SpriteGlyphのリストをループ
				foreach (var glyph in spriteAsset.spriteGlyphTable)
				{
					// GlyphMetricsを取得
					var metrics = glyph.metrics;

					// BXを0に設定
					metrics.horizontalBearingX = 0;

					// BYをHの0.8倍に設定
					metrics.horizontalBearingY = metrics.height * 0.8f;

					// 更新したGlyphMetricsを設定
					glyph.metrics = metrics;
				}

				// 変更を保存
				EditorUtility.SetDirty(spriteAsset);
				AssetDatabase.SaveAssets();

				Debug.Log("TMP Sprite Assetのメトリクスを更新しました。");
			}
			else
			{
				Debug.Log("TMP Sprite Assetが選択されていません。");
			}
		}
	}

}