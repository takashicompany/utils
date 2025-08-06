namespace takashicompany.Unity
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class StringStringAttribute : Attribute
	{
		public string key { get; }

		public string value { get; }

		public StringStringAttribute(string key, string value)
		{
			this.key = key;
			this.value = value;
		}
	}
}
