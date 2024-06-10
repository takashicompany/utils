namespace takashicompany.Unity.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System.IO;
	using System.Collections.Generic;

	public class FolderSpritePacker : EditorWindow
	{
		[MenuItem("TC Utils/選択したフォルダ内のSpriteを1枚のSpriteにまとめる")]
		private static void CombineSelectedSprites()
		{
			var selectedFolder = Selection.activeObject;
			if (selectedFolder == null || !AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(selectedFolder)))
			{
				EditorUtility.DisplayDialog("Error", "No folder selected", "OK");
				return;
			}

			string folderPath = AssetDatabase.GetAssetPath(selectedFolder);
			string parentFolderPath = Directory.GetParent(folderPath).FullName.Replace(Application.dataPath, "Assets");
			string[] fileEntries = Directory.GetFiles(folderPath, "*.png", SearchOption.TopDirectoryOnly);

			List<Texture2D> tempTextures = new List<Texture2D>();
			List<string> textureNames = new List<string>();
			foreach (string filePath in fileEntries)
			{
				string relativePath = filePath.Replace(Application.dataPath, "Assets");
				Texture2D originalTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(relativePath);

				if (originalTexture != null)
				{
					RenderTexture renderTex = RenderTexture.GetTemporary(originalTexture.width, originalTexture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
					Graphics.Blit(originalTexture, renderTex);

					RenderTexture previous = RenderTexture.active;
					RenderTexture.active = renderTex;

					Texture2D readableTexture = new Texture2D(originalTexture.width, originalTexture.height);
					readableTexture.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
					readableTexture.Apply();

					RenderTexture.active = previous;
					RenderTexture.ReleaseTemporary(renderTex);

					tempTextures.Add(readableTexture);
					textureNames.Add(Path.GetFileNameWithoutExtension(filePath)); // テクスチャの名前を保持
				}
			}

			Texture2D newSpriteSheet = new Texture2D(2048, 2048);
			Rect[] rects = newSpriteSheet.PackTextures(tempTextures.ToArray(), 0, 2048);

			List<SpriteMetaData> spriteDataList = new List<SpriteMetaData>();
			for (int i = 0; i < rects.Length; i++)
			{
				SpriteMetaData metaData = new SpriteMetaData
				{
					rect = new Rect(rects[i].x * newSpriteSheet.width, rects[i].y * newSpriteSheet.height, rects[i].width * newSpriteSheet.width, rects[i].height * newSpriteSheet.height),
					name = textureNames[i], // テクスチャの名前を使用
					pivot = new Vector2(0.5f, 0.5f),
					alignment = (int)SpriteAlignment.Custom
				};
				spriteDataList.Add(metaData);
			}

			byte[] bytes = newSpriteSheet.EncodeToPNG();
			string outputPath = parentFolderPath + "/" + selectedFolder.name + "_combined.png";
			File.WriteAllBytes(outputPath, bytes);
			AssetDatabase.Refresh();

			TextureImporter importer = AssetImporter.GetAtPath(outputPath) as TextureImporter;
			if (importer != null)
			{
				importer.textureType = TextureImporterType.Sprite;
				importer.spriteImportMode = SpriteImportMode.Multiple;
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
				importer.spritesheet = spriteDataList.ToArray();
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
				EditorUtility.SetDirty(importer);
				importer.SaveAndReimport();
			}

			EditorUtility.DisplayDialog("Sprites Combined", "Sprite sheet created at " + outputPath, "OK");
		}
	}
}