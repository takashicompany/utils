namespace TakashiCompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[CreateAssetMenu(fileName = "NumberBundle", menuName = "Number Bundle")]
	public class NumberBundle : ScriptableObject
	{
		[System.Serializable]
		protected class KeyValuePair
		{
			[SerializeField]
			private string _character = "";

			public string character => _character;

			[SerializeField]
			private Object _obj;

			public Object obj => _obj;

		}

		[SerializeField]
		private KeyValuePair _dataList;

		// public Object[] GetObjects(string character)
		// {
			
		// }

		// public Object[] GetObjects(string[] characters)
		// {
		// 	var list = new List<Object>();

		// 	foreach (var c in characters)
		// 	{
				
		// 	}
		// }
	}

	
}