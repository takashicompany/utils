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
	public class SerializableDictionary<K, V>: IEnumerable<KeyValuePair<K, V>>, IDictionary<K, V>, IReadOnlyDictionary<K, V>
	{
		[System.Serializable]
		public class SdKeyValuePair
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

			public SdKeyValuePair(K key, V value)
			{
				_key = key;
				_value = value;
			}
			
			private SdKeyValuePair()
			{

			}

			public static SdKeyValuePair Empty()
			{
				return new SdKeyValuePair();
			}
		}

		[SerializeField]
		private List<SdKeyValuePair> _list = new List<SdKeyValuePair>();

		public IReadOnlyList<SdKeyValuePair> list => _list;

		private Dictionary<K, SdKeyValuePair> _dictInternal;

		private Dictionary<K, SdKeyValuePair> _dict
		{
			get
			{
				if (_dictInternal == null)
				{
					_dictInternal = new Dictionary<K, SdKeyValuePair>();

					foreach (var kvp in _list)
					{
						_dictInternal.Add(kvp.key, kvp);
					}
				}

				return _dictInternal;
			}
		}

		public ICollection<K> Keys => _dict.Keys;

		public ICollection<V> Values => _dict.Values.Select(kvp => kvp.value).ToList();

		public int Count => _list.Count();

		public bool IsReadOnly => throw new System.NotImplementedException();

		IEnumerable<K> IReadOnlyDictionary<K, V>.Keys => this.Keys;

		IEnumerable<V> IReadOnlyDictionary<K, V>.Values => this.Values;

		int IReadOnlyCollection<KeyValuePair<K, V>>.Count => this.Count;

		V IReadOnlyDictionary<K, V>.this[K key] => this[key];

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

		public SerializableDictionary()
		{

		}

		public SerializableDictionary(IDictionary<K, V> dict)
		{
			foreach (var kvp in dict)
			{
				AddInternal(kvp.Key, kvp.Value);
			}
		}

		public SerializableDictionary(IReadOnlyDictionary<K, V> dict)
		{
			foreach (var kvp in dict)
			{
				AddInternal(kvp.Key, kvp.Value);
			}
		}

		public void Add(K key, V value)
		{
			if (_dict.ContainsKey(key))
			{
				return;
			}

			AddInternal(key, value);
			
			return;
		}

		private void AddInternal(K key, V value)
		{
			var kvp =  new SdKeyValuePair(key, value);

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
				kvp = SdKeyValuePair.Empty();
			}

			value = kvp.value;
			return result;

		}

		public void Add(KeyValuePair<K, V> item)
		{
			// ここの引数のKeyValuePairはC#のものなので分解する必要がある
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
			_list.Select(kvp => new KeyValuePair<K, V>(kvp.key, kvp.value)).ToArray().CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<K, V> item)
		{
			throw new System.NotImplementedException();
		}

		public Dictionary<K, V> GenerateDictionary()
		{
			var dict = new Dictionary<K, V>();

			CopyTo(dict);

			return dict;
		}

		public void CopyTo(IDictionary<K, V> dict)
		{
			dict.Clear();
			foreach (var kvp in _list)
			{
				dict.Add(kvp.key, kvp.value);
			}
		}

		bool IReadOnlyDictionary<K, V>.ContainsKey(K key)
		{
			return ContainsKey(key);
		}

		bool IReadOnlyDictionary<K, V>.TryGetValue(K key, out V value)
		{
			return TryGetValue(key, out value);
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
