namespace takashicompany.Unity
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.EventSystems;
	using DG.Tweening;
	using Random = UnityEngine.Random;

# if UNITY_EDITOR
	using UnityEditor;

#endif

	public static class Utils
	{
		public static bool IsDevelopmentBuild()
		{
			return Application.identifier.Contains("dev");
		}

		public static bool IsDevelopmentBuildOrEditor()
		{
#if UNITY_EDITOR
			return true;
#else
			return IsDevelopmentBuild();
#endif
		}

#region int
		public static int Pow(this int self, int pow)
		{
			var first = self;

			for (int i = 1; i < pow; i++)
			{
				self *= first; 
			}

			return self;
		}


		public static int Lerp(int min, int max, float t)
		{
			t = Mathf.Clamp01(t);

			var v = max - min;

			var per = 1f / v;

			return Mathf.RoundToInt(t / per);
		}
#endregion

#region ulong
		public static ulong Pow(this ulong self, int pow)
		{
			var first = self;

			for (int i = 1; i < pow; i++)
			{
				self *= first; 
			}

			return self;
		}
#endregion

#region string
		/// <summary>
		/// CamelCase => camel_case
		/// </summary>
		/// <returns></returns>
		public static string ToSnakeCase(this string str)
		{
			var regex = new System.Text.RegularExpressions.Regex("[a-z][A-Z]");
			return regex.Replace(str, s => $"{s.Groups[0].Value[0]}_{s.Groups[0].Value[1]}").ToLower();
		}

		/// <summary>
		/// TextMesh Proで使う
		/// </summary>
		/// <param name="spriteName"></param>
		/// <returns></returns>
		public static string ToTMPSpriteName(this string spriteName)
		{
			return string.Format("<sprite name=\"{0}\">", spriteName);
		}
#endregion

#region 多次元配列
		public static bool IsInBounds<T>(this T[,] self, Vector2Int p)
		{
			return self.IsInBounds(p.x, p.y);
		}

		public static bool IsInBounds<T>(this T[,] self, int x, int y)
		{
			if (x < 0 || self.GetLength(0) <= x || y < 0 || self.GetLength(1) <= y)
			{
				return false;
			}
			
			return true;
		}

		public static IEnumerable<T> GetEnumerable<T>(this T[,] self)
		{
			for (var x = 0; x < self.GetLength(0); x++)
			{
				for (var y = 0; y < self.GetLength(1); y++)
				{
					yield return self[x, y];
				}
			}
		}
#endregion

#region Vector2Int

		public static readonly Vector2Int[] Vector2Directions = new Vector2Int[]
		{
			Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
		};

		public static void Foreach(this Vector2Int self, System.Action<int, int> function)
		{
			for (var x = 0; x < self.x; x++)
			{
				for (var y = 0; y < self.y; y++)
				{
					function(x, y);
				}
			}
		}

		public static void Foreach(this Vector2Int self, System.Action<Vector2Int> function)
		{
			self.Foreach((x, y) => function(new Vector2Int(x, y)));
		}
		
		public static HashSet<Vector2Int> CreateHashSet(this Vector2Int self)
		{
			var hashset = new HashSet<Vector2Int>();

			self.Foreach((x, y) =>
			{
				hashset.Add(new Vector2Int(x, y));
			});
			
			return hashset;
		}

		/// <summary>
		/// 二次元座標の中の特定の位置の東西南北方向の座標の列を返す。添字はVector2.方向
		/// </summary>
		public static Dictionary<Vector2Int, List<Vector2Int>> Get4DirectionFromPoint(this Vector2Int self, Vector2Int point)
		{
			var dict = new Dictionary<Vector2Int, List<Vector2Int>>();

			for (var i = 0; i < Vector2Directions.Length; i++)
			{
				var current = point;
				var d = Vector2Directions[i];
				
				current += d;

				while (self.IsInBounds(current))
				{
					dict.NewAndAddList(d, current);
					current += d;
				}
			}

			return dict;
		}

		public static bool IsInBounds(this Vector2Int self, Vector2Int point)
		{
			if (point.x < 0 || self.x <= point.x || point.y < 0 || self.y <= point.y)
			{
				return false;
			}
			
			return true;
		}

#endregion

		public static float GetAngleXZ(Vector3 start, Vector3 target)
		{
			return GetAngle(new Vector2(start.x, start.z), new Vector2(target.x, target.z));
		}

		// https://t-stove-k.hatenablog.com/entry/2018/08/28/164549
		public static float GetAngle(Vector2 start, Vector2 target)
		{
			Vector2 dt = target - start;
			float rad = Mathf.Atan2(dt.x, dt.y);
			float degree = rad * Mathf.Rad2Deg;
			
			if (degree < 0) 
			{
				degree += 360;
			}
			
			return degree;
		}

		// https://setchi.hatenablog.com/entry/2017/07/12/202756
		/// <summary>
		/// 線分同士の交点を求める
		/// </summary>
		/// <param name="intersection">交差する位置</param>
		/// <returns>交差の有無</returns>
		public static bool LineSegmentsIntersection(
			Vector2 p1,
			Vector2 p2,
			Vector2 p3,
			Vector2 p4,
			out Vector2 intersection)
		{
			intersection = Vector2.zero;

			var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

			if (d == 0.0f)
			{
				return false;
			}

			var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
			var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

			if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
			{
				return false;
			}

			intersection.x = p1.x + u * (p2.x - p1.x);
			intersection.y = p1.y + u * (p2.y - p1.y);

			return true;
		}

		/// <summary>
		/// 線分同士の交点を求める
		/// </summary>
		/// <param name="intersection">交差する位置</param>
		/// <returns>交差の有無</returns>
		public static bool LineSegmentsIntersectionXZ(
			Vector3 p1,
			Vector3 p2,
			Vector3 p3,
			Vector3 p4,
			out Vector2 intersection)
		{
			return LineSegmentsIntersection(
				new Vector2(p1.x, p1.z),
				new Vector2(p2.x, p2.z),
				new Vector2(p3.x, p3.z),
				new Vector2(p4.x, p4.z),
				out intersection
			);
		}
		
		/// <summary>
		/// ベクトルABがあったとして、そこに点Pから下ろした垂線との交点を求めたい時に使う
		/// https://sleepygamersmemo.blogspot.com/2019/03/perpendicular-foot-point.html
		/// </summary>
		/// <returns></returns>
		public static Vector3 PerpendicularFootPoint(Vector3 a, Vector3 b, Vector3 p)
		{
			return a + Vector3.Project(p - a, b - a);
		}

		/// <summary>
		///  点Pから最も近い線分AB上にある点を返す
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public static Vector3 NearestPointOnLineSegment(Vector3 a, Vector3 b, Vector3 p)
		{
			Vector3 ab = b - a;
			float length = ab.magnitude;
			ab.Normalize();

			float k = Vector3.Dot(p - a, ab);
			k = Mathf.Clamp(k, 0, length);
			return a + k * ab;
		}

#region Path関係
		/// <summary>
		/// パスの中から位置を求める
		/// </summary>
		/// <param name="path"></param>
		/// <param name="normalized">0~1</param>
		/// <returns></returns>
		public static Vector3 GetPointOfProgress(this IList<Vector3> path, float normalized)
		{
			normalized = Mathf.Clamp01(normalized);

			var totalLength = path.GetTotalLength();

			var lengthOnProgress = totalLength * normalized;

			var currentLength = 0f;

			for (int i = 0; i < path.Count - 1; i++)
			{
				var head = path[i];
				var tail = path[i + 1];
				
				var distance = Vector3.Distance(head, tail);

				if (lengthOnProgress <= currentLength + distance)
				{
					var myLength = lengthOnProgress - currentLength;

					var myProgress = myLength / distance;

					return Vector3.Lerp(head, tail, myProgress);
				}

				currentLength += distance;
			}

			return path[path.Count - 1];
		}

		/// <summary>
		/// パスの長さを求める
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static float GetTotalLength(this IList<Vector3> path)
		{
			var length = 0f;

			for (int i = 0; i < path.Count - 1; i++)
			{
				var head = path[i];
				var tail = path[i + 1];

				length += Vector3.Distance(head, tail);
			}

			return length;
		}
		
		/// <summary>
		/// パスの中から最も点に近い位置を求める
		/// </summary>
		/// <param name="path"></param>
		/// <param name="point"></param>
		/// <param name="ratio"></param>
		/// <returns></returns>
		public static Vector3 GetPointOnPath(this IList<Vector3> path, Vector3 point, out float ratio)
		{
			var result = point;

			var totalLength = 0f;

			var distanceByStart = 0f;

			for (int i = 0;i < path.Count - 1; i++)
			{
				var start = path[i];
				var end = path[i + 1];

				var p = PerpendicularFootPoint(start, end, point);

				var maxDistance = Vector3.Distance(start, end);
				var distance = Vector3.Distance(point, p);

				if (i == 0 || distance < Vector3.Distance(point, result))
				{
					var pDistance = Vector3.Distance(start, p);
					distanceByStart = totalLength + pDistance;
					result = p;
				}

				totalLength += maxDistance;
			}

			ratio = distanceByStart / totalLength;

			// Debug.Log("ratio:" + ratio + " d:" + distanceByStart + " tl:" + totalLength);

			return result;
		}

#endregion
		
		public static Vector3 ToX(this Vector3 self, float x)
		{
			self.x = x;
			return self;
		}

		public static Vector3 ToY(this Vector3 self, float y)
		{
			self.y = y;
			return self;
		}

		public static Vector3 ToZ(this Vector3 self, float z)
		{
			self.z = z;
			return self;
		}
		
		/// <summary>
		/// ワールド座標上に対するキャンバス座標にTransformの位置を設定する。HPゲージなどのオーバレイ表示に向いてる。
		/// </summary>
		/// <param name="transform"></param>
		/// <param name="worldPosition"></param>
		public static  void AttachOnCanvas(this Transform transform, Vector3 worldPosition)
		{
			var sp = Camera.main.WorldToScreenPoint(worldPosition);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform, sp, null, out var localPosition);
			transform.localPosition = localPosition;
		}

