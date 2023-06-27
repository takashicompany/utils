namespace takashicompany.Unity
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using System.Reflection;

	public static class JsonKey
	{
		public static string[] GetKeys<T>()
		{
			return typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.IsPublic || Attribute.IsDefined(f, typeof(SerializeField))).Select(f => f.Name).ToArray();
		}

		public static void CopyKeys<T>()
		{
			GUIUtility.systemCopyBuffer = string.Join("\t ", GetKeys<T>());
		}
	}
}