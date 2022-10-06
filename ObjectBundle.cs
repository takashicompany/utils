namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public abstract class ObjectBundle<T> : ScriptableObject where T : Object
	{
		[SerializeField]
		protected List<T> _list;

		public Dictionary<string, T> BuildNameDictionary()
		{
			var dict = new Dictionary<string, T>();

			foreach (var item in _list)
			{
				dict.TryAdd(item.name, item);
			}

			return dict;
		}
	}
	
	public abstract class ObjectBundle<K, V> : ScriptableObject, IEnumerable<V> where V : Object
	{
		[SerializeField]
		protected SerializableDictionary<K, V> _dict;

		public IEnumerator<V> GetEnumerator()
		{
			foreach (var kvp in _dict)
			{
				yield return kvp.Value;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach (var kvp in _dict)
			{
				yield return kvp.Value;
			}
		}

		public Dictionary<K, V> BuildDictionary()
		{
			return new Dictionary<K, V>(_dict);
		}
	}
}