#region TrajectoryCalculate
		
		// http://takashicompany.hatenablog.com/entry/2015/03/06/200454

		/// <summary>
		/// Trajectory calculate.
		/// </summary>
		public static class TrajectoryCalculate
		{
			/// <summary>
			/// Calculates the trajectory by force.
			/// </summary>
			/// <returns>The trajectory position at time.</returns>
			/// <param name="start">Start Position.</param>
			/// <param name="force">Force.(e.g. rigidbody.AddRelativeForce(force))</param>
			/// <param name="mass">Mass.(e.g. rigidbody.mass)</param>
			/// <param name="gravity">Gravity.(e.g. Physics.gravity)</param>
			/// <param name="gravityScale">Gravity scale.(e.g. rigidbody2D.gravityScale)</param>
			/// <param name="time">Time.</param>
			public static Vector3 Force(
				Vector3 start,
				Vector3 force,
				float mass,
				Vector3 gravity,
				float gravityScale,
				float time
			)
			{
				var speedX = force.x / mass * Time.fixedDeltaTime;
				var speedY = force.y / mass * Time.fixedDeltaTime;
				var speedZ = force.z / mass * Time.fixedDeltaTime;

				var halfGravityX = gravity.x * 0.5f * gravityScale;
				var halfGravityY = gravity.y * 0.5f * gravityScale;
				var halfGravityZ = gravity.z * 0.5f * gravityScale;

				var positionX = speedX * time + halfGravityX * Mathf.Pow(time, 2);
				var positionY = speedY * time + halfGravityY * Mathf.Pow(time, 2);
				var positionZ = speedZ * time + halfGravityZ * Mathf.Pow(time, 2);

				return start + new Vector3(positionX, positionY, positionZ);
			}

			/// <summary>
			/// Calculates the trajectory by velocity.
			/// </summary>
			/// <returns>The trajectory position at time.</returns>
			/// <param name="start">Start Position.</param>
			/// <param name="velocity">Velocity.(e.g. rigidbody.velocity)</param>
			/// <param name="gravity">Gravity.(e.g. Physics.gravity)</param>
			/// <param name="gravityScale">Gravity scale.(e.g. rigidbody2D.gravityScale)</param>
			/// <param name="time">Time.</param>
			public static Vector3 Velocity(
				Vector3 start,
				Vector3 velocity,
				Vector3 gravity,
				float gravityScale,
				float time
			)
			{
				var halfGravityX = gravity.x * 0.5f * gravityScale;
				var halfGravityY = gravity.y * 0.5f * gravityScale;
				var halfGravityZ = gravity.z * 0.5f * gravityScale;

				var positionX = velocity.x * time + halfGravityX * Mathf.Pow(time, 2);
				var positionY = velocity.y * time + halfGravityY * Mathf.Pow(time, 2);
				var positionZ = velocity.z * time + halfGravityZ * Mathf.Pow(time, 2);

				return start + new Vector3(positionX, positionY, positionZ);
			}
		}
