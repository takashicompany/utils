namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public static class CellUtils
	{
		public static Vector3Int ToCell(Vector3 position, Vector3 cellSize)
		{
			return Vector3Int.FloorToInt(new Vector3(
				(position.x + cellSize.x * 0.5f) / cellSize.x,
				(position.y + cellSize.y * 0.5f) / cellSize.y,
				(position.z + cellSize.z * 0.5f) / cellSize.z
			));
		}

		/// <summary>
		/// 指定のTrasnformの子供のColliderを全て取得し、指定のセルサイズでグリッド化してHashSetに格納する
		/// </summary>
		public static HashSet<Vector3Int> GetCells(this Transform transform, Vector3 cellSize)
		{
			var result = new HashSet<Vector3Int>();
			var colliders = transform.GetComponentsInChildren<Collider>();

			foreach (var col in colliders)
			{
				Bounds worldBounds = col.bounds;

				// ローカル空間に変換
				Vector3 min = transform.InverseTransformPoint(worldBounds.min);
				Vector3 max = transform.InverseTransformPoint(worldBounds.max);

				Vector3 localBoundsMin = Vector3.Min(min, max);
				Vector3 localBoundsMax = Vector3.Max(min, max);

				Vector3Int minCell = Vector3Int.FloorToInt(Vector3.Scale(localBoundsMin, new Vector3(1 / cellSize.x, 1 / cellSize.y, 1 / cellSize.z)))/* - Vector3Int.one*/;
				Vector3Int maxCell = Vector3Int.FloorToInt(Vector3.Scale(localBoundsMax, new Vector3(1 / cellSize.x, 1 / cellSize.y, 1 / cellSize.z))) + Vector3Int.one;

				for (int x = minCell.x; x <= maxCell.x; x++)
				{
					for (int y = minCell.y; y <= maxCell.y; y++)
					{
						for (int z = minCell.z; z <= maxCell.z; z++)
						{
							// セルの中心点をローカル座標からワールド座標に変換してOverlapBoxで確認
							Vector3 localCenter = new Vector3(x, y, z);
							Vector3 worldCenter = transform.TransformPoint(Vector3.Scale(localCenter, cellSize));
							Vector3 worldHalfExtents = Vector3.Scale(cellSize, transform.lossyScale) * 0.5f;
							if (Physics.OverlapBox(worldCenter, worldHalfExtents * 0.999f, transform.rotation).Length > 0)
							{
								result.Add(new Vector3Int(x, y, z));
							}
						}
					}
				}
			}

			return result;
		}

		public static HashSet<Vector3Int> GetCellsWorld(this IEnumerable<Bounds> allBounds, Vector3 cellSize, Vector3 originOffset)
		{
			var result = new HashSet<Vector3Int>();

			foreach (var bounds in allBounds)
			{
				Vector3Int minCell = Vector3Int.FloorToInt(new Vector3(
					(bounds.min.x - originOffset.x) / cellSize.x,
					(bounds.min.y - originOffset.y) / cellSize.y,
					(bounds.min.z - originOffset.z) / cellSize.z
				));

				Vector3Int maxCell = Vector3Int.FloorToInt(new Vector3(
					(bounds.max.x - originOffset.x) / cellSize.x,
					(bounds.max.y - originOffset.y) / cellSize.y,
					(bounds.max.z - originOffset.z) / cellSize.z
				));

				var myBounds = bounds;
				myBounds.size *= 0.999f; // 0.999fでオーバーラップを回避する

				for (int x = minCell.x; x <= maxCell.x; x++)
				{
					for (int y = minCell.y; y <= maxCell.y; y++)
					{
						for (int z = minCell.z; z <= maxCell.z; z++)
						{
							Vector3 cellCenter = new Vector3(
								x * cellSize.x + originOffset.x,
								y * cellSize.y + originOffset.y,
								z * cellSize.z + originOffset.z
							);
							Bounds cellBounds = new Bounds(cellCenter, cellSize);
							
							if (cellBounds.Intersects(myBounds))
							{
								result.Add(new Vector3Int(x, y, z));
							}
						}
					}
				}
			}

			return result;
		}

		public static List<Vector3Int> GetOverlappingCells(this Transform gridOrigin, Bounds worldBounds, Vector3 cellSize, Vector3 originOffset, IEnumerable<Vector3Int> gridCells)
		{
			var result = new List<Vector3Int>();

			Vector3 localMin = gridOrigin.InverseTransformPoint(worldBounds.min);
			Vector3 localMax = gridOrigin.InverseTransformPoint(worldBounds.max);

			Vector3 localBoundsMin = Vector3.Min(localMin, localMax);
			Vector3 localBoundsMax = Vector3.Max(localMin, localMax);

			Vector3Int minCell = Vector3Int.FloorToInt(new Vector3(
				(localBoundsMin.x - originOffset.x) / cellSize.x,
				(localBoundsMin.y - originOffset.y) / cellSize.y,
				(localBoundsMin.z - originOffset.z) / cellSize.z
			));

			Vector3Int maxCell = Vector3Int.FloorToInt(new Vector3(
				(localBoundsMax.x - originOffset.x) / cellSize.x,
				(localBoundsMax.y - originOffset.y) / cellSize.y,
				(localBoundsMax.z - originOffset.z) / cellSize.z
			));

			for (int x = minCell.x; x <= maxCell.x; x++)
			{
				for (int y = minCell.y; y <= maxCell.y; y++)
				{
					for (int z = minCell.z; z <= maxCell.z; z++)
					{
						var cell = new Vector3Int(x, y, z);
						if (gridCells.Contains(cell))
						{
							result.Add(cell);
						}
					}
				}
			}

			return result;
		}

		public static List<Vector3Int> GetOverlappingCellsWorld(Bounds worldBounds, Vector3 cellSize, Vector3 originOffset, IEnumerable<Vector3Int> gridCells)
		{
			var result = new List<Vector3Int>();

			Vector3Int minCell = Vector3Int.FloorToInt(new Vector3(
				(worldBounds.min.x - originOffset.x) / cellSize.x,
				(worldBounds.min.y - originOffset.y) / cellSize.y,
				(worldBounds.min.z - originOffset.z) / cellSize.z
			));

			Vector3Int maxCell = Vector3Int.FloorToInt(new Vector3(
				(worldBounds.max.x - originOffset.x) / cellSize.x,
				(worldBounds.max.y - originOffset.y) / cellSize.y,
				(worldBounds.max.z - originOffset.z) / cellSize.z
			));

			for (int x = minCell.x; x <= maxCell.x; x++)
			{
				for (int y = minCell.y; y <= maxCell.y; y++)
				{
					for (int z = minCell.z; z <= maxCell.z; z++)
					{
						var cell = new Vector3Int(x, y, z);
						if (gridCells.Contains(cell))
						{
							result.Add(cell);
						}
					}
				}
			}

			return result;
		}

	}
}
