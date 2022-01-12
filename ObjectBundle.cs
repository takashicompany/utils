namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	
	public abstract class ObjectBundle<K, V> : ScriptableObject where V : Object
	{
		[System.Serializable]
		protected class Param
		{
			[SerializeField]
			private K _key;

			public K key => _key;

			[SerializeField]
			private V _obj;

			public V obj => _obj;

			public Param(K key, V obj)
			{
				_key = key;
				_obj = obj;
			}

		}

		[SerializeField]
		protected Param[] _paramList;

		public Dictionary<K, V> BuildDictionary()
		{
			var dict = new Dictionary<K, V>();

			foreach (var p in _paramList)
			{
				dict.Add(p.key, p.obj);
			}

			return dict;
		}
	}

	

	
}