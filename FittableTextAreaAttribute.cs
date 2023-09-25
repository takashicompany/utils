namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

#if UNITY_EDITOR
	using UnityEditor;
#endif
	public class FittableTextAreaAttribute : PropertyAttribute
	{
		public string columnSeparate;
		public string rowSeparete;

		public FittableTextAreaAttribute()
		{

		}

		public FittableTextAreaAttribute(string columnSeparate)
		{
			this.columnSeparate = columnSeparate;
		}

		public FittableTextAreaAttribute(string columnSeparate, string rowSeparete) : this(columnSeparate)
		{
			this.rowSeparete = rowSeparete;
		}

	}

#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(FittableTextAreaAttribute))]
	public class FittableTextAreaDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType == SerializedPropertyType.String && Selection.objects.Length == 1)	// なぜかコンポーネントを複数選択すると上書きされてしまうことがあったので、選択しているオブジェクトが1つの時だけ有効にする。
			{
				var attribute = (FittableTextAreaAttribute)base.attribute;

				EditorGUI.BeginProperty(position, label, property);

				// Calculate the height required for the text area based on the content
				GUIContent content = new GUIContent(property.stringValue);
				float textHeight = GUI.skin.textArea.CalcHeight(content, position.width);

				// Create a rect for the label above the text area
				Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

				// Create a rect for the text area below the label
				Rect textAreaRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, textHeight);

				var newLabel = new GUIContent(label);
				// newLabel.text += " Length: " + property.stringValue.Length.ToString() + " ";

				if (!string.IsNullOrEmpty(attribute.columnSeparate))
				{
					string[,] map;

					if (!string.IsNullOrEmpty(attribute.rowSeparete))
					{
						map = property.stringValue.Get2dMap(attribute.columnSeparate, attribute.rowSeparete);
					}
					else
					{
						map = property.stringValue.Get2dMap(attribute.columnSeparate);
					}

					newLabel.text += "(" + map.GetLength(0) + ", " + map.GetLength(1) + ")";
				}

				// Draw the label and text area
				EditorGUI.LabelField(labelRect, newLabel);
				property.stringValue = EditorGUI.TextArea(textAreaRect, property.stringValue);

				EditorGUI.EndProperty();
			}
			else
			{
				EditorGUI.LabelField(position, label.text, "Use ResizeTextArea with strings or select only one object.");
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (property.propertyType == SerializedPropertyType.String)
			{
				GUIContent content = new GUIContent(property.stringValue);
				float textHeight = GUI.skin.textArea.CalcHeight(content, EditorGUIUtility.currentViewWidth);
				return textHeight + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			}
			else
			{
				return EditorGUIUtility.singleLineHeight;
			}
		}
	}
#endif
}