#endregion

#region 色関係
		public static Color GetRandomColor()
		{
			return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1), 1);
		}
#endregion

		public static bool Include(this Vector3Int self, Vector3Int target)
		{
			return 0 <= target.x && target.x < self.x && 0 <= target.y && target.y < self.y && 0 <= target.z && target.z < self.z;
		}

		public static IEnumerable<KeyValuePair<Vector3Int, T>> GetEnumerable<T>(this T[,,] self)
		{
			for (var x = 0; x < self.GetLength(0); x++)
			{
				for (var y = 0; y < self.GetLength(1); y++)
				{
					for (var z = 0; z < self.GetLength(2); z++)
					{
						yield return new KeyValuePair<Vector3Int, T>(new Vector3Int(x, y, z), self[x, y, z]);
					}
				}
			}
		}

		public static void Foreach(this Vector3Int self, System.Action<int, int, int> function)
		{
			for (var x = 0; x < self.x; x++)
			{
				for (var y = 0; y < self.y; y++)
				{
					for (var z = 0; z < self.z; z++)
					{
						function(x, y, z);
					}
				}
			}
		}

		public static void Foreach(this Vector3Int self, System.Action<Vector3Int> function)
		{
			self.Foreach((x, y, z) => function(new Vector3Int(x, y, z)));
		}

		

		public static void Foreach(this BoundsInt b, System.Action<Vector3Int> function, bool includeMax = false)
		{
			for (var x = b.xMin; includeMax ? x <= b.xMax : x < b.xMax; x++)
			{
				for (var y = b.yMin; includeMax ? y <= b.yMax : x < b.yMax; y++)
				{
					for (var z = b.zMin; includeMax ? z <= b.zMax : z < b.zMax; z++)
					{
						function?.Invoke(new Vector3Int(x, y, z));
					}
				}
			}
		}

		public static IEnumerable<Vector3Int> GetV3Ints(this BoundsInt b, bool includeMax = false)
		{
			for (var x = b.xMin; includeMax ? x <= b.xMax : x < b.xMax; x++)
			{
				for (var y = b.yMin; includeMax ? y <= b.yMax : x < b.yMax; y++)
				{
					for (var z = b.zMin; includeMax ? z <= b.zMax : z < b.zMax; z++)
					{
						yield return new Vector3Int(x, y, z);
					}
				}
			}
		}

		// public static void GetPositionOnGrids(Vector3Int gridSize, Vector3 unitPerGrid, out Vector3[,,] centerPositions, out Vector3[,,] crossPositions)
		// {
		// 	var crossGridSize = new Vector3Int(gridSize.x + 1, gridSize.y + 1, gridSize.z + 1);
		// 	var centers = new Vector3[gridSize.x, gridSize.y, gridSize.z];
		// 	var crosses = new Vector3[crossGridSize.x, crossGridSize.y, crossGridSize.z];
			

		// 	var endX = gridSize.x - 1;
		// 	var endY = gridSize.y - 1;
		// 	var endZ = gridSize.z - 1;

		// 	gridSize.Foreach(v3int => 
		// 	{
		// 		var center = GetPositionOnGrid(gridSize, v3int, unitPerGrid);

		// 		centers[v3int.x, v3int.y, v3int.z] = center;

		// 		(Vector3Int.one * 2).Foreach((x, y, z) =>
		// 		{
		// 			var myX = x == 0 ? -1 : 1;
		// 			var myY = y == 0 ? -1 : 1;
		// 			var myZ = z == 0 ? -1 : 1;

		// 			var c = new Vector3Int(v3int.x + x, v3int.y + y, v3int.z + z);

		// 			if (crossGridSize.Include(c))
		// 			{
		// 				crosses[x, y, 
		// 			}
		// 		});

		// 		crosses[v3int.x, v3int.y, v3int.z] = center - unitPerGrid / 2;
				

		// 		var isEndX = v3int.x == endX;
		// 		var isEndY = v3int.y == endY;
		// 		var isEndZ = v3int.z == endZ;

		// 		if (isEndX || isEndY|| isEndZ)
		// 		{
		// 			var nextX = isEndX ? gridSize.x : v3int.x;
		// 			var nextY = isEndY ? gridSize.y : v3int.y;
		// 			var nextZ = isEndZ ? gridSize.z : v3int.z;

		// 			var offsetX = isEndX ? unitPerGrid.x / 2 : 0;
		// 			var offsetY = isEndY ? unitPerGrid.y / 2 : 0;
		// 			var offsetZ = isEndZ ? unitPerGrid.z / 2 : 0;

		// 			crosses[nextX, nextY, nextZ] = center + 
		// 		}
		// 	});
		// }

		public static Vector2 GetPositionOnGrid(Vector2Int gridSize, Vector2Int gridPosition, Vector2 unitPerGrid)
		{
			return GetPositionOnGrid(gridSize.ToV3Int(), gridPosition.ToV3Int(), unitPerGrid);
		}

		public static Vector3 GetPositionOnGrid(Vector3Int gridSize, Vector3Int gridPosition, Vector3 unitPerGrid)
		{
			var half = unitPerGrid / 2;

			var start = new Vector3(
				-half.x * Mathf.Max(gridSize.x - 1, 0),
				-half.y * Mathf.Max(gridSize.y - 1, 0),
				-half.z * Mathf.Max(gridSize.z - 1, 0)
			);
			
			return start + new Vector3(unitPerGrid.x * gridPosition.x, unitPerGrid.y * gridPosition.y, unitPerGrid.z * gridPosition.z);
		}

		public static Vector3Int GetGridPosition(Vector3Int gridSize, Vector3 unitPerGrid, Vector3 position)
		{
			// マスあたりのサイズ x マス数で大きさを出す
			var size = new Vector3(unitPerGrid.x * gridSize.x, unitPerGrid.y * gridSize.y, unitPerGrid.z * gridSize.z);

			// 原点の座標を出す
			var origin = size / -2;

			// 原点から見た時の距離
			var p = position - origin;

			 return new Vector3Int((int)Mathf.Floor(p.x / unitPerGrid.x), (int)Mathf.Floor(p.y / unitPerGrid.y), (int)Mathf.Floor(p.z / unitPerGrid.z));
		}

		/// <summary>
		/// 円周上の位置を返す関数
		/// </summary>
		/// <param name="pointCount">取得したい円周上の座標の数</param>
		/// <param name="start">始点のOffset。0~1</param>
		/// <param name="cycle">周期</param>
		/// <returns></returns>
		public static Vector2[] GetPositionOnCircle(int pointCount, float start = 0, float cycle = 1f)
		{
			var points = new List<Vector2>();

			var rad2 = Mathf.PI * 2;

			var perAngle = rad2 * cycle / (pointCount - 1);		// -1することで最後の点が始点と重なるようにしている

			for (int i = 0; i < pointCount; i++)
			{
				var x = Mathf.Cos(rad2 * start + perAngle * i);
				var y = -Mathf.Sin(rad2 * start + perAngle * i);

				points.Add(new Vector2(x, y));
			}

			return points.ToArray();
		}

		public static IEnumerable<T> GetEnumerableByFlag<T>(this T self, bool includeZero = false) where T : System.Enum
		{
			foreach (T e in System.Enum.GetValues(typeof(T)))
			{
				if (!includeZero)
				{
					IConvertible convertible = e;
					var num = convertible.ToInt32(null);

					if (num == 0)
					{
						continue;
					}
				}
				
				if (self.HasFlag(e))
				{
					yield return e;
				}
			}
		}
		
