namespace takashicompany.Unity.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System.IO;

	public class RenameFilesInFolder : EditorWindow
	{
		[MenuItem("Tools/選択中のフォルダ内のファイルを連番でリネーム")]
		private static void RenameFiles()
		{
			if (Selection.activeObject == null)
			{
				Debug.LogWarning("フォルダを選択してください。");
				return;
			}

			string folderPath = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (!AssetDatabase.IsValidFolder(folderPath))
			{
				Debug.LogWarning("選択されたのがフォルダではありません。");
				return;
			}

			string[] guids = AssetDatabase.FindAssets("t:Object", new[] { folderPath });
			int index = 1;

			foreach (string guid in guids)
			{
				string assetPath = AssetDatabase.GUIDToAssetPath(guid);
				string extension = Path.GetExtension(assetPath);

				if (extension == ".meta")
					continue;

				string newFileName = string.Format("{0:D4}{1}", index, extension);
				string newAssetPath = Path.Combine(Path.GetDirectoryName(assetPath), newFileName).Replace("\\", "/");

				if (AssetDatabase.RenameAsset(assetPath, newFileName) != "")
				{
					Debug.LogError($"リネームに失敗しました: {assetPath}");
					continue;
				}

				index++;
			}

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			Debug.Log("リネームが完了しました。");
		}
	}
}
