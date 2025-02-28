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
		public string name { get; }

		public string value { get; }

		public StringStringAttribute(string name, string value)
		{
			this.name = name;
			this.value = value;
		}
	}
}
