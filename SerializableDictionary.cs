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
	public class SerializableDictionary<K, V>: IEnumerable<KeyValuePair<K, V>>, IDictionary<K, V>
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

			private KeyValuePair()
			{

			}

			public static KeyValuePair Empty()
			{
				return new KeyValuePair();
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
			set 
			{	
				if (!_dict.ContainsKey(key))
				{
					Add(key, value);
				}
				else
				{
					_dict[key].value = value;
				}
			}
		}

		public bool TryAdd(K key, V value)
		{
			if (_dict.ContainsKey(key))
			{
				return false;
			}

			AddInternal(key, value);
			
			return true;
		}

		private void AddInternal(K key, V value)
		{
			var kvp =  new KeyValuePair(key, value);

			_dict.Add(key, kvp);
			_list.Add(kvp);
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
			var result = _dict.TryGetValue(key, out var kvp);

			if (!result)
			{
				kvp = KeyValuePair.Empty();
			}

			value = kvp.value;
			return result;
		}

		public void Add(KeyValuePair<K, V> item)
		{
			TryAdd(item.Key, item.Value);
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

		public void Add(K key, V value)
		{
			AddInternal(key, value);
		}
	}

	public static class SerializableDictionaryExtensions
	{
		public static bool HasFlag<K, V>(this SerializableDictionary<K, V> self, K flag) where K : System.Enum
		{
			foreach (var kvp in self)
			{
				if(kvp.Key.HasFlag(flag))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// フラグから見た辞書のキーでフラグを判定する
		/// </summary>
		public static bool HasFlag<K, V>(this K flag, SerializableDictionary<K, V> dict) where K : System.Enum
		{
			foreach (var kvp in dict)
			{
				if(flag.HasFlag(kvp.Key))
				{
					return true;
				}
			}

			return false;
		}
	}
}