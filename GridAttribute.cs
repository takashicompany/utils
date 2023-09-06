namespace takashicompany.Unity
{
	using System;
	using UnityEngine;
	using System.Collections.Generic;

#if UNITY_EDITOR
	using UnityEditor;
#endif

	/// <summary>
	/// SerilaizableDictionaryの要素をグリッド状に表示するためのAttribute。動的にサイズを変えられる。
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class DynamicGridAttribute : PropertyAttribute
	{

	}

	/// <summary>
	/// SerilaizableDictionaryの要素をグリッド状に表示するためのAttribute。サイズは固定。
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class StaticGridAttribute : PropertyAttribute
	{
		public int cols { get; }
		public int rows { get; }

		public StaticGridAttribute(int cols, int rows)
		{
			this.cols = cols;
			this.rows = rows;
		}
	}

#if UNITY_EDITOR

	[CustomPropertyDrawer(typeof(DynamicGridAttribute))]
	public class DynamicGridPropertyDrawer : PropertyDrawer
	{

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			EditorGUI.LabelField(new Rect(position.x, position.y, position.width, 20), label.text);
			position.y += 20;  // ラベルの下に描画するためのオフセット

			var targetObject = property.serializedObject.targetObject;
			var targetType = targetObject.GetType();
			var field = targetType.GetField(property.name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

			if (field != null && field.FieldType == typeof(SerializableDictionary<Vector2Int, bool>))
			{
				SerializableDictionary<Vector2Int, bool> dictionary = field.GetValue(targetObject) as SerializableDictionary<Vector2Int, bool>;

				Vector2Int lengths = dictionary.GetLengths();
				
				Vector2Int newLengths = EditorGUI.Vector2IntField(new Rect(position.x, position.y, position.width, 20), "Grid Dimensions:", lengths);
				position.y += 25;

				// Dimensionsが変更された場合
				if (newLengths != lengths)
				{
					for (int y = 0; y < newLengths.y; y++)
					{
						for (int x = 0; x < newLengths.x; x++)
						{
						
							Vector2Int pos = new Vector2Int(x, y);
							if (!dictionary.ContainsKey(pos))
							{
								dictionary[pos] = false;
							}
						}
					}

					// 新しい範囲外の要素を削除します
					List<Vector2Int> keysToRemove = new List<Vector2Int>();
					foreach (var key in dictionary.Keys)
					{
						if (key.x > newLengths.x || key.y > newLengths.y)
						{
							keysToRemove.Add(key);
						}
					}
					foreach (var key in keysToRemove)
					{
						dictionary.Remove(key);
					}
				}

				for (int y = 0; y < newLengths.y; y++)
				{
					for (int x = 0; x < newLengths.x; x++)
					{
					
						Vector2Int pos = new Vector2Int(x, y);
						bool value = dictionary.ContainsKey(pos) ? dictionary[pos] : false;

						bool newValue = EditorGUI.Toggle(
							new Rect(position.x + x * 20, 
									position.y + (newLengths.y - 1 - y) * 20, 
									20, 
									20), 
							value
						);

						if (value != newValue)
						{
							dictionary[pos] = newValue;
						}
					}
				}

				field.SetValue(targetObject, dictionary);
				property.serializedObject.ApplyModifiedProperties();
			}
			else
			{
				EditorGUI.PropertyField(position, property, label, true);
			}

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var targetObject = property.serializedObject.targetObject;
			var targetType = targetObject.GetType();
			var field = targetType.GetField(property.name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if (field != null && field.FieldType == typeof(SerializableDictionary<Vector2Int, bool>))
			{
				SerializableDictionary<Vector2Int, bool> dictionary = field.GetValue(targetObject) as SerializableDictionary<Vector2Int, bool>;
				Vector2Int maxDimensions = dictionary.GetLengths();
				return (maxDimensions.y + 2) * 20 + 25; // +2 は変数名とGrid Dimensions: のため
			}
			return base.GetPropertyHeight(property, label);
		}
	}

	[CustomPropertyDrawer(typeof(StaticGridAttribute))]
	public class StaticGridPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			var targetObject = property.serializedObject.targetObject;
			var targetType = targetObject.GetType();
			var field = targetType.GetField(property.name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			
			// 直接型チェックを行う
			if (field.FieldType == typeof(SerializableDictionary<Vector2Int, bool>))
			{
				SerializableDictionary<Vector2Int, bool> dictionary = field.GetValue(targetObject) as SerializableDictionary<Vector2Int, bool>;

				var attribute = this.attribute as StaticGridAttribute;
				int cols = attribute.cols;
				int rows = attribute.rows;

				for (int y = 0; y < rows; y++)
				{
					for (int x = 0; x < cols; x++)
					{
						Vector2Int pos = new Vector2Int(x, y);
						bool value = dictionary.ContainsKey(pos) ? dictionary[pos] : false;

						Rect toggleRect = new Rect(position.x + x * 20, position.y + y * 20, 20, 20);
						bool newValue = EditorGUI.Toggle(toggleRect, value);

						if (value != newValue)
						{
							dictionary[pos] = newValue;
						}
					}
				}
			}

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var attribute = this.attribute as StaticGridAttribute;
			return attribute.rows * 20;
		}
	}
	#endif
}