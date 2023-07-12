namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	// public interface IPlaced
	// {
	// 	void OnPlace(Vector3Int pointV3);
	// }

	public class GridPlacer : MonoBehaviour
	{
		[System.Serializable]
		public class Param<T> where T : Component
		{
			[SerializeField]
			private Bounds _bounds = new Bounds(Vector3.zero, Vector3.one);

			[SerializeField]
			private Vector3Int _grids = new Vector3Int(1, 1, 1);

			public Vector3Int grids => _grids;

			[SerializeField]
			private List<BoundsInt> _noPlaceAreas = new List<BoundsInt>();

			[SerializeField]
			private T[] _prefabs;

			public IReadOnlyList<T> prefabs => _prefabs;

			[SerializeField]
			private Transform _root;

			public void SetPrefabs(T[] prefabs)
			{
				_prefabs = prefabs;
			}

			public List<T> Place(System.Action<T> onGenerate = null)
			{
				var list = new List<T>();

				var zeroPoint = GetBounds().center;

				for (int x = 0; x < _grids.x; x++)
				{
					for (int y = 0; y < _grids.y; y++)
					{
						for (int z = 0; z < _grids.z; z++)
						{
							var v3int = new Vector3Int(x, y, z);

							if (_noPlaceAreas.Any((noPlaceArea) => noPlaceArea.Contains(v3int)))
							{
								continue;
							}
							
							var prefab = _prefabs[Random.Range(0, _prefabs.Length)];

							T obj = default(T);

#if UNITY_EDITOR
							if (Application.isPlaying)
							{
								obj = Instantiate(prefab, _root);
							}
							else
							{
								obj = UnityEditor.PrefabUtility.InstantiatePrefab(prefab, _root) as T;	// 動くかは未確認
							}
#else
							obj = Instantiate(prefab, _root);
#endif
							obj.name = prefab.name;
							Place(obj, v3int);
							onGenerate?.Invoke(obj);
							list.Add(obj);

							// if (obj.TryGetComponent<IPlaced>(out var placed))
							// {
							// 	placed.OnPlace(v3int);
							// }
						}
					}
				}
				
				return list;
			}

			public void Place(T obj, Vector3Int gridPosition)
			{
				obj.name = obj.name + "(" + gridPosition.x + ", " + gridPosition.y + ", " + gridPosition.z + ")";
				obj.transform.position = GetPosition(gridPosition);
			}

			public Vector3 GetUnitPerGrid()
			{
				return new Vector3(
					_bounds.size.x != 0f ? _bounds.size.x / _grids.x : 0,
					_bounds.size.y != 0f ? _bounds.size.y / _grids.y : 0,
					_bounds.size.z != 0f ? _bounds.size.z / _grids.z : 0);
			}

			public Vector3 GetPosition(Vector3Int cellPosition)
			{
				return GetBounds().center + Utils.GetPositionByCell(_grids, cellPosition, GetUnitPerGrid());
			}

			private Bounds GetBounds()
			{
				return _bounds.Transform(_root);
			}

			public void DrawGizmos()
			{
				if (_root == null)
				{
					return;
				}
			
				var b = GetBounds();

				var unitPerGrid = new Vector3(
					_bounds.size.x != 0f ? (float)_bounds.size.x / _grids.x : 0,
					_bounds.size.y != 0f ? (float)_bounds.size.y / _grids.y : 0,
					_bounds.size.z != 0f ? (float)_bounds.size.z / _grids.z : 0);

				// var zeroPoint = GetBounds().center;

				for (int x = 0; x < _grids.x; x++)
				{
					for (int y = 0; y < _grids.y; y++)
					{
						for (int z = 0; z <_grids.z; z++)
						{
							// var p = Utils.GetPositionByCell(_grid, new Vector3Int(x, y, z), unitPerGrid);

							Gizmos.DrawWireCube(GetPosition(new Vector3Int(x, y, z)), unitPerGrid);
						}
					}
				}
			}

			public void AddNoPlaceArea(BoundsInt b)
			{
				_noPlaceAreas.Add(b);
			}

			public void AddNoPlaceArea(Vector3Int pos)
			{
				_noPlaceAreas.Add(new BoundsInt(pos, Vector3Int.one));
			}
		}
		
		[SerializeField]
		private Param<Transform>[] _paramList;

		[SerializeField]
		private List<Transform> _placed = new List<Transform>();

		void Start()
		{
			if (_placed == null || _placed.Count == 0)
			{
				Place();
			}
		}
		
		[ContextMenu("配置する")]
		public void Place()
		{
			foreach (var p in _paramList)
			{
				_placed.AddRange(p.Place());
			}
		}

		public void AddNoPlaceArea(BoundsInt b)
		{
			foreach (var p in _paramList)
			{
				p.AddNoPlaceArea(b);
			}
		}

		public IEnumerable<Vector3Int> GetGrids()
		{
			foreach (var p in _paramList)
			{
				yield return p.grids;
			}
		}

		public Vector3 GetPosition(Vector3Int cellPosition, int paramListIndex = 0)
		{
			return _paramList[paramListIndex].GetPosition(cellPosition);
		}

		[ContextMenu("消去する")]
		private void Clear()
		{
			foreach (var p in _placed)
			{
				DestroyImmediate(p.gameObject);
			}

			_placed.Clear();
		}

		public List<Transform> GenerateList()
		{
			return new List<Transform>(_placed);
		}

		private void OnDrawGizmosSelected() {
		
			if (_paramList != null)
			{

				var index = 0;
				foreach (var p in _paramList)
				{
					Gizmos.color = PowerOf2.GetColor(index);	// お借りしちゃおう
					p.DrawGizmos();
					index++;
				}
			}
		}
	}
}