namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEditor;

#if UNITY_EDITOR
	public static class EditorUtils
	{
		public static string GetOpenedPrefab()
		{
			var prefabStage = UnityEditor.
#if UNITY_2021_3_OR_NEWER
								SceneManagement
#else
								Experimental.SceneManagement
#endif
								.PrefabStageUtility.GetCurrentPrefabStage();

			if (prefabStage == null)
			{
				return null;
			}

			return prefabStage.assetPath;
		}

		public static T GetEditingPrefab<T>() where T : Object
		{
			var prefabStage = UnityEditor.
#if UNITY_2021_3_OR_NEWER
								SceneManagement
#else
								Experimental.SceneManagement
#endif
								.PrefabStageUtility.GetCurrentPrefabStage();

			if (prefabStage == null)
			{
				return default(T);
			}

			var asset = AssetDatabase.LoadAssetAtPath<T>(prefabStage.assetPath);

			return asset;
		}

		public static bool TryGetEditingPrefab<T>(out T prefab) where T : Object
		{
			prefab = GetEditingPrefab<T>();

			return prefab != null;
		}

		public static void EditorNotificationGameView(string message)
		{
			var assembly = typeof(UnityEditor.EditorWindow).Assembly;
			var type = assembly.GetType("UnityEditor.GameView");
			UnityEditor.EditorWindow.GetWindow(type).ShowNotification(new GUIContent(message), 5);
		}

		public static void EditorNotificationSceneView(string message)
		{
			var assembly = typeof(UnityEditor.EditorWindow).Assembly;
			var type = assembly.GetType("UnityEditor.SceneView");
			UnityEditor.EditorWindow.GetWindow(type).ShowNotification(new GUIContent(message), 5);
		}

		public static void EditorNotificationConsoleWindow(string message)
		{
			var assembly = typeof(UnityEditor.EditorWindow).Assembly;
			var type = assembly.GetType("UnityEditor.ConsoleWindow");
			UnityEditor.EditorWindow.GetWindow(type).ShowNotification(new GUIContent(message), 5);
		}

		public static void EditorNotificationAll(string message)
		{
			EditorNotificationGameView(message);
			EditorNotificationSceneView(message);
			EditorNotificationConsoleWindow(message);
		}

		public static void EditorNotificationGameAndSceneView(string message)
		{
			EditorNotificationGameView(message);
			EditorNotificationSceneView(message);
		}

		/// <summary>
		/// リストの要素Indexを返す
		/// </summary>
		public static int GetArrayElementIndex(this SerializedProperty property)
		{
			// プロパティがリストのインデックスであれば、パスは(変数名).Array.data[(インデックス)] 
			// となるため、この文字列からインデックスを取得する

			// リストの要素であるか判定する
			var match = System.Text.RegularExpressions.Regex.Match(property.propertyPath, "^([a-zA-Z0-9_]*).Array.data\\[([0-9]*)\\]$");
			
			if (!match.Success)
			{
				return -1;
			}

			// Indexを抜き出す
			var splitPath = property.propertyPath.Split('.');
			var regax = new System.Text.RegularExpressions.Regex(@"[^0-9]");
			var indexText = regax.Replace(splitPath[splitPath.Length - 1], "");
			int index = 0;
			
			if (!int.TryParse(indexText, out index))
			{
				return -1;
			}

			return index;
		}
	}
#endif
}