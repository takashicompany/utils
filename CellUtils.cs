namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public static class CellUtils
	{
		public static Vector3 ToWorldPosition(this Vector3Int cellPosition, Vector3 cellSize, Vector3 originOffset)
		{
			return new Vector3(
				cellPosition.x * cellSize.x + originOffset.x,
				cellPosition.y * cellSize.y + originOffset.y,
				cellPosition.z * cellSize.z + originOffset.z
			);
		}

		public static Vector3Int ToCellPosition(this Vector3 position, Vector3 cellSize)
		{
			return Vector3Int.RoundToInt(new Vector3(
				(position.x + cellSize.x * 0.5f) / cellSize.x,
				(position.y + cellSize.y * 0.5f) / cellSize.y,
				(position.z + cellSize.z * 0.5f) / cellSize.z
			));
		}

		/// <summary>
		/// 指定されたBounds(Collider.boundsなど)が占有するセルを取得する
		/// </summary>
		public static HashSet<Vector3Int> GetOccupiedCells(this IEnumerable<Bounds> allBounds, Vector3 cellSize, Vector3 originOffset)
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
	}
}
