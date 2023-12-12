namespace takashicompany.Unity
{
	using UnityEngine;
	using System.Collections.Generic;
	using System.Linq;

	[System.Serializable]
	public abstract class PoolingContainer<T>
	{
		protected List<T> _pooled = new List<T>();

		public List<T> pooled => _pooled;

		/// <summary>
		/// 指定した個数までオブジェクトを生成しておく
		/// </summary>
		public void Prepare(int amount)
		{
			while(_pooled.Count < amount)
			{
				Generate();
			}
		}

		public virtual T Get()
		{
			var obj = _pooled.FirstOrDefault(p => CanUse(p));

			if (obj == null)
			{
				obj = Generate();
			}

			Use(obj);

			return obj;
		}

		public virtual void CollectAll()
	 	{
			foreach (var obj in _pooled)
			{
				Collect(obj);
			}
		}

		public IEnumerable<T> GetUsedAll()
		{
			return _pooled.Where(p => IsUse(p));
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

		public Transform container => _container;

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
			obj.name = _prefab.name + "_" + _pooled.Count;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = _prefab.transform.localScale;
			_pooled.Add(obj);

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
	

	/// <summary>
	/// Prefabを元にPoolingContainerを生成するクラス
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ActivePoolingContainerPrefabBundle<T> where T : Component
	{
		private Transform _root;
		private Dictionary<T, ActivePoolingContainer<T>> _containers = new Dictionary<T, ActivePoolingContainer<T>>();

		public ActivePoolingContainerPrefabBundle(Transform root)
		{
			_root = root;
		}

		public T GetOrInstantiate(T prefab)
		{
			if (!_containers.ContainsKey(prefab))
			{
				var pool = new ActivePoolingContainer<T>();
				pool.Setup(prefab, _root);
				_containers.Add(prefab, pool);
			}

			return _containers[prefab].Get();
		}
	}
	
	/// <summary>
	/// 複数のPoolingContainerを束ねるクラス
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[System.Serializable]
	public class ActivePoolingContainerBundle<T> /*: PoolingContainer<T>*/ where T : Component
	{
		[SerializeField]
		private Transform _container;

		[SerializeField]
		private T[] _prefabs;

		private Dictionary<T, ActivePoolingContainer<T>> _prefabDict;
		/*private Dictionary<T, ActivePoolingContainer<T>> _instanceDict;*/

		private void Init()
		{
			if (_prefabDict == null/* && _instanceDict == null*/)
			{
				_prefabDict = new Dictionary<T, ActivePoolingContainer<T>>();
				//_instanceDict = new Dictionary<T, ActivePoolingContainer<T>>();

				foreach (var prefab in _prefabs)
				{
					var container = new ActivePoolingContainer<T>();
					container.Setup(prefab, _container);
					_prefabDict.Add(prefab, container);
				}
			}
		}

		public T Get()
		{
			Init();

			var prefab = _prefabs.GetRandom();
			var container = _prefabDict[prefab];
			return container.Get();
		}

		public void CollectAll()
		{
			Init();

			foreach (var container in _prefabDict.Values)
			{
				container.CollectAll();
			}
		}
	}
}