#region リストをランダムで処理する
		public static int GetRandomIndex<T>(this IList<T> self)
		{
			return UnityEngine.Random.Range(0, self.Count);
		}

		public static int GetRandomIndex(this IList self)
		{
			return UnityEngine.Random.Range(0, self.Count);
		}

		public static T GetRandom<T>(this IList self)
		{
			return (T)(self[self.GetRandomIndex()]);
		}

		public static T GetRandom<T>(this IList<T> self)
		{
			return self[self.GetRandomIndex()];
		}

		public static T PickRandom<T>(this IList<T> self)
		{
			var index = self.GetRandomIndex();

			var item = self[index];

			self.RemoveAt(index);

			return item;
		}

		public static List<T> GetRandomSorted<T>(this IList<T> self)
		{
			var myList = new List<T>(self);

			var list = new List<T>();

			while(myList.Count > 0)
			{
				list.Add(myList.PickRandom());
			}

			return list;
		}

		public static T[] GetRandom<T>(this IList<T> self, int count)
		{
			var result = new T[count];
			
			for (int i = 0; i < count; i++)
			{
				result[i] = self.GetRandom();
			}

			return result;
		}
#endregion

#region 配列

		public static bool TryGet<T>(this IList<T> self, int index, out T result)
		{
			if (index < 0 || self.Count <= index)
			{
				result = default(T);
				return false;
			}

			result = self[index];
			return true;
		}

#endregion

		public static int IndexOf<T>(this IList<T> self, T item)
		{
			return self.IndexOf(item);
		}

		public static IEnumerable<T> ToEnumerable<T>() where T : System.Enum
		{
			// 実装的に同じらしい
			// foreach (T t in System.Enum.GetValues(typeof(T)))
			// {
			// 	yield return t;
			// }

			return from T t in System.Enum.GetValues(typeof(T)) select t;
		}

		public static T[] ToArray<T>() where T : System.Enum
		{
			// https://kaz-dora.com/2020/07/20/line-and-enum/ を参考にした
			return System.Enum.GetValues(typeof(T)).OfType<T>().ToArray();

			// var result = new List<T>();

			// foreach (T t in System.Enum.GetValues(typeof(T)))
			// {
			// 	result.Add(t);
			// }

			// return result.ToArray();
		}

		public static T PickRandom<T>() where T : System.Enum
		{
			var array = System.Enum.GetValues(typeof(T));

			return array.GetRandom<T>();
		}

		public static T[] PickRandom<T>(int count) where T : System.Enum
		{
			var list = new List<T>();

			foreach (T t in System.Enum.GetValues(typeof(T)))
			{
				list.Add(t);
			}

			var result = new T[count];

			for (int i = 0; i < count; i++)
			{
				result[i] = list.GetRandom();
			}

			return result;
		}

#region Transform
		public static void ToX(this Transform self, float x)
		{
			var p = self.position;
			p.x = x;
			self.position = p;
		}

		public static void ToY(this Transform self, float y)
		{
			var p = self.position;
			p.y = y;
			self.position = p;
		}

		public static void ToZ(this Transform self, float z)
		{
			var p = self.position;
			p.z = z;
			self.position = p;
		}

		public static void ToLocalX(this Transform self, float x)
		{
			var p = self.localPosition;
			p.x = x;
			self.localPosition = p;
		}

		public static void ToLocalY(this Transform self, float y)
		{
			var p = self.localPosition;
			p.y = y;
			self.localPosition = p;
		}

		public static void ToLocalZ(this Transform self, float z)
		{
			var p = self.localPosition;
			p.z = z;
			self.localPosition = p;
		}

		public static string GetNameWithHierarchy(this Transform self)
		{
			var str = self.name;

			var current = self.parent;
			
			while (current != null)
			{
				str = current.name + "/" + str;
				current = current.parent;
			}

			return str;
		}

