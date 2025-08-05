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

		public static V3IntDirection Reverse(this V3IntDirection direction)
		{
			switch (direction)
			{
				case V3IntDirection.Left: return V3IntDirection.Right;
				case V3IntDirection.Right: return V3IntDirection.Left;
				case V3IntDirection.Down: return V3IntDirection.Up;
				case V3IntDirection.Up: return V3IntDirection.Down;
				case V3IntDirection.Back: return V3IntDirection.Forward;
				case V3IntDirection.Forward: return V3IntDirection.Back;
				default: return direction; // Noneの場合はそのまま返す
			}
		}

		public static bool TryToXZ(this KeyCode keyCode, out V3IntDirection direction)
		{
			switch (keyCode)
			{
				case KeyCode.LeftArrow:
				case KeyCode.A:
					direction = V3IntDirection.Left;
					return true;

				case KeyCode.RightArrow:
				case KeyCode.D:
					direction = V3IntDirection.Right;
					return true;

				case KeyCode.DownArrow:
				case KeyCode.S:
					direction = V3IntDirection.Back;
					return true;

				case KeyCode.UpArrow:
				case KeyCode.W:
					direction = V3IntDirection.Forward;
					return true;
			}

			direction = V3IntDirection.None;
			return false;
		}

		public static bool GetKey(this V3IntDirection direction)
		{
			switch (direction)
			{
				case V3IntDirection.Left:
					return Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
				case V3IntDirection.Right:
					return Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);

				// yとzは上下の入力とする

				case V3IntDirection.Down:
				case V3IntDirection.Back:
					return Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
				
				case V3IntDirection.Up:
				case V3IntDirection.Forward:
					return Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);

				default:
					return false;
			}
		}

		public static bool GetKeyDown(this V3IntDirection direction)
		{
			switch (direction)
			{
				case V3IntDirection.Left:
					return Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
				case V3IntDirection.Right:
					return Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);

				// yとzは上下の入力とする

				case V3IntDirection.Down:
				case V3IntDirection.Back:
					return Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
				
				case V3IntDirection.Up:
				case V3IntDirection.Forward:
					return Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);

				default:
					return false;
			}
		}

		public static bool GetKeyUp(this V3IntDirection direction)
		{
			switch (direction)
			{
				case V3IntDirection.Left:
					return Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A);
				case V3IntDirection.Right:
					return Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D);

				// yとzは上下の入力とする

				case V3IntDirection.Down:
				case V3IntDirection.Back:
					return Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S);
				
				case V3IntDirection.Up:
				case V3IntDirection.Forward:
					return Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W);

				default:
					return false;
			}
		}
		
	}
}
