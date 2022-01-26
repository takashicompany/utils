namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class RandomPlacer : MonoBehaviour
	{

		[System.Serializable]
		private class Param
		{
			[SerializeField]
			private int _min = 10;

			[SerializeField]
			private int _max = 20;

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
				var map = new Transform[_grid.x, _grid.y, _grid.z];

				var placed = new List<Transform>();

				var count = Random.Range(_min, _max);

				var unitPerGrid = new Vector3(
					_bounds.size.x != 0f ? _bounds.size.x / _grid.x : 0,
					_bounds.size.y != 0f ? _bounds.size.y / _grid.y : 0,
					_bounds.size.z != 0f ? _bounds.size.z / _grid.z : 0);

				var zeroPoint = GetBounds().center;

				for (int i = 0; i < count; i++)
				{
					var pos = _grid.GetRandom();

					var isValid = false;

					for (int j = 0; j < 100; j++)
					{
						if (map[pos.x, pos.y, pos.z] == null)
						{
							isValid = true;
							break;
						}
					}

					if (!isValid)
					{
						break;
					}

					var prefab = _prefabs[Random.Range(0, _prefabs.Length)];

					var obj = Instantiate(prefab, _root);

					obj.name = prefab.name + "(" + i + ")";
					// obj.transform.position = GetBounds().RandomPoint();

					map[pos.x, pos.y, pos.z] = obj;

					var p = Utils.GetPositionOnGrid(_grid, pos, unitPerGrid);

					obj.position = zeroPoint + p;

					placed.Add(obj);
				}
				
				return placed;
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

				Gizmos.color = Color.red;
			
				var b = GetBounds();

				Gizmos.DrawWireCube(b.center, b.size);
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

		private void OnDrawGizmos()
		{
			if (_paramList != null)
			{
				foreach (var p in _paramList)
				{
					p.DrawGizmos();
				}
			}
		}
	}
}
