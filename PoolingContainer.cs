namespace takashicompany.Unity
{

	using UnityEngine;
	using System.Collections.Generic;
	using System.Linq;
	
	/// <summary>
	/// GameObjectを再利用するクラス
	/// GameObject.activeSelfで再利用可能かを判断する
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[System.Serializable]
	public class PoolingContainer<T> where T : Component
	{
		[SerializeField]
		protected T _prefab;

		public T prefab => _prefab;

		[SerializeField]
		protected Transform _container;

		protected List<T> _pooledList = new List<T>();

		// コンストラクタを入れると動かなくなるのでいれないでくれ
		// public PoolingContainer(T prefab, Transform container)
		// {
		// 	_prefab = prefab;
		// 	_container = container;
		// 	_pooledList = new List<T>();
		// }

		public void Setup(T prefab, Transform container)
		{
			_prefab = prefab;
			_container = container;
		}

		/// <summary>
		/// 指定した個数までオブジェクトを生成しておく
		/// </summary>
		public void Prepare(int amount)
		{
			while(_pooledList.Count < amount)
			{
				Generate();
			}
		}

		/// <summary>
		/// 全てのオブジェクトを回収する
		/// </summary>
		public virtual void CollectAll()
		{
			foreach (var item in _pooledList)
			{
				item.transform.SetParent(_container);
				item.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// activeがtrue/falseのオブジェクトを一つ取得する
		/// </summary>
		/// <param name="flag"></param>
		/// <returns></returns>
		protected T FindOne(bool flag)
		{
			return _pooledList.Find(m => m.gameObject.activeSelf == flag);
		}

		/// <summary>
		/// activeがtrue/falseのオブジェクトリストを取得する
		/// </summary>
		/// <param name="flag"></param>
		/// <returns></returns>
		private IEnumerable<T> FindAll(bool usable)
		{
			foreach (var p in _pooledList)
			{
				if (IsUsed(p) == usable)
				{
					yield return p;
				}
			}
		}

		/// <summary>
		/// オブジェクトを取得する
		/// </summary>
		/// <returns></returns>
		public virtual T Get()
		{
			var myObject = FindOne(false);

			if (myObject == null)
			{
				myObject = Generate();
			}
			myObject.gameObject.SetActive(true);

			return myObject;
		}

		public List<T> GetUsedList()
		{
			return FindAll(true).ToList();
		}

		public IEnumerable<T> GetUsedAll()
		{
			return FindAll(true);
		}

		/// <summary>
		/// オブジェクトを生成する
		/// </summary>
		/// <returns></returns>
		protected virtual T Generate()
		{
			var myObject = GameObject.Instantiate(_prefab, _container);
			myObject.gameObject.SetActive(false);
			myObject.name = _prefab.name + "_" + _pooledList.Count;
			myObject.transform.localPosition = Vector3.zero;
			myObject.transform.localScale = _prefab.transform.localScale;
			_pooledList.Add(myObject);

			return myObject;
		}

		/// <summary>
		/// オブジェクトが利用されているか
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static bool IsUsed(T obj)
		{
			return obj.gameObject.activeSelf;
		}

		public static bool CanUse(T obj)
		{
			return !IsUsed(obj);
		}
	}
}