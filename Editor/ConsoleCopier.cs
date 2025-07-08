namespace takashicompany.Unity.Editor
{
	using UnityEditor;
	using UnityEngine;
	using System.Reflection;
	using System.Text;

	public static class ConsoleCopier
	{
		[MenuItem("TC Utils/Consoleに出力されている内容をクリップボードにコピーする")]
		private static void CopyVisibleLogs()
		{
			// ── UnityEditor.LogEntries & LogEntry を取得 ──
			var asm = typeof(EditorWindow).Assembly;
			var logEntriesType = asm.GetType("UnityEditor.LogEntries");
			var logEntryType = asm.GetType("UnityEditor.LogEntry");

			if (logEntriesType == null || logEntryType == null)
			{
				Debug.LogError("LogEntries / LogEntry が見つかりません。Unity バージョンを確認してください。");
				return;
			}

			// ── 必要なメソッド ──
			BindingFlags s = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			var getCount = logEntriesType.GetMethod("GetCount", s);
			var startGetting = logEntriesType.GetMethod("StartGettingEntries", s);
			var getEntryInternal = logEntriesType.GetMethod("GetEntryInternal", s);
			var endGetting = logEntriesType.GetMethod("EndGettingEntries", s);
			var getStatusMask = logEntriesType.GetMethod("GetStatusMask", s);

			// ── 必要なフィールド ──
			BindingFlags f = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			var modeField = logEntryType.GetField("mode", f);
			var messageField = logEntryType.GetField("message", f) ??   // 2021.2 以降
							   logEntryType.GetField("condition", f);   // 旧バージョン

			if (getCount == null || startGetting == null || getEntryInternal == null ||
				endGetting == null || getStatusMask == null ||
				modeField == null || messageField == null)
			{
				Debug.LogError("必要な Unity 内部 API へアクセスできませんでした。");
				return;
			}

			int total = (int)getCount.Invoke(null, null);
			int mask = (int)getStatusMask.Invoke(null, null);   // Console トグル状態

			var sb = new StringBuilder(1024);
			object entryObj = System.Activator.CreateInstance(logEntryType);
			object[] args = { 0, entryObj };

			startGetting.Invoke(null, null);

			var count = 0;
			
			for (int i = 0; i < total; i++)
			{
				args[0] = i;
				getEntryInternal.Invoke(null, args);
				var entry = args[1];

				int entryMode = (int)modeField.GetValue(entry);
				if ((entryMode & mask) == 0) continue;                      // 無効種別はスキップ
				count++;
				string msg = (string)messageField.GetValue(entry);
				sb.AppendLine(msg);
			}

			endGetting.Invoke(null, null);

			EditorGUIUtility.systemCopyBuffer = sb.ToString();
			Debug.Log($"{count}件のログ:{sb.Length}文字をコピーしました。");
		}
	}
}
