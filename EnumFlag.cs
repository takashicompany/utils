namespace takashicompany.Unity
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif
	
	/// <summary>
	/// この属性をつけるEnumは2進数記法で値を定義すると良い
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class EnumFlagAttribute : PropertyAttribute
	{

	}

#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(EnumFlagAttribute))]
	public class EnumFlagDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var e = (Enum)(Enum.ToObject(fieldInfo.FieldType, property.intValue));
			var f = EditorGUI.EnumFlagsField(position, label, e);
			property.intValue = Convert.ToInt32(f);
		}
	}
#endif
}