#endregion

#region  RectTransform

		/// <summary>
		/// アンカー側のマージンを基準にサイズを0 ~ 1で変化させる。左右で異なるマージンを設定する使い方はできない
		/// pivot.xは0じゃないと正しく動かないです。つまり左側ゲージのみ
		/// </summary>
		public static void SimpleHorizontalGauge(this RectTransform self, float normalizedX)
		{
			var parent = self.parent as RectTransform;
			
			var width = parent.rect.width - Mathf.Lerp(self.offsetMin.x, self.offsetMax.x, self.pivot.x) * 2;

			self.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * normalizedX);
		}
#endregion

		public static T GetComponentSelfOrInParent<T>(this Component self) where T : Component
		{
			return self.gameObject.GetComponentSelfOrInParent<T>();
		}

		public static T GetComponentSelfOrInParent<T>(this GameObject self) where T : Component
		{
			var result = self.GetComponent<T>();

			if (result != null)
			{
				return result;
			}

			return self.GetComponentInParent<T>();
		}

		public static List<T> GetComponentsSelfAndChildren<T>(this Component self) where T : Component
		{
			var list = new List<T>();

			list.AddRange(self.GetComponents<T>());
			list.AddRange(self.GetComponentsInChildren<T>(true));

			return list;
		}

#region GameObject
		public static List<T> GetComponentsSelfAndChildren<T>(this GameObject self) where T : Component
		{
			var list = new List<T>();

			list.AddRange(self.GetComponents<T>());
			list.AddRange(self.GetComponentsInChildren<T>(true));

			return list;
		}

		public static void ChangeLayerWithChildren(this GameObject self, int layer)
		{
			self.layer = layer;
			
			foreach (Transform n in self.transform)
			{
				ChangeLayerWithChildren(n.gameObject, layer);
			}
		}

		/// <summary>
		/// 現在のアクティブ状況と異なる場合にアクティブを切り替える
		/// </summary>
		public static void SetActiveIfNot(this GameObject self, bool active)
		{
			if (self.activeSelf != active)
			{
				self.SetActive(active);
			}
		}
#endregion

		public static Color SetAlpha(this Color self, float alpha)
		{
			self.a = alpha;
			return self;
		}

		public static void SetAlpha(this MaskableGraphic self, float alpha)
		{
			var c = self.color;
			c.a = alpha;
			self.color = c;
		}

		public static void SetAlpha(this SpriteRenderer self, float alpha)
		{
			var c = self.color;
			c.a = alpha;
			self.color = c;
		}

		public static void SetAlpha(this Material self, float alpha)
		{
			self.color = self.color.SetAlpha(alpha);
			var c = self.color;
			c.a = alpha;
			self.color = c;
		}

		public static Vector2 CalcNormalizedPosition(this PointerEventData self)
		{
			return self.position / new Vector2(Screen.width, Screen.height);
		}

		public static Vector2 CalcPressNormalizedPosition(this PointerEventData self)
		{
			return self.pressPosition / new Vector2(Screen.width, Screen.height);
		}

		public static Vector3 CalcNormalizedMovement(this PointerEventData self)
		{
			return (self.position - self.pressPosition) / new Vector2(Screen.width, Screen.height);
		}

		public static Vector2 CalcDeltaNormalized(this PointerEventData self)
		{
			return self.delta / new Vector2(Screen.width, Screen.height);
		}

		public static void Kill(ref Tweener tweener)
		{
			if (tweener != null && tweener.IsPlaying())
			{
				tweener.Kill();
			}

			tweener = null;
		}

		public static void Kill(ref Sequence seq)
		{
			if (seq != null && seq.IsPlaying())
			{
				seq.Kill();
			}

			seq = null;
		}

		public static Sequence KillAndNew(ref Sequence seq)
		{
			if (seq != null && seq.IsPlaying())
			{
				seq.Kill();
			}

			seq = DOTween.Sequence();

			return seq;
		}

		public static Tweener DOEmmison(this Material self, Color color, float duration)
		{
			var c = self.GetEmmisionColor();
			return DOTween.To(() => c, v => c = v, color, duration).OnUpdate(() =>
			{
				self.SetEmmisionColor(c);
			});
		}

		public static Color GetEmmisionColor(this Material self)
		{
			return self.GetColor("_EmissionColor");
		}

		public static void SetEmmisionColor(this Material self, Color color)
		{
			self.EnableKeyword("_EMISSION");
			self.SetColor("_EmissionColor", color);
		}

		public static Tweener DOBaseColor(this Material self, Color color, float duration)
		{
			var c = self.GetBaseColor();
			return DOTween.To(() => c, v => c = v, color, duration).OnUpdate(() =>
			{
				self.SetBaseColor(c);
			});
		}

		public static Color GetBaseColor(this Material self)
		{
			return self.GetColor("_BaseColor");
		}

		public static void SetBaseColor(this Material self, Color color)
		{
			self.SetColor("_BaseColor", color);
		}

		public static Tweener DOForward(this Transform self, Vector3 forward, float duration)
		{
			var current = self.forward;

			return DOTween.To(() => current, v => current = v.normalized, forward, duration).OnUpdate(() =>
			{
				self.forward = current;
			});
		}

		public static Tweener DOScale(this LineRenderer self, Vector3 center, float end, float duration)
		{
			var froms = new Vector3[self.positionCount];
			self.GetPositions(froms);

			var current = 1f;

			return DOTween.To(() => current, v => current = v, end, duration).OnUpdate(() =>
			{
				for (int i = 0; i < froms.Length; i++)
				{
					var from = froms[i];
					self.SetPosition(i, Vector3.LerpUnclamped(center, from, current));
				}
			});
		}

		public static Tweener DOScale(this IList<Vector3> self, Vector3 center, float end, float duration)
		{
			var froms = self.ToArray();
			var current = 1f;
			return DOTween.To(() => current, v => current = v, end, duration).OnUpdate(() =>
			{
				for (int i = 0; i < self.Count; i++)
				{
					var from = froms[i];
					self[i] = Vector3.LerpUnclamped(center, from, current);
				}
			});
		}
