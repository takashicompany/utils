namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	
	/// <summary>
	/// インスペクターで辞書を使えるようにした
	/// </summary>
	[System.Serializable]
	public class SerializableDictionary<K, V>: IEnumerable<KeyValuePair<K, V>>//, IDictionary<K, V>
	{
		[System.Serializable]
		private class KeyValuePair
		{
			[SerializeField]
			private K _key;

			public K key => _key;

			[SerializeField]
			private V _value;

			public V value
			{
				get => _value;
				set => _value = value;
			}

			public KeyValuePair(K key, V value)
			{
				_key = key;
				_value = value;
			}
		}

		[SerializeField]
		private List<KeyValuePair> _list = new List<KeyValuePair>();

		private Dictionary<K, KeyValuePair> _dictInternal;

		private Dictionary<K, KeyValuePair> _dict
		{
			get
			{
				if (_dictInternal == null)
				{
					_dictInternal = new Dictionary<K, KeyValuePair>();

					foreach (var kvp in _list)
					{
						_dictInternal.Add(kvp.key, kvp);
					}
				}

				return _dictInternal;
			}
		}

		public ICollection<K> Keys => throw new System.NotImplementedException();

		public ICollection<V> Values => throw new System.NotImplementedException();

		public int Count => throw new System.NotImplementedException();

		public bool IsReadOnly => throw new System.NotImplementedException();

		public V this[K key]
		{
			get => _dict[key].value;
			set => _dict[key].value = value;
		}

		public bool Add(K key, V value)
		{
			if (_dict.ContainsKey(key))
			{
				return false;
			}

			var kvp =  new KeyValuePair(key, value);

			_dict.Add(key, kvp);
			_list.Add(kvp);
			
			return true;
		}

		IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator()
		{
			foreach (var kvp in _dict)
			{
				yield return new KeyValuePair<K, V>(kvp.Key, kvp.Value.value);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach (var kvp in _dict)
			{
				yield return new KeyValuePair<K, V>(kvp.Key, kvp.Value.value);
			}
		}

		public bool ContainsKey(K key)
		{
			return _dict.ContainsKey(key);
		}

		public bool Remove(K key)
		{
			_list.RemoveAll(m => m.key.Equals(key));

			return _dict.Remove(key);
		}

		public bool TryGetValue(K key, out V value)
		{
			throw new System.NotImplementedException();
		}

		public void Add(KeyValuePair<K, V> item)
		{
			Add(item.Key, item.Value);
		}

		public void Clear()
		{
			_dict.Clear();
			_list.Clear();
		}

		public bool Contains(KeyValuePair<K, V> item)
		{
			throw new System.NotImplementedException();
		}

		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			throw new System.NotImplementedException();
		}

		public bool Remove(KeyValuePair<K, V> item)
		{
			throw new System.NotImplementedException();
		}
	}
}