namespace takashicompany.Unity/*.Editor*/ // Editorネームスペースをつけたいところではあるが、Editorフォルダに入れてしまうとビルド時にコンパイルエラーになるのでコメントアウト。
{
	using UnityEngine;
	using System;
	using System.Linq;
	using System.Collections.Generic;

#if UNITY_EDITOR
	using System.Reflection;
	using UnityEditor;
#endif

	/// <summary>
	/// 第一引数にEnumを取る関数に取り付ける。エディターからEnumの値を引数として関数を実行できるようになる。
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class MethodWithEnumDebuggerAttribute : Attribute
	{
		// 現在、この属性はマーカーとして機能するだけです。
		// 特別なロジックは必要ありません。
	}


#if UNITY_EDITOR

	public class MethodWithEnumDebuggerWindow : EditorWindow
	{
		private Dictionary<string, int> methodSelections = new Dictionary<string, int>();

		[MenuItem("TC Utils/MethodWithEnumDebugger属性がついた引数を持つ関数を実行する")]
		public static void ShowWindow()
		{
			GetWindow<MethodWithEnumDebuggerWindow>("MethodWithEnumDebuggerWindow");
		}

		private void OnGUI()
		{
			GUILayout.Label("Class and Methods", EditorStyles.boldLabel);

			BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
			foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
			{
				foreach (var method in type.GetMethods(bindingFlags))
				{
					var attributes = method.GetCustomAttributes(typeof(MethodWithEnumDebuggerAttribute), false);
					if (attributes.Length > 0)
					{
						var parameters = method.GetParameters();
						if (parameters.Length == 1 && parameters[0].ParameterType.IsEnum)
						{
							Type enumType = parameters[0].ParameterType;
							string[] enumNames = Enum.GetNames(enumType);
							string methodKey = $"{type.FullName}.{method.Name}";

							if (!methodSelections.ContainsKey(methodKey))
							{
								methodSelections[methodKey] = 0;
							}

							GUILayout.BeginHorizontal();
							GUILayout.Label($"{type.Name}.{method.Name}");
							methodSelections[methodKey] = EditorGUILayout.Popup(methodSelections[methodKey], enumNames);

							if (GUILayout.Button("実行"))
							{
								ExecuteMethod(type, method, methodSelections[methodKey], enumType);
							}

							GUILayout.EndHorizontal();
						}
					}
				}
			}
		}

		private void ExecuteMethod(Type type, MethodInfo method, int selectedIndex, Type enumType)
		{
			// Enum の値を取得
			var enumValue = Enum.GetValues(enumType).GetValue(selectedIndex);

			// シーン内のすべてのオブジェクトを検索し、対象の型を持つものを見つける
			foreach (var obj in FindObjectsOfType(type))
			{
				// メソッドが引数を取る場合は、enum の値を渡す
				if (method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == enumType)
				{
					method.Invoke(obj, new object[] { enumValue });
				}
				else
				{
					// 引数がない場合は引数なしで実行
					method.Invoke(obj, null);
				}
			}
		}
	}

#endif

}