#region Dictionary
		/// <summary>
		/// Dictionary<K, V>からDictionay<V, K>を生成する。
		/// Valueがnullだったり値が被る場合は除外
		/// </summary>
		public static Dictionary<V, K> BuildReverse<K, V>(this Dictionary<K, V> self)
		{
			var dict = new Dictionary<V, K>();

			foreach (var kvp in self)
			{
				if (kvp.Value != null && !dict.ContainsKey(kvp.Value))
				{
					dict.Add(kvp.Value, kvp.Key);
				}
			}

			return dict;
		}

		public static void NewAndAddList<K, V>(this Dictionary<K, List<V>> self, K key, V val)
		{
			if (!self.ContainsKey(key))
			{
				self.Add(key, new List<V>());
			}

			self[key].Add(val);
		}

		public static void Foreach<K, V>(this Dictionary<K, V> self, System.Action<K, V> function)
		{
			foreach (var kvp in self)
			{
				function?.Invoke(kvp.Key, kvp.Value);
			}
		}

		public static K GetRandomKey<K, V>(this Dictionary<K, V> self)
		{
			return self.Keys.ToList().GetRandom();
		}

		public static V GetRandomValue<K, V>(this Dictionary<K, V> self)
		{			
			return self[self.GetRandomKey()];
		}

		public static void AddOrSet<K, V>(this IDictionary<K, V> self, K key, V value)
		{
			if (self.ContainsKey(key))
			{
				self[key] = value;
			}
			else
			{
				self.Add(key, value);
			}
		}

		public static bool TryAdd<K, V>(this IDictionary<K, V> self, K key, V value)
		{
			if (!self.ContainsKey(key))
			{
				self.Add(key, value);
				return true;
			}

			return false;
		}

		/// <summary>
		/// IDictionary<K, int>型で足し算を行う
		/// </summary>
		public static int Sum<K>(this IDictionary<K, int> self, K key, int value)
		{
			if (self.ContainsKey(key))
			{
				self[key] += value; 
			}
			else
			{
				self.Add(key, value);
			}

			return self[key];
		}
#endregion

		public static IEnumerable<T> FindAbove<T>(this Collider self, float height, int layerMask, QueryTriggerInteraction queryTriggerInteraction= QueryTriggerInteraction.UseGlobal)
		{
			return Physics.OverlapBox(self.bounds.center + Vector3.up * self.bounds.size.y, self.bounds.extents, self.transform.rotation, layerMask, queryTriggerInteraction).Select(c => c.GetComponent<T>());
		}

		public static Collider[] Overlap(this BoxCollider self, int layerMask, QueryTriggerInteraction queryTriggerInteraction= QueryTriggerInteraction.UseGlobal)
		{
			return Physics.OverlapBox(self.transform.position, self.size / 2, self.transform.rotation, layerMask, queryTriggerInteraction);
		}

		public static Bounds Transform(this Bounds self, Transform transform)
		{
			var center = transform.TransformPoint(self.center);
			var size = new Vector3( // transform.TransformVector(self.size); 回転した時にサイズが変わるので自分で計算する
						self.size.x * transform.lossyScale.x,
						self.size.y * transform.lossyScale.y,
						self.size.z * transform.lossyScale.z
			);

			return new Bounds(center, size);
		}

		public static Vector2 RandomVector2(float min, float max)
		{
			return (Vector2)RandomVector3(min, max);
		}

#region Vector3
		public static Vector3 RandomVector3(float min, float max)
		{
			return RandomVector3(new Vector3(min, min, min), new Vector3(max, max, max));
		}

		public static Vector3 RandomVector3(Vector3 min, Vector3 max)
		{
			return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
		}

		public static Transform GetClosest(this Vector3 worldPoint, params Transform[] transforms)
		{
			return transforms.OrderBy(t => Vector3.Distance(worldPoint, t.position)).FirstOrDefault();
		}

#endregion

#region Vector2Int
		public static Vector2Int RandomVector2Int(Vector2Int min, Vector2Int max)
		{
			return (Vector2Int)RandomVector3Int(((Vector3Int)min), ((Vector3Int)max));
		}

		public static Vector2Int GetRandom(this Vector2Int max)
		{
			return RandomVector2Int(Vector2Int.zero, max);
		}
#endregion

