namespace takashicompany.Unity
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// HashSetとListを併用したリスト。同じインスタンスを許容しない & 入れた順番が保持される
	/// </summary>
	[System.Serializable]
	public class HashSetList<T> : IList<T>, IReadOnlyList<T>
	{
		private HashSet<T> _hashSetInternal;
		private HashSet<T> _hashSet
		{
			get
			{
				if (_hashSetInternal == null)
				{
					_hashSetInternal = new HashSet<T>();

					for (int i = 0; i < _list.Count; i++)
					{
						if (!_hashSetInternal.Add(_list[i]))
						{
							// 既に登録済みの場合の処理。
						}
					}
				}

				return _hashSetInternal;
			}
		}


		private List<T> _list = new List<T>();

		public T this[int index]
		{
			get => _list[index];
			set
			{
				// 自分はこっちの実装を最初にイメージしていたけど、AIは排除しない実装にしていたので、一旦はAIを信じる。
				// if (_hashSet.Add(value))
				// {
				// 	_list[index] = value;
				// }

				_hashSet.Remove(_list[index]);
				_hashSet.Add(value);
				_list[index] = value;
			}
		}

		public int Count => _list.Count;

		public bool IsReadOnly => false;

		public bool TryAdd(T item)
		{
			if (_hashSet.Add(item))
			{
				_list.Add(item);
				return true;
			}
			return false;
		}

		public void Add(T item)
		{
			if (_hashSet.Add(item))
			{
				_list.Add(item);
			}
		}

		public void Clear()
		{
			_hashSet.Clear();
			_list.Clear();
		}

		public bool Contains(T item)
		{
			return _hashSet.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		public int IndexOf(T item)
		{
			return _list.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			if (_hashSet.Add(item))
			{
				_list.Insert(index, item);
			}
		}

		public bool Remove(T item)
		{
			if (_hashSet.Remove(item))
			{
				_list.Remove(item);
				return true;
			}
			return false;
		}

		public void RemoveAt(int index)
		{
			var item = _list[index];
			_hashSet.Remove(item);
			_list.RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		public void Sort(Comparison<T> comparison)
		{
			_list.Sort(comparison);
		}
	}
}
