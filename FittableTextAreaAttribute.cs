namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	
# if UNITY_EDITOR
	using UnityEditor;
#endif
	public class FittableTextAreaAttribute : PropertyAttribute
	{
		
	}

#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(FittableTextAreaAttribute))]
	public class FittableTextAreaDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType == SerializedPropertyType.String)
			{
				EditorGUI.BeginProperty(position, label, property);

				// Calculate the height required for the text area based on the content
				GUIContent content = new GUIContent(property.stringValue);
				float textHeight = GUI.skin.textArea.CalcHeight(content, position.width);

				// Create a rect for the text area
				Rect textAreaRect = new Rect(position.x, position.y, position.width, textHeight);

				// Draw the text area
				property.stringValue = EditorGUI.TextArea(textAreaRect, property.stringValue);

				EditorGUI.EndProperty();
			}
			else
			{
				EditorGUI.LabelField(position, label.text, "Use ResizeTextArea with strings.");
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (property.propertyType == SerializedPropertyType.String)
			{
				GUIContent content = new GUIContent(property.stringValue);
				float textHeight = GUI.skin.textArea.CalcHeight(content, EditorGUIUtility.currentViewWidth);
				return textHeight + EditorGUIUtility.standardVerticalSpacing;
			}
			else
			{
				return EditorGUIUtility.singleLineHeight;
			}
		}
	}
#endif
}