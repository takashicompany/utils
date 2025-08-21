namespace takashicompany.Unity.Editor
{

	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	public class ComponentValueCopierWindow : EditorWindow
	{
		private GameObject _fromGO;
		private GameObject _toGO;

		private MonoBehaviour[] _fromComponents = Array.Empty<MonoBehaviour>();
		private MonoBehaviour[] _toComponents = Array.Empty<MonoBehaviour>();

		private int _fromIndex = -1;
		private int _toIndex = -1;

		[MenuItem("TC Utils/コンポーネントの値を別のコンポーネントにコピーする")]
		private static void Open()
		{
			var wnd = GetWindow<ComponentValueCopierWindow>();
			wnd.titleContent = new GUIContent("Value Copier");
			wnd.Show();
		}

		private void OnGUI()
		{
			EditorGUILayout.LabelField("Copy serialized values with same variable names", EditorStyles.boldLabel);
			EditorGUILayout.Space();

			using (new EditorGUI.DisabledScope(EditorApplication.isCompiling))
			{
				DrawObjectPickers();
				EditorGUILayout.Space();
				DrawComponentPickers();
				EditorGUILayout.Space();
				DrawActions();
			}
		}

		private void DrawObjectPickers()
		{
			var newFrom = (GameObject)EditorGUILayout.ObjectField("From GameObject", _fromGO, typeof(GameObject), true);
			if (newFrom != _fromGO)
			{
				_fromGO = newFrom;
				_fromComponents = GetMonoBehaviours(_fromGO);
				_fromIndex = _fromComponents.Length > 0 ? 0 : -1;
			}

			var newTo = (GameObject)EditorGUILayout.ObjectField("To GameObject", _toGO, typeof(GameObject), true);
			if (newTo != _toGO)
			{
				_toGO = newTo;
				_toComponents = GetMonoBehaviours(_toGO);
				_toIndex = _toComponents.Length > 0 ? 0 : -1;
			}
		}

		private void DrawComponentPickers()
		{
			using (new EditorGUI.DisabledScope(_fromComponents.Length == 0))
			{
				_fromIndex = EditorGUILayout.Popup("From Component", _fromIndex, BuildComponentDisplayNames(_fromComponents));
			}

			using (new EditorGUI.DisabledScope(_toComponents.Length == 0))
			{
				_toIndex = EditorGUILayout.Popup("To Component", _toIndex, BuildComponentDisplayNames(_toComponents));
			}

			if (_fromComponents.Length == 0 && _fromGO != null)
			{
				EditorGUILayout.HelpBox("From に MonoBehaviour 継承コンポーネントが見つかりません。", MessageType.Info);
			}
			if (_toComponents.Length == 0 && _toGO != null)
			{
				EditorGUILayout.HelpBox("To に MonoBehaviour 継承コンポーネントが見つかりません。", MessageType.Info);
			}
		}

		private void DrawActions()
		{
			bool canCopy = _fromGO != null && _toGO != null
				&& _fromIndex >= 0 && _fromIndex < _fromComponents.Length
				&& _toIndex >= 0 && _toIndex < _toComponents.Length;

			using (new EditorGUI.DisabledScope(!canCopy))
			{
				if (GUILayout.Button("Copy serialized values"))
				{
					var src = _fromComponents[_fromIndex];
					var dst = _toComponents[_toIndex];
					CopySerializedValues(src, dst);
				}
			}
		}

		private static MonoBehaviour[] GetMonoBehaviours(GameObject go)
		{
			if (go == null) return Array.Empty<MonoBehaviour>();
			return go.GetComponents<MonoBehaviour>();
		}

		private static string[] BuildComponentDisplayNames(MonoBehaviour[] comps)
		{
			if (comps == null || comps.Length == 0) return Array.Empty<string>();

			var names = new List<string>(comps.Length);
			var typeCount = new Dictionary<Type, int>();

			for (int i = 0; i < comps.Length; i++)
			{
				var t = comps[i]?.GetType() ?? typeof(MonoBehaviour);
				if (!typeCount.ContainsKey(t)) typeCount[t] = 0;
				typeCount[t]++;
				int indexOfType = typeCount[t];
				string label = $"{t.Name} #{indexOfType}";
				names.Add(label);
			}
			return names.ToArray();
		}

		private static void CopySerializedValues(MonoBehaviour fromComp, MonoBehaviour toComp)
		{
			if (fromComp == null || toComp == null)
			{
				Debug.LogWarning("コピー元またはコピー先のコンポーネントが無効です。");
				return;
			}

			var srcSO = new SerializedObject(fromComp);
			var dstSO = new SerializedObject(toComp);

			Undo.RecordObject(toComp, "Copy Serialized Values");

			int copied = 0;
			int skipped = 0;

			var iterator = srcSO.GetIterator();
			bool enterChildren = true;
			while (iterator.NextVisible(enterChildren))
			{
				enterChildren = false;

				// スクリプト参照は除外
				if (iterator.propertyPath == "m_Script") continue;

				var dstProp = dstSO.FindProperty(iterator.propertyPath);
				if (dstProp == null)
				{
					skipped++;
					continue;
				}

				if (TryCopyProperty(iterator, dstProp))
					copied++;
				else
					skipped++;
			}

			dstSO.ApplyModifiedProperties();
			EditorUtility.SetDirty(toComp);
			Debug.Log($"[Component Value Copier] コピー完了: {copied} 項目 / スキップ: {skipped} 項目\nFrom: {fromComp.GetType().Name}  To: {toComp.GetType().Name}", toComp);
		}

		/// <summary>
		/// “参照渡し”が可能なものは参照を割り当てる（ObjectReference / ManagedReference）。
		/// それ以外は型一致のうえ値コピー。Genericは再帰で子プロパティをコピー。
		/// </summary>
		private static bool TryCopyProperty(SerializedProperty src, SerializedProperty dst)
		{
			// 型が違えば諦める（例：int と float）
			if (src.propertyType != dst.propertyType)
			{
				// ただし配列／リストはどちらも isArray なら許容して要素ごとコピー
				if (!(src.isArray && dst.isArray)) return false;
			}

			try
			{
				// 参照をそのまま渡せるケース
				if (src.propertyType == SerializedPropertyType.ObjectReference)
				{
					dst.objectReferenceValue = src.objectReferenceValue; // ScriptableObject, Component, Texture など
					return true;
				}
#if UNITY_2021_3_OR_NEWER
				if (src.propertyType == SerializedPropertyType.ManagedReference)
				{
					// 型チェック（同一/互換型のみを想定）
					if (!ManagedRefTypeCompatible(src, dst)) return false;
					dst.managedReferenceValue = src.managedReferenceValue; // [SerializeReference] の参照共有
					return true;
				}
#endif

				// 配列／List<T>
				if (src.isArray && dst.isArray)
				{
					// stringは“配列扱い”でないので除外
					if (src.propertyType == SerializedPropertyType.String) return CopyValue(src, dst);

					dst.arraySize = src.arraySize;
					for (int i = 0; i < src.arraySize; i++)
					{
						var sElem = src.GetArrayElementAtIndex(i);
						var dElem = dst.GetArrayElementAtIndex(i);
						TryCopyProperty(sElem, dElem);
					}
					return true;
				}

				// 通常の値コピー
				if (src.propertyType != SerializedPropertyType.Generic)
				{
					return CopyValue(src, dst);
				}

				// Generic（独自クラス/struct 等）→ 子プロパティを再帰コピー（値コピー）
				var sIter = src.Copy();
				var end = sIter.GetEndProperty();
				bool first = true;
				while (sIter.NextVisible(first) && !SerializedProperty.EqualContents(sIter, end))
				{
					first = false;
					if (sIter.propertyPath == "m_Script") continue;
					var dChild = dst.FindPropertyRelative(sIter.name);
					if (dChild != null)
					{
						TryCopyProperty(sIter, dChild);
					}
				}
				return true;
			}
			catch (Exception e)
			{
				Debug.LogWarning($"プロパティ '{src.propertyPath}' のコピーに失敗: {e.Message}");
				return false;
			}
		}

		private static bool CopyValue(SerializedProperty src, SerializedProperty dst)
		{
			try
			{
				switch (src.propertyType)
				{
					case SerializedPropertyType.Integer: dst.intValue = src.intValue; break;
					case SerializedPropertyType.Boolean: dst.boolValue = src.boolValue; break;
					case SerializedPropertyType.Float: dst.floatValue = src.floatValue; break;
					case SerializedPropertyType.String: dst.stringValue = src.stringValue; break;
					case SerializedPropertyType.Color: dst.colorValue = src.colorValue; break;
					case SerializedPropertyType.ObjectReference: dst.objectReferenceValue = src.objectReferenceValue; break;
					case SerializedPropertyType.LayerMask: dst.intValue = src.intValue; break;
					case SerializedPropertyType.Enum: dst.enumValueIndex = src.enumValueIndex; break;
					case SerializedPropertyType.Vector2: dst.vector2Value = src.vector2Value; break;
					case SerializedPropertyType.Vector3: dst.vector3Value = src.vector3Value; break;
					case SerializedPropertyType.Vector4: dst.vector4Value = src.vector4Value; break;
					case SerializedPropertyType.Rect: dst.rectValue = src.rectValue; break;
					case SerializedPropertyType.AnimationCurve: dst.animationCurveValue = src.animationCurveValue; break;
					case SerializedPropertyType.Bounds: dst.boundsValue = src.boundsValue; break;
					case SerializedPropertyType.Quaternion: dst.quaternionValue = src.quaternionValue; break;
#if UNITY_2021_3_OR_NEWER
					case SerializedPropertyType.ManagedReference:
						dst.managedReferenceValue = src.managedReferenceValue; // 参照共有
						break;
#endif
					default:
						return false;
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

#if UNITY_2021_3_OR_NEWER
		private static bool ManagedRefTypeCompatible(SerializedProperty a, SerializedProperty b)
		{
			// 例: "AssemblyName TypeFullName"
			var aType = a.managedReferenceFullTypename;
			var bType = b.managedReferenceFullTypename;

			// どちらかが未設定(null/empty)なら許容（代入で設定される想定）
			if (string.IsNullOrEmpty(aType) || string.IsNullOrEmpty(bType)) return true;

			// 完全一致のみ許容（安全側）。必要ならここで型互換チェックを拡張。
			return aType == bType;
		}
#endif
	}
}
