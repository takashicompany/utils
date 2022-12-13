namespace takashicompany.Unity.ObjectPool
{
	using UnityEngine;
	using System.Collections.Generic;
	using System.Linq;

	[System.Serializable]
	public abstract class PoolingContainer<T>
	{
		protected List<T> _pooledList = new List<T>();

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

		public virtual T Get()
		{
			var obj = _pooledList.FirstOrDefault(p => CanUse(p));

			if (obj == null)
			{
				obj = Generate();
			}

			Use(obj);

			return obj;
		}

		public void CollectAll()
	 	{
			foreach (var obj in _pooledList)
			{
				Collect(obj);
			}
		}

		public IEnumerable<T> GetUsedAll()
		{
			return _pooledList.Where(p => IsUse(p));
		}

		public abstract bool IsUse(T obj);
		
		protected bool CanUse(T obj)
		{
			return !IsUse(obj);
		}

		protected abstract void Use(T obj);

		protected abstract T Generate();

		protected abstract void Collect(T obj);

	}


	[System.Serializable]
	public abstract class TransformPoolingContainer<T> : PoolingContainer<T> where T : Component
	{
		[SerializeField]
		protected Transform _container;

		[SerializeField]
		protected T _prefab;

		public T prefab => _prefab;

		public virtual void Setup(T prefab, Transform container)
		{
			_prefab = prefab;
			_container = container;
		}
		
		protected override void Collect(T obj)
		{
			obj.transform.SetParent(_container);
		}

		protected override T Generate()
		{
			var obj = GameObject.Instantiate(_prefab, _container);
			obj.name = _prefab.name + "_" + _pooledList.Count;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = _prefab.transform.localScale;
			_pooledList.Add(obj);

			Collect(obj);

			return obj;
		}
	}

	/// <summary>
	/// GameObjectを再利用するクラス
	/// GameObject.activeSelfで再利用可能かを判断する
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[System.Serializable]
	public class ActivePoolingContainer<T> : TransformPoolingContainer<T> where T : Component
	{
		protected override void Collect(T item)
		{
			base.Collect(item);
			item.gameObject.SetActive(false);
		}
		
		public override bool IsUse(T obj)
		{
			return obj.gameObject.activeSelf;
		}

		protected override void Use(T obj)
		{
			obj.gameObject.SetActive(true);
		}
	}
}