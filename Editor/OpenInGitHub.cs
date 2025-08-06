namespace takashicompany.Unity.Editor
{
	using UnityEditor;
	using UnityEngine;
	using System.IO;
	using System.Text.RegularExpressions;

	public static class OpenInGitHub
	{
		[MenuItem("Assets/Open in GitHub", false, 1000)]
		private static void OpenFileInGitHub()
		{
			string filePath = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (string.IsNullOrEmpty(filePath))
				return;

			string rootPath = Application.dataPath.Replace("/Assets", "");
			string gitConfigPath = Path.Combine(rootPath, ".git/config");
			if (!File.Exists(gitConfigPath))
			{
				Debug.LogError("Git config not found.");
				return;
			}

			string gitConfig = File.ReadAllText(gitConfigPath);
			string urlPattern = @"url = (.+)";
			Match match = Regex.Match(gitConfig, urlPattern);
			if (!match.Success)
			{
				Debug.LogError("Git URL not found in config.");
				return;
			}

			string gitUrl = match.Groups[1].Value;
			if (gitUrl.StartsWith("git@"))
			{
				gitUrl = "https://" + gitUrl.Replace("git@", "").Replace(":", "/").Replace(".git", "");
			}

			string relativeFilePath = filePath.Replace("Assets", "blob/develop/Assets");
			string finalUrl = Path.Combine(gitUrl, relativeFilePath).Replace("\\", "/");

			// URLエンコードを適用
			var finalUri = new System.Uri(finalUrl);
			string encodedUrl = finalUri.AbsoluteUri;

			Application.OpenURL(encodedUrl);
		}

		[MenuItem("Assets/Open in Github", true)]
		private static bool OpenFileInGitHubValidation()
		{
			// This function provides a condition to whether show or not the menu item.
			return Selection.activeObject != null && AssetDatabase.Contains(Selection.activeObject);
		}
	}
}
