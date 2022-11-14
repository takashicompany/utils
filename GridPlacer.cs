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

			[SerializeField]
			private Transform[] _prefabs;

			[SerializeField]
			private Transform _root;

			public List<Transform> Place()
			{
				var list = new List<Transform>();

				var unitPerGrid = new Vector3(
					_bounds.size.x != 0f ? _bounds.size.x / _grid.x : 0,
					_bounds.size.y != 0f ? _bounds.size.y / _grid.y : 0,
					_bounds.size.z != 0f ? _bounds.size.z / _grid.z : 0);

				var zeroPoint = GetBounds().center;

				for (int x = 0; x < _grid.x; x++)
				{
					for (int y = 0; y < _grid.y; y++)
					{
						for (int z = 0; z < _grid.z; z++)
						{
							var v3int = new Vector3Int(x, y, z);
							var prefab = _prefabs[Random.Range(0, _prefabs.Length)];
							var obj = Instantiate(prefab, _root);
							obj.name = prefab.name + "(" + x + ", " + y + ", " + z + ")";
							var p = Utils.GetPositionByCell(_grid, v3int, unitPerGrid);
							obj.position = zeroPoint + p;
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

				var zeroPoint = GetBounds().center;

				for (int x = 0; x < _grid.x; x++)
				{
					for (int y = 0; y < _grid.y; y++)
					{
						for (int z = 0; z <_grid.z; z++)
						{
							var p = Utils.GetPositionByCell(_grid, new Vector3Int(x, y, z), unitPerGrid);

							Gizmos.DrawWireCube(zeroPoint + p, unitPerGrid);
						}
					}
				}
			}
		}
		
		[SerializeField]
		private Param[] _paramList;

		private List<Transform> _placed = new List<Transform>();

		void Awake()
		{
			foreach (var p in _paramList)
			{
				_placed.AddRange(p.Place());
			}
		}

		// https://hwks.hatenadiary.jp/entry/2014/06/13/022747 のため
		void Start()
		{

		}

		public List<Transform> GenerateList()
		{
			return new List<Transform>(_placed);
		}

		private void OnDrawGizmos()
		{
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