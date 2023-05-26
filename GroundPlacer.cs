// namespace takashicompany.Unity
// {
// 	using System.Collections;
// 	using System.Collections.Generic;
// 	using System.Linq;
// 	using UnityEngine;

// 	public class GroundPlacer : MonoBehaviour
// 	{
// 		[SerializeField]
// 		private Vector3 _cellSize = Vector3.one;

// 		public Vector3 cellSize { get { return _cellSize; } set { _cellSize = value; } }

// 		[SerializeField]
// 		private List<Collider> _colliders = new List<Collider>();

// 		public List<Collider> colliders => _colliders;

// 		public Bounds bounds { get; private set; }

// 		public void UpdateBounds()
// 		{
// 			bounds = GetBounds();
// 		}

// 		private Bounds GetBounds()
// 		{
// 			var bounds = _colliders.Select(c => c.bounds).GetBounds();

// 			var size = bounds.size;
// 			size.y = 0f;
// 			bounds.size = size;

// 			return bounds;
// 		}

// 		private Vector3Int GetGrid()
// 		{
// 			return new Vector3Int(
// 				Mathf.RoundToInt(bounds.size.x / _cellSize.x),
// 				1, // Mathf.RoundToInt(bounds.size.y / _cellSize.y),
// 				Mathf.RoundToInt(bounds.size.z / _cellSize.z));
// 		}

// 		private Vector3 GetPositon(Vector3Int point)
// 		{
// 			var min = bounds.min + _cellSize / 2;

// 			return min + new Vector3(
// 				point.x * _cellSize.x,
// 				0f,
// 				point.z * _cellSize.z);
// 		}

// 		public delegate bool CanPlaceDelegate(RaycastHit hit);

// 		public bool CanPlace(Vector3Int point, float y, int layer, CanPlaceDelegate delegateMethod = null)
// 		{
// 			var pos = GetPositon(point);
// 			pos.y = y;
			
// 			var success = Physics.BoxCast(pos, cellSize / 2, Vector3.down, out var hit, Quaternion.identity, 100f, layer);

// 			if (delegateMethod != null)
// 			{
// 				if (success && delegateMethod.Invoke(hit))
// 				{
// 					return true;
// 				}
// 			}
			
// 			return success;
// 		}

// 		public List<Vector3Int> GetPlaceablePoints(float y, int layer, CanPlaceDelegate delegateMethod = null)
// 		{
// 			var grid = GetGrid();

// 			var points = new List<Vector3Int>();

// 			grid.Foreach(v3 =>
// 			{
// 				if (CanPlace(v3, y, layer, delegateMethod))
// 				{
// 					points.Add(v3);
// 				}
// 			});

// 			return points;
// 		}

// 		private void OnDrawGizmos()
// 		{
// 			UpdateBounds();
// 			var grid = GetGrid();

// 			grid.Foreach(v3 =>
// 			{
// 				var pos = GetPositon(v3);

// 				Gizmos.DrawWireCube(pos, _cellSize);
// 			});
// 		}

// 	}
// }


namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class GroundPlacer : MonoBehaviour
	{
		[SerializeField]
		private Vector3 _cellSize = Vector3.one;

		public Vector3 cellSize { get { return _cellSize; } set { _cellSize = value; } }

		[SerializeField]
		private List<Collider> _colliders = new List<Collider>();

		public List<Collider> colliders => _colliders;

		public Bounds bounds { get; private set; }

		public void UpdateBounds()
		{
			bounds = GetBounds(_colliders);
		}

		public static Bounds GetBounds(IEnumerable<Collider> colliders)
		{
			var bounds = colliders.Select(c => c.bounds).GetBounds();

			var size = bounds.size;
			size.y = 0f;
			bounds.size = size;

			return bounds;
		}

		private static Vector3Int GetGrid(Bounds bounds, Vector3 cellSize)
		{
			return new Vector3Int(
				Mathf.RoundToInt(bounds.size.x / cellSize.x),
				1, // Mathf.RoundToInt(bounds.size.y / _cellSize.y),
				Mathf.RoundToInt(bounds.size.z / cellSize.z));
		}

		public static Vector3 GetPositon(Bounds bounds, Vector3Int point, Vector3 cellSize)
		{
			var min = bounds.min + cellSize / 2;

			return min + new Vector3(
				point.x * cellSize.x,
				0f,
				point.z * cellSize.z);
		}

		public delegate bool CanPlaceDelegate(RaycastHit hit);

		public static bool CanPlace(Bounds bounds, Vector3Int point, float y, int layer, Vector3 cellSize, CanPlaceDelegate delegateMethod = null)
		{
			var pos = GetPositon(bounds, point, cellSize);
			pos.y = y;

			// 隣のコライダーにヒットする時があるので、キャストの箱のサイズは0.99倍にしておく
			var success = Physics.BoxCast(pos, (cellSize / 2) * 0.99f, Vector3.down, out var hit, Quaternion.identity, y * 2);

			if (delegateMethod != null)
			{
				if (success && delegateMethod.Invoke(hit))
				{
					return true;
				}
			}

			return success;
		}

		public static List<Vector3Int> GetPlaceablePoints(IEnumerable<Collider> colliders, float y, int layer, Vector3 cellSize, CanPlaceDelegate delegateMethod = null)
		{
			var bounds = GetBounds(colliders);
			var grid = GetGrid(bounds, cellSize);

			var points = new List<Vector3Int>();

			grid.Foreach(v3 =>
			{
				if (CanPlace(bounds, v3, y, layer, cellSize, delegateMethod))
				{
					points.Add(v3);
				}
			});

			return points;
		}

		private void OnDrawGizmos()
		{
			UpdateBounds();
			var grid = GetGrid(bounds, _cellSize);

			grid.Foreach(v3 =>
			{
				var pos = GetPositon(bounds, v3, _cellSize);

				var canPlace = CanPlace(bounds, v3, 50f, 1, _cellSize);

				Gizmos.color = canPlace ? Color.green : Color.red;

				Gizmos.DrawWireCube(pos, _cellSize);
			});
		}
	}
}
