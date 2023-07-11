namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public interface IPlaced
	{
		void OnPlace(Vector3Int pointV3);
	}

	public class GridPlacer : MonoBehaviour
	{
		[System.Serializable]
		private class Param
		{
			[SerializeField]
			private Bounds _bounds = new Bounds(Vector3.zero, Vector3.one);

			[SerializeField]
			private Vector3Int _grid = new Vector3Int(1, 1, 1);

			public Vector3Int grid => _grid;

			[SerializeField]
			private List<BoundsInt> _noPlaceAreas = new List<BoundsInt>();

			[SerializeField]
			private Transform[] _prefabs;

			[SerializeField]
			private Transform _root;

			public List<Transform> Place()
			{
				var list = new List<Transform>();

				

				var zeroPoint = GetBounds().center;

				for (int x = 0; x < _grid.x; x++)
				{
					for (int y = 0; y < _grid.y; y++)
					{
						for (int z = 0; z < _grid.z; z++)
						{
							var v3int = new Vector3Int(x, y, z);

							if (_noPlaceAreas.Any((noPlaceArea) => noPlaceArea.Contains(v3int)))
							{
								continue;
							}
							
							var prefab = _prefabs[Random.Range(0, _prefabs.Length)];

							Transform obj = null;

#if UNITY_EDITOR
							if (Application.isPlaying)
							{
								obj = Instantiate(prefab, _root);
							}
							else
							{
								obj = UnityEditor.PrefabUtility.InstantiatePrefab(prefab, _root) as Transform;
							}
#else
							obj = Instantiate(prefab, _root);
#endif
							obj.name = prefab.name + "(" + x + ", " + y + ", " + z + ")";
							obj.position = GetPosition(v3int);
							list.Add(obj);

							if (obj.TryGetComponent<IPlaced>(out var placed))
							{
								placed.OnPlace(v3int);
							}
						}
					}
				}
				
				return list;
			}

			public Vector3 GetUnitPerGrid()
			{
				return new Vector3(
					_bounds.size.x != 0f ? _bounds.size.x / _grid.x : 0,
					_bounds.size.y != 0f ? _bounds.size.y / _grid.y : 0,
					_bounds.size.z != 0f ? _bounds.size.z / _grid.z : 0);
			}

			public Vector3 GetPosition(Vector3Int cellPosition)
			{
				if (Application.isPlaying) Debug.Log($"bounds: {GetBounds()} _grid: {_grid}, cellPosition: {cellPosition} upg: {GetUnitPerGrid()}"); // " + Utils.GetPositionByCell(_grid, cellPosition, GetUnitPerGrid()));
				return GetBounds().center + Utils.GetPositionByCell(_grid, cellPosition, GetUnitPerGrid());
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
					_bounds.size.x != 0f ? (float)_bounds.size.x / _grid.x : 0,
					_bounds.size.y != 0f ? (float)_bounds.size.y / _grid.y : 0,
					_bounds.size.z != 0f ? (float)_bounds.size.z / _grid.z : 0);

				// var zeroPoint = GetBounds().center;

				for (int x = 0; x < _grid.x; x++)
				{
					for (int y = 0; y < _grid.y; y++)
					{
						for (int z = 0; z <_grid.z; z++)
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
		}
		
		[SerializeField]
		private Param[] _paramList;

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
				yield return p.grid;
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