namespace TakashiCompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[CreateAssetMenu(fileName = "ObjectBundle", menuName = "Object Bundle")]
	public class ObjectBundle : ScriptableObject
	{
		[System.Serializable]
		protected class Param
		{
			[SerializeField]
			private string _key = "";

			public string key => _key;

			[SerializeField]
			private Object _obj;

			public Object obj => _obj;

			public Param(string key, Object obj)
			{
				_key = key;
				_obj = obj;
			}

		}

		[SerializeField]
		private Param[] _paramList;


		[ContextMenu("Setup Number Bundle")]
		private void NumberBundle()
		{
			var suffix = PowerOf2.suffix.Where(s => !string.IsNullOrEmpty(s)).ToArray();
			_paramList = new Param[10 + suffix.Length];

			for (int i = 0; i < 10; i++)
			{
				var p = new Param(i.ToString(), null);

				_paramList[i] = p;
			}

			for (int i = 0; i < suffix.Length; i++)
			{
				var p = new Param(suffix[i], null);
				_paramList[i + 10] = p;
			}
		}

		public Dictionary<string, T> BuildDictionary<T>() where T : Object
		{
			var dict = new Dictionary<string, T>();

			Debug.Log(typeof(T));

			foreach (var p in _paramList)
			{
				// if (p.obj is T obj)
				// {
				// 	dict.Add(p.key, obj);
				// }

				// var obj = p.obj as T;

				// Debug.Log(obj != null);

				// if (obj != null)
				// {
				// 	dict.Add(p.key, obj);
				// }

				dict.Add(p.key, (T)p.obj);
			}

			return dict;
		}

		public Dictionary<string, Sprite> BuildDictionary()
		{
			var dict = new Dictionary<string, Sprite>();

			foreach (var p in _paramList)
			{
				// if (p.obj is T obj)
				// {
				// 	dict.Add(p.key, obj);
				// }

				// var obj = p.obj as T;

				// Debug.Log(obj != null);

				// if (obj != null)
				// {
				// 	dict.Add(p.key, obj);
				// }

				dict.Add(p.key, (Sprite)p.obj);
			}

			return dict;
		}

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