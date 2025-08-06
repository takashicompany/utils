namespace takashicompany.Unity.Editor
{
	using UnityEditor;
	using UnityEngine;
	using System.IO;
	using System.Linq;
	using System.Collections.Generic;

	public class OpenErrorFiles : EditorWindow
	{
		[MenuItem("Tools/コンパイルエラーが置きている.csを全て開く")]
		public static void OpenErrorFilesInEditor()
		{
			string logPath = GetEditorLogPath();
			if (string.IsNullOrEmpty(logPath) || !File.Exists(logPath))
			{
				Debug.LogError($"Editor.logが {logPath} に存在しません。");
				return;
			}

			string[] logLines = File.ReadAllLines(logPath);

			// 最新のコンパイル開始キーワードを特定（Unityのバージョンにより異なる可能性がある）
			string[] compileStartMarkers =
			{
			"-----CompilerOutput:-stdout-----",  // 旧バージョン
            "Compilation started",              // Unity 2021+
            "Starting compile",                 // 別のバージョン
            "Begin MonoManager ReloadAssembly"  // リロード開始
        };

			// 直近のコンパイル開始地点を探す
			int lastCompileIndex = -1;
			foreach (var marker in compileStartMarkers)
			{
				int index = logLines.ToList().FindLastIndex(line => line.Contains(marker));
				if (index > lastCompileIndex)
				{
					lastCompileIndex = index;
				}
			}

			if (lastCompileIndex == -1)
			{
				Debug.LogError("No recent compilation marker found in Editor.log. Please check the log manually.");
				return;
			}

			// 最新のコンパイル以降のエラーのみを取得
			var errorLines = logLines.Skip(lastCompileIndex).Where(line => line.Contains("error CS")).ToList();

			HashSet<string> errorFiles = new HashSet<string>();
			foreach (string line in errorLines)
			{
				string filePath = ExtractFilePath(line);
				if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
				{
					errorFiles.Add(filePath);
				}
			}

			// ユニークなエラーのあるファイルだけ開く
			foreach (string file in errorFiles)
			{
				EditorUtility.OpenWithDefaultApp(file);
			}

			Debug.Log($"Opened {errorFiles.Count} files with errors.");
		}

		private static string GetEditorLogPath()
		{
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), @"Unity\Editor\Editor.log");
			}
			else if (Application.platform == RuntimePlatform.OSXEditor)
			{
				return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Library/Logs/Unity/Editor.log");
			}
			return null;
		}

		private static string ExtractFilePath(string logLine)
		{
			int start = logLine.IndexOf("Assets/");
			if (start == -1) return null;

			int end = logLine.IndexOf(".cs", start);
			if (end == -1) return null;

			return Path.Combine(Directory.GetCurrentDirectory(), logLine.Substring(start, end - start + 3));
		}
	}
}