#region Vector3Int
		public static Vector3Int RandomVector3Int(Vector3Int min, Vector3Int max)
		{
			return new Vector3Int(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
		}

		public static Vector3Int GetRandom(this Vector3Int max)
		{
			return RandomVector3Int(Vector3Int.zero, max);
		}
#endregion

		public static Quaternion RandomRotation()
		{
			return Quaternion.Euler(RandomVector3(0, 360));
		}

		public static Quaternion RandomRotation(Vector3 min, Vector3 max)
		{
			return Quaternion.Euler(RandomVector3(min, max));
		}


		public static Vector3 RandomPoint(this Bounds bounds)
		{
			return bounds.RandomPoint(Vector3.zero);
		}

		public static Vector3 RandomPoint(this Bounds bounds, Vector3 excludeFromEdge)
		{
			var x = Random.Range(bounds.min.x + excludeFromEdge.x, bounds.max.x - excludeFromEdge.x);
			var y = Random.Range(bounds.min.y + excludeFromEdge.y, bounds.max.y - excludeFromEdge.y);
			var z = Random.Range(bounds.min.z + excludeFromEdge.z, bounds.max.z - excludeFromEdge.z);

			return new Vector3(x, y, z);
		}

		public static Bounds GetBounds(this IEnumerable<Vector3> points)
		{
			var minX = float.MaxValue;
			var minY = float.MaxValue;
			var minZ = float.MaxValue;

			var maxX = float.MinValue;
			var maxY = float.MinValue;
			var maxZ = float.MinValue;

			foreach (var p in points)
			{
				if (minX > p.x) minX = p.x;
				if (minY > p.y) minY = p.y;
				if (minZ > p.z) minZ = p.z;

				if (maxX < p.x) maxX = p.x;
				if (maxY < p.y) maxY = p.y;
				if (maxZ < p.z) maxZ = p.z;
			}

			var size = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
			var center = new Vector3(minX + size.x / 2, minY + size.y / 2, minZ + size.z / 2);

			return new Bounds(center, size);
		}

		public static Bounds GetBounds(this IEnumerable<Rect> rects)
		{
			var points = new List<Vector3>();

			foreach (var r in rects)
			{
				points.Add(r.min);
				points.Add(r.max);
			}

			return points.GetBounds();
		}

		public static Bounds GetBounds(this IEnumerable<Bounds> self)
		{
			var points = new List<Vector3>();

			foreach (var b in self)
			{
				points.Add(b.min);
				points.Add(b.max);
			}

			return points.GetBounds();
		}

		public static Bounds GetBounds(this Rect rect)
		{
			return new Bounds(rect.center, rect.size);
		}

		public static string ToArrayStr<T>(this IList<T> list)
		{
			var str = "";

			for (int i = 0; i < list.Count; i++)
			{
				str += list[i].ToString();
				
				if (i - 1 < list.Count)
				{
					str += "\n";
				}
			}

			return str;
		}

		/// <summary>
		/// クラス名でResources.Loadをする
		/// </summary>
		public static T LoadPrefab<T>() where T : UnityEngine.Object
		{
			return Resources.Load<T>(typeof(T).Name);
		}

		public static void DrawGizmosWireCubeWithRotate(Vector3 center, Quaternion rotation, Vector3 size)
		{
			var matrix = Gizmos.matrix;
			Gizmos.matrix = Matrix4x4.TRS(center, rotation, size);
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
			Gizmos.matrix = matrix;
		}

		public static void DrawWireFrame(this Vector3[] points)
		{
			for (int i = 0; i < points.Length; i++)
			{
				var current = points[i];
				var next = i == points.Length - 1 ? points[0] : points[i + 1];

				Gizmos.DrawLine(current, next);
			}
		}

		public static void DrawGizmosCross(Bounds bounds)
		{
			Gizmos.DrawLine(bounds.min, bounds.max);
			var a = new Vector3(bounds.min.x, bounds.max.y,bounds.min.z);
			var b = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
			Gizmos.DrawCube(a, b);
		}

		public static void DrawGizmosPoint(Vector3 point, float extents = 0.5f)
		{
			Gizmos.DrawLine(point + Vector3.down * extents, point + Vector3.up * extents);
			Gizmos.DrawLine(point + Vector3.left * extents, point + Vector3.right * extents);
			Gizmos.DrawLine(point +	Vector3.back * extents, point + Vector3.forward * extents);
		}

#region Animator
		/// <summary>
		/// AnimatorのHumanoidから名前で対応したTransformを取得する
		/// </summary>
		/// <param name="self"></param>
		/// <param name="humanName">https://docs.unity3d.com/ja/current/ScriptReference/HumanBone.html に書いてある</param>
		[System.Obsolete("Animator.GetBoneTransformに同じ機能の関数があったよ")]
		public static Transform GetHumanTransform(this Animator self, string humanName)
		{
			foreach (var h in self.avatar.humanDescription.human)
			{
				if (h.humanName == humanName)
				{
					var transforms = self.GetComponentsInChildren<Transform>(true);
					
					foreach (var t in transforms)
					{
						if (t.name == h.boneName)
						{
							return t;
						}
					}
				}
			}

			return null;
		}

		public static void PlayAllLayer(this Animator self, string stateName, int layer = -1, float normalizedTime = float.NegativeInfinity)
		{
			for (int i = 0; i < self.layerCount; i++)
			{
				self.Play(stateName, i, normalizedTime);
			}
		}

		public static void CrossFadeAllLayer(this Animator self, string stateName, float normalizedTransitionDuration, float normalizedTimeOffset = float.NegativeInfinity, float normalizedTransitionTime = 0.0f)
		{
			for (int i = 0; i < self.layerCount; i++)
			{
				self.CrossFade(stateName, normalizedTransitionDuration, i, normalizedTimeOffset, normalizedTransitionTime);
			}
		}

		/// <summary>
		/// 特定のStateの再生完了を待っているか。
		/// この関数を判定に使う場合、Animator.Playを読んでから1フレーム待たないと、stateNameのstateに入らないことがある
		/// </summary>
		public static bool IsCompleteWaiting(this AnimatorStateInfo self, string stateName)
		{
			return self.IsName(stateName) && self.normalizedTime < 1f;
		}

#endregion

#region Vector2Int

		public static HashSet<Vector2Int> GetBorder(this IEnumerable<Vector2Int> points, bool diagonalCorner = false)
		{
			// HashSet.TryGetValueは.Net４.7.2から使えるらしいのでHashSetはむりぽ https://docs.microsoft.com/ja-jp/dotnet/api/system.collections.generic.hashset-1.trygetvalue?view=net-6.0#system-collections-generic-hashset-1-trygetvalue(-0-0@)
			var map =  new Dictionary<Vector2Int, bool>();	//  new HashSet<Vector2Int>();

			foreach (var point in points)
			{
				map.TryAdd(point, true);
			}

			var result = new HashSet<Vector2Int>();

			foreach (var point in points)
			{
				for (int x = -1; x <= 1; x++)
				{
					for (int y = -1; y <= 1; y++)
					{
						if (x == 0 && y == 0)
						{
							continue;
						}

						var offset = new Vector2Int(x, y);
						if (!diagonalCorner && offset.magnitude >= 2)
						{
							continue;
						}

						if (!map.ContainsKey(point + offset))
						{
							result.Add(point + offset);
						}
					}
				}
			}

			return result;
		}

		
#endregion

		public static Vector3Int ToV3IntXZ(this Vector2Int self)
		{
			return new Vector3Int(self.x, 0, self.y);
		}

		public static Vector3Int ToV3Int(this Vector2Int self)
		{
			return new Vector3Int(self.x, self.y, 0);
		}

		public static Vector2Int XZtoV2Int(this Vector3Int self)
		{
			return new Vector2Int(self.x, self.z);
		}

		public static Vector3 ToXZ(this Vector3 self)
		{
			return new Vector3(self.x, 0, self.y);
		}

		public static Vector2 XZtoV2(this Vector3 self)
		{
			return new Vector2(self.x, self.z);
		}

		public static Vector3 ToV3XZ(this Vector2 self)
		{
			return new Vector3(self.x, 0, self.y);
		}

		public static void Foreach<T>(this T[,] self, System.Action<int, int, T> callback)
		{
			for (var x = 0; x < self.GetLength(0); x++)
			{
				for (var y = 0; y < self.GetLength(1); y++)
				{
					callback(x, y, self[x, y]);
				}
			}
		}

		public static void Foreach<T>(this T[,] self, System.Action<Vector2Int, T> callback)
		{
			self.Foreach((x, y, item) => callback(new Vector2Int(x, y), item));
		}

		public static bool TryGet<T>(this T? self, out T result) where T : struct
		{
			result = self.GetValueOrDefault();
			return self.HasValue;
		}

		/// <summary>
		/// 2次元上の多角形が対象の点を内包しているか(囲んでいるか)を確認する関数
		/// </summary>
		public static bool IsSurrounding(this IList<Vector2> points, Vector2 target)
		{
			// https://www.nttpc.co.jp/technology/number_algorithm.html を参考に実装
			
			var wn = 0;

			for(var i = 0; i < points.Count - 1; i++)
			{
				// 上向きの辺、下向きの辺によって処理が分かれる。
				// 上向きの辺。点Pがy軸方向について、始点と終点の間にある。ただし、終点は含まない。(ルール1)
				if ( (points[i].y <= target.y) && (points[i+1].y > target.y) ) {
					// 辺は点pよりも右側にある。ただし、重ならない。(ルール4)
					// 辺が点pと同じ高さになる位置を特定し、その時のxの値と点pのxの値を比較する。
					var vt = (target.y - points[i].y) / (points[i+1].y - points[i].y);
					if(target.x < (points[i].x + (vt * (points[i+1].x - points[i].x)))){
						++wn;  //ここが重要。上向きの辺と交差した場合は+1
					}
				} 
				// 下向きの辺。点Pがy軸方向について、始点と終点の間にある。ただし、始点は含まない。(ルール2)
				else if ( (points[i].y > target.y) && (points[i+1].y <= target.y) ) {
					// 辺は点pよりも右側にある。ただし、重ならない。(ルール4)
					// 辺が点pと同じ高さになる位置を特定し、その時のxの値と点pのxの値を比較する。
					var vt = (target.y - points[i].y) / (points[i+1].y - points[i].y);
					if(target.x < (points[i].x + (vt * (points[i+1].x - points[i].x)))){
						--wn;  //ここが重要。下向きの辺と交差した場合は-1
					}	
				}
				// ルール1,ルール2を確認することで、ルール3も確認できている。
			}

			return wn != 0;
		}

		public static bool  TryGetDirectionFromTouch(
			this Camera camera, Vector2 fromScreenPoint, Vector2 toScreenPoint, out Vector3 direction, float distance = 1000f, params string[] layerNames)
		{
			return TryGetDirectionFromTouch(camera, fromScreenPoint, toScreenPoint, out direction, LayerMask.GetMask(layerNames), distance);
		}

		public static bool TryGetDirectionFromTouch(
			this Camera camera, Vector2 fromScreenPoint, Vector2 toScreenPoint, out Vector3 direction, LayerMask layerMask, float distance = 1000f)
		{
			var fromRay = camera.ScreenPointToRay(fromScreenPoint);
			RaycastHit fromRayHit;

			if (!Physics.Raycast(fromRay, out fromRayHit, distance, layerMask))
			{
				direction = Vector3.zero;
				return false;
			}

			var toRay = camera.ScreenPointToRay(toScreenPoint);
			RaycastHit toRayHit;

			if (!Physics.Raycast(toRay, out toRayHit, distance, layerMask))
			{
				direction = Vector3.zero;
				return false;
			}

			direction = (toRayHit.point - fromRayHit.point).normalized;

			return true;
		}

		public static class Debug
		{
			public static void DrawLines(Color color, float duration, params Vector3[] points)
			{
				for (int i = 0; i < points.Length; i++)
				{
					var start = points[i];

					var end = i + 1 >= points.Length ? points[0] : points[i + 1];

					UnityEngine.Debug.DrawLine(start, end, color, duration);
				}
			}

			public static void DrawCross(Vector3 center, Vector3 size, Color color, float duration)
			{
				UnityEngine.Debug.DrawLine(center + Vector3.left * size.x / 2, center + Vector3.right * size.x / 2, color, duration);
				UnityEngine.Debug.DrawLine(center + Vector3.down * size.y / 2, center + Vector3.up * size.y / 2, color, duration);
				UnityEngine.Debug.DrawLine(center + Vector3.back * size.z / 2, center + Vector3.forward * size.z / 2, color, duration);
			}
		}
	}

	public class IMGrid
	{
		public delegate void ForeachDelegate(int x, int y);

		private int _width;
		private int _height;

		private float _gridWidth => (float)Screen.width / (float)_width;
		private float _gridHeight => (float)Screen.height / (float)_height;

		public IMGrid(int width, int height)
		{
			_width = width;
			_height = height;
		}

		public void Foreach(ForeachDelegate callback)
		{
			for (var x = 0; x < _width; x++)
			{
				for (var y = 0; y < _height; y++)
				{
					callback.Invoke(x, y);
				}
			}
		}

		public void Button(int x, int y, string label, System.Action onClick = null)
		{
			if (GUI.Button(new Rect(_gridWidth * x, _gridHeight * y, _gridWidth, _gridHeight), label))
			{
				onClick?.Invoke();
			}
		}
	}
}