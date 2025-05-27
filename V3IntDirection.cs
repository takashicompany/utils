namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[System.Flags]
	public enum V3IntDirection
	{
		None = 0b0,
		Left = 0b1,
		Right = 0b10,
		Down = 0b100,
		Up = 0b1000,
		Back = 0b10000,
		Forward = 0b100000,
	}

	public static class V3IntDirectionExtensions
	{
		public static Vector3Int ToVector3Int(this V3IntDirection direction)
		{
			Vector3Int result = Vector3Int.zero;
			if (direction.HasFlag(V3IntDirection.Left)) result.x--;
			if (direction.HasFlag(V3IntDirection.Right)) result.x++;
			if (direction.HasFlag(V3IntDirection.Down)) result.y--;
			if (direction.HasFlag(V3IntDirection.Up)) result.y++;
			if (direction.HasFlag(V3IntDirection.Back)) result.z--;
			if (direction.HasFlag(V3IntDirection.Forward)) result.z++;
			return result;
		}

		public static int ToIndex(this V3IntDirection direction)
		{
			switch (direction)
			{
				case V3IntDirection.Left:
				case V3IntDirection.Right:
					return 0;

				case V3IntDirection.Down:
				case V3IntDirection.Up:
					return 1;

				case V3IntDirection.Back:
				case V3IntDirection.Forward:
					return 2;
			}

			return -1;
		}

		public static int ToValue(this V3IntDirection direction)
		{
			switch (direction)
			{
				case V3IntDirection.Left:
				case V3IntDirection.Down:
				case V3IntDirection.Back:
					return -1;

				case V3IntDirection.Right:
				case V3IntDirection.Up:
				case V3IntDirection.Forward:
					return 1;
			}

			return 0;
		}

		public static bool IsPlus(this V3IntDirection direction)
		{
			return direction.ToValue() > 0;
		}

		public static int ToSign(this V3IntDirection direction)
		{
			return direction.ToValue();
		}

		/// <summary>
		/// BoundsIntからその方向の外側の点を返す。中心はmin + size / 2したもの。
		/// </summary>
		public static Vector3Int GetOuterPoint(this BoundsInt boundsInt, V3IntDirection direction)
		{
			Vector3Int center = boundsInt.min + boundsInt.size / 2; // 整数除算

			var minOrMax = direction.IsPlus() ? boundsInt.max : boundsInt.min - Vector3Int.one;

			center[direction.ToIndex()] = minOrMax[direction.ToIndex()];

			return center;
		}
	}
}
