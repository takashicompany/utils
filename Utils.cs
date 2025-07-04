﻿namespace takashicompany.Unity
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.EventSystems;
	using DG.Tweening;
	using DG.Tweening.Core;
	using DG.Tweening.Plugins.Options;
	using Random = UnityEngine.Random;

#if UNITY_EDITOR
	using UnityEditor;

#endif

	[System.Flags]
	public enum UpdateType
	{
		None = 0b0,
		Update = 0b1,
		LateUpdate = 0b10,
		FixedUpdate = 0b100,
	}

	public static class Utils
	{
		public static string GeneratePathByPersistent(string fileName)
		{
			var path = Application.persistentDataPath;
#if UNITY_EDITOR
			path = System.IO.Path.GetFullPath(".");
			path = System.IO.Path.Combine(path, "persistent-data");
			if (!System.IO.Directory.Exists(path))
			{
				System.IO.Directory.CreateDirectory(path);
			}
#endif
			path = System.IO.Path.Combine(path, fileName);
			return path;
		}

		public static bool IsUpdate(this UpdateType updateType)
		{
			return updateType.HasFlag(UpdateType.Update);
		}

		public static bool IsLateUpdate(this UpdateType updateType)
		{
			return updateType.HasFlag(UpdateType.LateUpdate);
		}

		public static bool IsFixedUpdate(this UpdateType updateType)
		{
			return updateType.HasFlag(UpdateType.FixedUpdate);
		}

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

		public static void SetDirty(UnityEngine.Object obj)
		{

#if UNITY_EDITOR
			EditorUtility.SetDirty(obj);
#endif
		}

		#region Object
		public static T Instantiate<T>(this T obj, Transform parent) where T : UnityEngine.Object
		{
			return GameObject.Instantiate(obj, parent);
		}

		/// <summary>
		/// 再生中ではない時は、プレハブの参照がされたInstantiateを行う。
		/// </summary>
		public static T InstantiateWithPrefabReference<T>(this T prefab, Transform parent) where T : UnityEngine.Object
		{
			T obj = null;

#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				obj = UnityEditor.PrefabUtility.InstantiatePrefab(prefab, parent) as T; // 動くかは未確認
			}
			if (obj == null)
			{
				obj = GameObject.Instantiate(prefab, parent);
			}
#else
			obj = Instantiate(prefab, parent);
#endif
			return obj;
		}
		#endregion

		#region MemberInfo
		public static A GetAttribute<T, A>(this T code) where T : Enum where A : Attribute
		{
			var type = code.GetType();
			var memberInfo = type.GetMember(code.ToString());
			if (memberInfo.Length > 0)
			{
				var attributes = memberInfo[0].GetCustomAttributes(typeof(A), false);
				if (attributes.Length > 0)
				{
					// 最初に見つかった MyDescriptionAttribute の Description プロパティを返す
					var attr = (A)attributes[0];
					return attr;
				}
			}

			return null;
		}

		public static IEnumerable<A> GetAttributes<T, A>(this T code) where T : Enum where A : Attribute
		{
			var type = code.GetType();
			var memberInfo = type.GetMember(code.ToString());
			if (memberInfo.Length > 0)
			{
				var attributes = memberInfo[0].GetCustomAttributes(typeof(A), false);

				foreach (var attribute in attributes)
				{
					yield return (A)attribute;
				}
			}
		}
		#endregion

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

		// この関数は配列の長さと引数indexを受け取り、中央からのオフセットに基づいて添字を返します。
		public static int SelectIndexFromCenter(int length, int index)
		{
			int midPoint = length / 2; // 配列の中点を求める
			int offsetIndex;

			// 配列の要素数が偶数か奇数かによってロジックを分ける
			if (length % 2 == 0)
			{
				// 偶数の場合
				if (index % 2 == 0)
				{
					// indexが偶数の場合、中心から左へ移動
					offsetIndex = midPoint - (index / 2 + 1);
				}
				else
				{
					// indexが奇数の場合、中心から右へ移動
					offsetIndex = midPoint + (index / 2);
				}
			}
			else
			{
				// 奇数の場合
				if (index % 2 == 0)
				{
					// indexが偶数の場合、中心から左へ移動
					offsetIndex = midPoint - index / 2;
				}
				else
				{
					// indexが奇数の場合、中心から右へ移動
					offsetIndex = midPoint + index / 2;
				}
			}

			// 範囲外のindexをチェック
			if (offsetIndex < 0 || offsetIndex >= length)
			{
				throw new IndexOutOfRangeException("Index is out of range.");
			}

			return offsetIndex;
		}

		#endregion

		#region Enum
		public static IEnumerable<T> GetValues<T>() where T : System.Enum
		{
			foreach (var value in System.Enum.GetValues(typeof(T)))
			{
				yield return (T)value;
			}
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

		public static readonly string[] enclosedNumerics = { "①", "②", "③", "④", "⑤", "⑥", "⑦", "⑧", "⑨", "⑩", "⑪", "⑫", "⑬", "⑭", "⑮", "⑯", "⑰", "⑱", "⑲", "⑳", "㉑", "㉒", "㉓", "㉔", "㉕", "㉖", "㉗", "㉘", "㉙", "㉚", "㉛", "㉜", "㉝", "㉞", "㉟", "㊱", "㊲", "㊳", "㊴", "㊵", "㊶", "㊷", "㊷", "㊸", "㊹", "㊺", "㊻", "㊼", "㊽", "㊾", "㊿" };

		/// <summary>
		/// ①〜㊿までの数字を返す
		/// </summary>
		/// <param name="num"></param>
		/// <returns></returns>
		public static string GetEnclosedNumeric(int num)
		{
			if (0 < num && num <= enclosedNumerics.Length)
			{
				return enclosedNumerics[num - 1];
			}

			return null;
		}

		public static bool TryGetEnclosedNumeric(int num, out string result)
		{
			result = GetEnclosedNumeric(num);

			return result != null;
		}

		/// <summary>
		/// ①~㊿の文字列を数値に返す
		/// </summary>
		/// <param name="enclosedNumeric"></param>
		/// <returns></returns>
		public static int GetNumber(string enclosedNumeric)
		{
			var index = enclosedNumerics.IndexOf(enclosedNumeric);

			if (index < 0)
			{
				return int.MinValue;
			}

			return index + 1;
		}

		public static bool TryGetNumber(string enclosedNumeric, out int result)
		{
			result = GetNumber(enclosedNumeric);

			return result != int.MinValue;
		}

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

		public static string ToTMPColorTag(this string text, Color color)
		{
			return string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color), text);
		}

		public static string ToTMPSizeTag(this string text, string size)
		{
			return string.Format("<size={0}>{1}</size>", size, text);
		}

		public static string ToStringByItem<T>(this IEnumerable<T> items)
		{
			var str = "";

			foreach (var item in items)
			{
				if (!string.IsNullOrEmpty(str))
				{
					str += ", ";
				}

				str += item.ToString();
			}

			return str;
		}

		/// <summary>
		/// 二次元的にマッピングされた文字列を2次元配列で返す
		/// 0,1,0
		/// 1,0,1
		/// 1,1,0
		/// </summary>
		public static string[,] Get2dMap(this string str, string columnSeparete, string rowSeparete = "\n")
		{
			if (string.IsNullOrEmpty(str))
			{
				return new string[0, 0];
			}

			var rows = str.Split(rowSeparete);

			var columnLists = new List<string[]>();

			foreach (var r in rows)
			{
				if (string.IsNullOrEmpty(r))
				{
					continue;
				}

				var column = r.Split(columnSeparete);
				columnLists.Add(column);
			}

			if (columnLists.Count == 0)
			{
				return new string[0, 0];
			}

			var maxColumn = columnLists.Max(c => c.Length);

			var result = new string[maxColumn, columnLists.Count];

			for (var y = 0; y < result.GetLength(1); y++)
			{
				var column = columnLists[y];
				for (var x = 0; x < column.Length; x++)
				{
					result[x, y] = column[x];
				}
			}

			return result;
		}

		public static IEnumerable<Vector2Int> ToVector2Ints(this string[,] map, Func<string, Vector2Int, bool> func)
		{
			for (var y = 0; y < map.GetLength(1); y++)
			{
				for (var x = 0; x < map.GetLength(0); x++)
				{
					Debug.Log("x:" + x + " y:" + y + " map:" + map[x, y]);
					if (func(map[x, y], new Vector2Int(x, y)))
					{
						yield return new Vector2Int(x, y);
					}
				}
			}
		}

		/// <summary>
		/// 文字列を色に変換する
		/// </summary>
		/// <param name="htmlString">16進数の場合は先頭に#をつける</param>
		public static Color ToColor(this string htmlString)
		{
			ColorUtility.TryParseHtmlString(htmlString, out var color);
			return color;
		}

		#endregion

		#region  IEnumeralbe
		public static T GetByValidIndex<T>(this IEnumerable<T> self, int index)
		{
			// https://ufcpp.net/blog/2024/3/ilist-inherites-from-ireadonlylist/
			switch (self)
			{
				case IList<T> list:
					return list.GetByValidIndex(index);

				case IReadOnlyList<T> list:
					return list.GetByValidIndex(index);
			}

			return self.ElementAt(Mathf.Abs(index) % self.Count());
		}
		#endregion

		#region IList
		private static T GetByValidIndex<T>(this IList<T> self, int index)
		{
			return self[Mathf.Abs(index) % self.Count];
		}
		#endregion

		#region IReadOnlyList
		private static T GetByValidIndex<T>(this IReadOnlyList<T> self, int index)
		{
			return self[Mathf.Abs(index) % self.Count];
		}
		#endregion

		#region Vector3

		// オイラー角を用いてベクトルを回転させ、正規化したベクトルを返す関数
		public static Vector3 RotateAndNormalize(this Vector3 vector, Vector3 eulerAngles)
		{
			// オイラー角からQuaternionを作成
			Quaternion rotation = Quaternion.Euler(eulerAngles);

			// Quaternionを使用してベクトルを回転させる
			Vector3 rotatedVector = rotation * vector;

			// 回転したベクトルを正規化して返す
			return rotatedVector.normalized;
		}

		public static Vector3 ベクトルをオイラー角を用いて回転させた後に正規化して返す(this Vector3 vector, Vector3 eulerAngles)
		{
			return RotateAndNormalize(vector, eulerAngles);
		}

		#endregion

		#region 多次元配列

		// 二次元配列の行を反転する拡張メソッド
		public static T[,] TransposeColumns<T>(this T[,] inputArray)
		{
			int cols = inputArray.GetLength(0); // 列数
			int rows = inputArray.GetLength(1); // 行数

			for (int col = 0; col < cols / 2; col++)
			{
				for (int row = 0; row < rows; row++)
				{
					T temp = inputArray[col, row];
					inputArray[col, row] = inputArray[cols - col - 1, row];
					inputArray[cols - col - 1, row] = temp;
				}
			}

			return inputArray;
		}

		// 二次元配列の列を反転する拡張メソッド
		public static T[,] TransposeRows<T>(this T[,] inputArray)
		{
			int cols = inputArray.GetLength(0); // 列数
			int rows = inputArray.GetLength(1); // 行数

			for (int col = 0; col < cols; col++)
			{
				for (int row = 0; row < rows / 2; row++)
				{
					T temp = inputArray[col, row];
					inputArray[col, row] = inputArray[col, rows - row - 1];
					inputArray[col, rows - row - 1] = temp;
				}
			}

			return inputArray;
		}

		public delegate T ConvertDelegate<F, T>(F from);

		public static T[,] Convert<F, T>(this F[,] self, ConvertDelegate<F, T> convert)
		{
			var result = new T[self.GetLength(0), self.GetLength(1)];

			for (var x = 0; x < self.GetLength(0); x++)
			{
				for (var y = 0; y < self.GetLength(1); y++)
				{
					result[x, y] = convert(self[x, y]);
				}
			}

			return result;
		}


		public static T Get<T>(this T[,,] self, Vector3Int p)
		{
			return self[p.x, p.y, p.z];
		}

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

		public static bool IsInBounds<T>(this T[,,] self, Vector3Int p)
		{
			return self.IsInBounds(p.x, p.y, p.z);
		}

		public static bool IsInBounds<T>(this T[,,] self, int x, int y, int z)
		{
			if (x < 0 || self.GetLength(0) <= x || y < 0 || self.GetLength(1) <= y || z < 0 || self.GetLength(1) <= z)
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

		public static void Foreach<T>(this T[,] self, System.Action<int, int, T> callback)
		{
			for (var y = 0; y < self.GetLength(1); y++)
			{
				for (var x = 0; x < self.GetLength(0); x++)
				{
					callback(x, y, self[x, y]);
				}
			}
		}

		public static void Foreach<T>(this T[,,] self, System.Action<int, int, int, T> callback)
		{
			for (var x = 0; x < self.GetLength(0); x++)
			{
				for (var y = 0; y < self.GetLength(1); y++)
				{
					for (var z = 0; z < self.GetLength(2); z++)
					{
						callback?.Invoke(x, y, z, self[x, y, z]);
					}
				}
			}
		}

		public static void Foreach<T>(this T[,,] self, System.Action<Vector3Int, T> callback)
		{
			self.Foreach((x, y, z, item) => callback?.Invoke(new Vector3Int(x, y, z), item));
		}

		public static void Foreach<T>(this T[,] self, System.Action<Vector2Int, T> callback)
		{
			self.Foreach((x, y, item) => callback(new Vector2Int(x, y), item));
		}

		public static void Foreach<T>(this T[,,] self, System.Func<int, int, int, T, bool> callback)
		{
			for (var x = 0; x < self.GetLength(0); x++)
			{
				for (var y = 0; y < self.GetLength(1); y++)
				{
					for (var z = 0; z < self.GetLength(2); z++)
					{
						if (callback.Invoke(x, y, z, self[x, y, z]))
						{
							return;
						}
					}
				}
			}
		}

		public static void Foreach<T>(this T[,,] self, System.Func<Vector3Int, T, bool> callback)
		{
			self.Foreach((x, y, z, item) => callback?.Invoke(new Vector3Int(x, y, z), item));
		}

		public delegate bool BareDelegate<T>(T target) where T : IEquatable<T>;

		public static bool IsBare<T>(this T[,,] self, Vector3Int p) where T : IEquatable<T>
		{
			return self.IsBare(p, obj =>
			{
				return obj.Equals(default(T));      // nullとかだった時心配だわ
			});
		}

		public static bool IsBare<T>(this T[,,] self, Vector3Int p, BareDelegate<T> callback) where T : IEquatable<T>
		{
			for (var x = -1; x <= 1; x++)
			{
				for (var y = -1; y <= 1; y++)
				{
					for (var z = -1; z <= 1; z++)
					{
						if (x == p.x && y == p.y && z == p.z)
						{
							continue;
						}

						var current = new Vector3Int(p.x + x, p.y + y, p.z + z);

						if (!self.IsInBounds(p))    // 範囲外に触れているなら、むき出しとする
						{
							return true;
						}

						if (callback(self[current.x, current.y, current.z]))
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		public delegate bool ConnectDelegate<T>(T main, T current, T target);

		public static Dictionary<Vector3Int, int> GetConnected<T>(this T[,,] self, Vector3Int from, bool onlyOneDistance, ConnectDelegate<T> callback)
		{
			var connectedWithSteps = new Dictionary<Vector3Int, int>();

			var unconnecteds = new HashSet<Vector3Int>();

			var main = self[from.x, from.y, from.z];

			Search(from, 0);

			void Search(Vector3Int current, int step)
			{
				step++;

				for (var x = -1; x <= 1; x++) for (var y = -1; y <= 1; y++) for (var z = -1; z <= 1; z++)
						{
							var target = new Vector3Int(current.x + x, current.y + y, current.z + z);

							if (onlyOneDistance && Vector3Int.Distance(current, target) > 1)
							{
								continue;
							}

							if (target == from)
							{
								continue;
							}

							if (!self.IsInBounds(target))
							{
								continue;
							}

							if (unconnecteds.Contains(target))
							{
								continue;
							}

							if (connectedWithSteps.ContainsKey(target))
							{
								continue;
							}

							if (callback(main, self.Get(current), self.Get(target)))
							{
								connectedWithSteps.Add(target, step);
								Search(target, step);
							}
							else
							{
								unconnecteds.Add(target);
							}
						}
			}

			return connectedWithSteps;
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

		// 実装が古いのでコメントアウト

		// /// <summary>
		// /// 距離を元に最後の点を返す。
		// /// </summary>
		// public static Vector3 GetPathPointByDistance(this IList<Vector2> path, float distance, out int index)
		// {
		// 	var currentDistance = 0f;

		// 	for (int i = 1; i < path.Count; i++)
		// 	{
		// 		var prev = path[i - 1];
		// 		var current = path[i];
		// 		currentDistance += Vector2.Distance(prev, current);
		// 		if (currentDistance >= distance)
		// 		{
		// 			index = i - 1;
		// 			return prev;
		// 		}
		// 	}

		// 	index = path.Count - 1;

		// 	return path[index];
		// }

		// /// <summary>
		// /// 距離を元に最後の点を返す。
		// /// </summary>
		// public static Vector3 GetPathPointByDistance(this IList<Vector3> path, float distance, out int index)
		// {
		// 	// TODO ↑のVector2版と実装を共用にできないか

		// 	var currentDistance = 0f;

		// 	for (int i = 1; i < path.Count; i++)
		// 	{
		// 		var prev = path[i - 1];
		// 		var current = path[i];
		// 		currentDistance += Vector3.Distance(prev, current);
		// 		if (currentDistance >= distance)
		// 		{
		// 			index = i - 1;
		// 			return prev;
		// 		}
		// 	}

		// 	index = path.Count - 1;

		// 	return path[index];
		// }


		/// <summary>
		/// パスの中から位置を求める
		/// </summary>
		/// <param name="path"></param>
		/// <param name="normalized">0~1</param>
		/// <returns></returns>
		public static Vector3 GetPointOfProgress(this IReadOnlyList<Vector3> path, float normalized)
		{
			normalized = Mathf.Clamp01(normalized);

			var totalLength = path.GetTotalLength(out _, out _);

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
		public static float GetTotalLength(this IReadOnlyList<Vector3> path, out List<float> distances, out List<float> distancesFromStart, int startIndex = 0, float length = 0)
		{
			distances = new List<float>();
			distancesFromStart = new List<float>();

			for (int i = startIndex; i < path.Count - 1; i++)
			{
				var head = path[i];
				var tail = path[i + 1];

				var distance = Vector3.Distance(head, tail);
				distances.Add(distance);
				length += distance;
				distancesFromStart.Add(length);
			}

			return length;
		}

		public static int GetIndex(this IReadOnlyList<Vector3> path, float distance, out float distanceFromIndexOfPoint)
		{
			var totalLength = 0f;

			for (int i = 0; i < path.Count - 1; i++)
			{
				var head = path[i];
				var tail = path[i + 1];

				var currentDistance = Vector3.Distance(head, tail);

				if (distance < totalLength + currentDistance)
				{
					distanceFromIndexOfPoint = distance - totalLength;
					return i;
				}

				totalLength += currentDistance;
			}

			distanceFromIndexOfPoint = distance - totalLength;
			return path.Count - 1;
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

			for (int i = 0; i < path.Count - 1; i++)
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

		public static IEnumerable<Vector3> CreateCirclePoints(int plane, int pointCount, float radius, float angleOffset = 0f)
		{
			if (pointCount <= 0) yield break;

			float angleStep = 360f / pointCount;
			float startAngle = -90f + angleOffset;

			for (int i = 0; i < pointCount; i++)
			{
				float angle = (startAngle + i * angleStep) * Mathf.Deg2Rad;
				float x = Mathf.Cos(angle) * radius;
				float y = Mathf.Sin(angle) * radius;

				yield return ProjectToPlane(plane, x, y);
			}
		}

		public static IEnumerable<Vector3> CreateRegularPolygon(int plane, int sides, float radius, float angleOffset = 0f)
		{
			if (sides < 3) yield break;

			float angleStep = 360f / sides;
			float startAngle = -90f + angleOffset;

			for (int i = 0; i < sides; i++)
			{
				float angle = (startAngle + i * angleStep) * Mathf.Deg2Rad;
				float x = Mathf.Cos(angle) * radius;
				float y = Mathf.Sin(angle) * radius;

				yield return ProjectToPlane(plane, x, y);
			}
		}

		public static IEnumerable<Vector3> CreateTriangle(int plane, float radius, float angleOffset = 0f)
		{
			int sides = 3;
			float angleStep = 360f / sides;
			float startAngle = -270f + angleOffset;

			for (int i = 0; i < sides; i++)
			{
				float angle = (startAngle + i * angleStep) * Mathf.Deg2Rad;
				float x = Mathf.Cos(angle) * radius;
				float y = Mathf.Sin(angle) * radius;

				yield return ProjectToPlane(plane, x, y);
			}
		}

		public static IEnumerable<Vector3> CreateStar(int plane, int points, float outerRadius, float innerRadius, float angleOffset = 0f)
		{
			if (points < 2) yield break;

			float angleStep = 180f / points;
			float startAngle = -270f + angleOffset;

			for (int i = 0; i < points * 2; i++)
			{
				float angle = (startAngle + i * angleStep) * Mathf.Deg2Rad;
				float r = (i % 2 == 0) ? outerRadius : innerRadius;
				float x = Mathf.Cos(angle) * r;
				float y = Mathf.Sin(angle) * r;

				yield return ProjectToPlane(plane, x, y);
			}
		}

		public static IEnumerable<Vector3> CreateHeart(int plane, int pointCount, float radius, float angleOffset = 0f)
		{
			if (pointCount <= 0) yield break;

			float scale = radius / 18f;
			float offset = angleOffset * Mathf.Deg2Rad;

			for (int i = 0; i < pointCount; i++)
			{
				float t = offset + 2 * Mathf.PI * i / pointCount;
				float x = 16 * Mathf.Pow(Mathf.Sin(t), 3);
				float y = 13 * Mathf.Cos(t) - 5 * Mathf.Cos(2 * t) - 2 * Mathf.Cos(3 * t) - Mathf.Cos(4 * t);

				yield return ProjectToPlane(plane, x * scale, y * scale);
			}
		}

		public static IEnumerable<Vector3> CreateSpiral(int plane, int pointCount, float radius, float turns = 3f, float angleOffset = 0f)
		{
			if (pointCount <= 0) yield break;

			float totalAngle = 360f * turns;
			for (int i = 0; i < pointCount; i++)
			{
				float t = (float)i / (pointCount - 1);
				float angle = (angleOffset - 90f + t * totalAngle) * Mathf.Deg2Rad;
				float r = radius * t;
				float x = Mathf.Cos(angle) * r;
				float y = Mathf.Sin(angle) * r;

				yield return ProjectToPlane(plane, x, y);
			}
		}

		public static IEnumerable<Vector3> CreateRoseCurve(int plane, int pointCount, float radius, int petalCount = 4, float angleOffset = 0f)
		{
			if (pointCount <= 0 || petalCount <= 0) yield break;

			for (int i = 0; i < pointCount; i++)
			{
				float t = 2 * Mathf.PI * i / pointCount;
				float angle = t + angleOffset * Mathf.Deg2Rad;
				float r = radius * Mathf.Cos(petalCount * angle);
				float x = Mathf.Cos(angle) * r;
				float y = Mathf.Sin(angle) * r;

				yield return ProjectToPlane(plane, x, y);
			}
		}

		public static IEnumerable<Vector3> CreateSineWave(int plane, int pointCount, float length, float amplitude, float frequency = 1f, float angleOffset = 0f)
		{
			if (pointCount <= 0) yield break;

			for (int i = 0; i < pointCount; i++)
			{
				float t = (float)i / (pointCount - 1);
				float x = (t - 0.5f) * length; // -length/2 ～ +length/2 に調整
				float y = Mathf.Sin(2 * Mathf.PI * frequency * t + angleOffset * Mathf.Deg2Rad) * amplitude;

				yield return ProjectToPlane(plane, x, y);
			}
		}

		public static IEnumerable<Vector3> CreateArc(int plane, int pointCount, float radius, float startAngle, float endAngle, float angleOffset = 0f)
		{
			if (pointCount <= 1) yield break;

			List<Vector2> points = new List<Vector2>();

			for (int i = 0; i < pointCount; i++)
			{
				float t = (float)i / (pointCount - 1);
				float angle = (Mathf.Lerp(startAngle, endAngle, t) + angleOffset - 90f) * Mathf.Deg2Rad;
				float x = Mathf.Cos(angle) * radius;
				float y = Mathf.Sin(angle) * radius;
				points.Add(new Vector2(x, y));
			}

			// 中心点（バウンディングボックスの中央）を求めて原点に平行移動
			Vector2 min = points[0];
			Vector2 max = points[0];
			foreach (var p in points)
			{
				min = Vector2.Min(min, p);
				max = Vector2.Max(max, p);
			}
			Vector2 center = (min + max) / 2f;

			foreach (var p in points)
			{
				yield return ProjectToPlane(plane, p.x - center.x, p.y - center.y);
			}
		}

		public static IEnumerable<Vector3> CreateCardioid(int plane, int pointCount, float radius, float angleOffset = 0f)
		{
			if (pointCount <= 0) yield break;

			for (int i = 0; i < pointCount; i++)
			{
				float t = 2 * Mathf.PI * i / pointCount;
				float angle = t + angleOffset * Mathf.Deg2Rad;
				float r = radius * (1 - Mathf.Sin(angle));
				float x = r * Mathf.Cos(angle);
				float y = r * Mathf.Sin(angle);

				yield return ProjectToPlane(plane, x, y);
			}
		}

		public static IEnumerable<Vector3> CreateLimacon(int plane, int pointCount, float radius, float innerFactor = 0.5f, float angleOffset = 0f)
		{
			if (pointCount <= 0) yield break;

			for (int i = 0; i < pointCount; i++)
			{
				float t = 2 * Mathf.PI * i / pointCount;
				float angle = t + angleOffset * Mathf.Deg2Rad;
				float r = radius * (1 + innerFactor * Mathf.Cos(angle));
				float x = r * Mathf.Cos(angle);
				float y = r * Mathf.Sin(angle);

				yield return ProjectToPlane(plane, x, y);
			}
		}

		public static IEnumerable<Vector3> CreateHarmonicCurve(int plane, int pointCount, float radius, float frequency = 2f, float amplitude = 0.5f, float angleOffset = 0f)
		{
			if (pointCount <= 0) yield break;

			for (int i = 0; i < pointCount; i++)
			{
				float t = (float)i / (pointCount - 1);
				float x = Mathf.Lerp(-radius, radius, t);
				float y = Mathf.Sin(t * Mathf.PI * frequency + angleOffset * Mathf.Deg2Rad) * amplitude * radius;

				yield return ProjectToPlane(plane, x, y);
			}
		}

		public static IEnumerable<Vector3> CreateInfinityCurve(int plane, int pointCount, float radius, float angleOffset = 0f)
		{
			if (pointCount <= 0) yield break;

			for (int i = 0; i < pointCount; i++)
			{
				float t = 2 * Mathf.PI * i / pointCount;
				float angle = t + angleOffset * Mathf.Deg2Rad;
				float x = radius * Mathf.Sin(angle);
				float y = radius * Mathf.Sin(angle) * Mathf.Cos(angle);

				yield return ProjectToPlane(plane, x, y);
			}
		}

		public static IEnumerable<Vector3> CreateClover(int plane, int pointCount, float radius, int leaves = 3, float angleOffset = 0f)
		{
			if (pointCount <= 0 || leaves <= 0) yield break;

			for (int i = 0; i < pointCount; i++)
			{
				float t = 2 * Mathf.PI * i / pointCount;
				float angle = t + angleOffset * Mathf.Deg2Rad;
				float r = radius * Mathf.Sin(leaves * angle);
				float x = r * Mathf.Cos(angle);
				float y = r * Mathf.Sin(angle);

				yield return ProjectToPlane(plane, x, y);
			}
		}

		public static IEnumerable<Vector3> CreateDoubleLemniscate(int plane, int pointCount, float radius, float angleOffset = 0f)
		{
			if (pointCount <= 0) yield break;

			for (int i = 0; i < pointCount; i++)
			{
				float t = 2 * Mathf.PI * i / pointCount;
				float angle = t + angleOffset * Mathf.Deg2Rad;
				float r = radius * Mathf.Sqrt(Mathf.Abs(Mathf.Cos(2 * angle)));
				float x = r * Mathf.Cos(angle);
				float y = r * Mathf.Sin(angle);

				yield return ProjectToPlane(plane, x, y);
			}
		}

		public static IEnumerable<Vector3> CreateButterflyCurve(int plane, int pointCount, float radius, float angleOffset = 0f)
		{
			if (pointCount <= 0) yield break;

			for (int i = 0; i < pointCount; i++)
			{
				float t = 12 * Mathf.PI * i / (pointCount - 1);
				float angle = t + angleOffset * Mathf.Deg2Rad;

				float r = radius * (Mathf.Exp(Mathf.Sin(angle))
						  - 2 * Mathf.Cos(4 * angle)
						  + Mathf.Pow(Mathf.Sin((2 * angle - Mathf.PI) / 24), 5));

				float x = r * Mathf.Cos(angle);
				float y = r * Mathf.Sin(angle);

				yield return ProjectToPlane(plane, x, y);
			}
		}

		public static IEnumerable<Vector3> CreateHypotrochoid(int plane, int pointCount, float bigRadius, float smallRadius, float offset, float angleOffset = 0f)
		{
			if (pointCount <= 0) yield break;

			for (int i = 0; i < pointCount; i++)
			{
				float t = 2 * Mathf.PI * i / pointCount;
				float angle = t + angleOffset * Mathf.Deg2Rad;

				float R = bigRadius;
				float r = smallRadius;
				float d = offset;

				float x = (R - r) * Mathf.Cos(angle) + d * Mathf.Cos((R - r) / r * angle);
				float y = (R - r) * Mathf.Sin(angle) - d * Mathf.Sin((R - r) / r * angle);

				yield return ProjectToPlane(plane, x, y);
			}
		}

		public static IEnumerable<Vector3> CreateTrefoilKnot(int plane, int pointCount, float radius, float angleOffset = 0f)
		{
			if (pointCount <= 0) yield break;

			for (int i = 0; i < pointCount; i++)
			{
				float t = 2 * Mathf.PI * i / pointCount;
				float angle = t + angleOffset * Mathf.Deg2Rad;

				float x = radius * (Mathf.Sin(angle) + 2 * Mathf.Sin(2 * angle));
				float y = radius * (Mathf.Cos(angle) - 2 * Mathf.Cos(2 * angle));

				yield return ProjectToPlane(plane, x, y);
			}
		}


		private static Vector3 ProjectToPlane(int plane, float x, float y)
		{
			switch (plane)
			{
				case 0: return new Vector3(0, x, y);     // YZ
				case 1: return new Vector3(x, 0, y);     // XZ
				case 2: return new Vector3(x, y, 0);     // XY
				default: return Vector3.zero;
			}
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
		public static void AttachOnCanvas(this Transform transform, Vector3 worldPosition, Camera camera = null)
		{
			if (camera == null) camera = Camera.main;
			var sp = camera.WorldToScreenPoint(worldPosition);
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

		#region Color
		public static Color GetRandomColor()
		{
			return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1), 1);
		}

		public static string ColorToHex(this Color self, bool withAlpha = false)
		{
			return ColorToHex((Color32)self);
		}

		public static string ColorToHex(this Color32 self, bool withAlpha = false)
		{
			string hex = self.r.ToString("X2") + self.g.ToString("X2") + self.b.ToString("X2");
			if (withAlpha) hex += self.a.ToString("X2");
			return hex;
		}
		#endregion

		#region UnityUI
		public static string WrapColorTag(this string text, Color color)
		{
			return "<color=#" + color.ColorToHex() + ">" + text + "</color>";
		}

		public static string WrapSizeTag(this string text, int size)
		{
			return "<size=" + size + ">" + text + "</size>";
		}
		#endregion

		#region  Vector3
		/// <summary>
		/// 四捨五入したV3Intを返す
		/// </summary>
		public static Vector3Int ToRound(this Vector3 p) => ToRound(p, Vector3.one);

		/// <summary>
		/// 四捨五入したV3Intを返す
		/// </summary>
		public static Vector3Int ToRound(this Vector3 p, Vector3 unitPerGrid)
		{
			return new Vector3Int(
				(int)System.Math.Round(p.x / unitPerGrid.x, System.MidpointRounding.AwayFromZero),
				(int)System.Math.Round(p.y / unitPerGrid.y, System.MidpointRounding.AwayFromZero),
				(int)System.Math.Round(p.z / unitPerGrid.y, System.MidpointRounding.AwayFromZero)
			);
		}

		/// <summary>
		/// XZ座標でfromからtoの点がforward方向から一定角度の範囲内かを判定する
		/// </summary>
		public static bool IsInAngleXZ(this Vector3 from, Vector3 to, Vector3 forward, float angleThreshold)
		{
			Vector3 direction = to - from;
			direction.y = 0;
			direction.Normalize();
			float angle = Vector3.Angle(forward, direction);

			return angle <= angleThreshold;
		}

		public static Quaternion LookAt(this Quaternion currentRotation, Vector3 from, Vector3 to, float delta)
		{
			var direction = to - from;

			if (direction.magnitude == 0)
			{
				return Quaternion.identity;
			}

			var targetRotation = Quaternion.LookRotation(direction);
			return Quaternion.Slerp(currentRotation, targetRotation, delta);
		}

		#endregion

		#region Vector3Int

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

		public static IEnumerable<Vector3Int> GetV3Ints(this Vector3Int self, bool includeMax = false)
		{
			for (var z = 0; includeMax ? z <= self.z : z < self.z; z++)
			{
				for (var y = 0; includeMax ? y <= self.y : y < self.y; y++)
				{
					for (var x = 0; includeMax ? x <= self.x : x < self.x; x++)
					{
						yield return new Vector3Int(x, y, z);
					}
				}
			}
		}

		public static Vector3 ToVector3(this Vector3Int self)
		{
			return new Vector3(self.x, self.y, self.z);
		}

		public static int GetIndexSingle(this Vector3Int self)
		{
			Debug.Assert(self.ToAxisMask().magnitude == 1, "複数の軸が指定されています。");
			if (self.x != 0) return 0;
			if (self.y != 0) return 1;
			if (self.z != 0) return 2;

			throw new InvalidOperationException("指定されたベクトルは、いずれの軸も 0 です。");
		}

		/// <summary>
		/// Vector3Int の各成分が 0 でない場合は 1 に変換する。
		/// </summary>
		public static Vector3Int ToAxisMask(this Vector3Int self)
		{
			return new Vector3Int(
				self.x != 0 ? 1 : 0,
				self.y != 0 ? 1 : 0,
				self.z != 0 ? 1 : 0
			);
		}

		/// <summary>
		/// Vector3Int 同士の演算のみで、direction 方向へ最も突き出た点を返す。
		/// </summary>
		public static Vector3Int MaxInDirection(this IEnumerable<Vector3Int> source, Vector3Int direction)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			using var e = source.GetEnumerator();
			if (!e.MoveNext()) throw new InvalidOperationException("source contains no elements.");

			// --- ローカル関数でドット積を計算 ---
			static int Dot(in Vector3Int a, in Vector3Int b) =>
				a.x * b.x + a.y * b.y + a.z * b.z;
			// ----------------------------------

			Vector3Int best = e.Current;
			int bestDot = Dot(best, direction);

			while (e.MoveNext())
			{
				int dot = Dot(e.Current, direction);
				if (dot > bestDot)
				{
					bestDot = dot;
					best = e.Current;
				}
			}

			return best;
		}

		/// <summary>
		/// 点集合すべてを含む BoundsInt を返す。
		/// （BoundsInt は position を最小座標、size を max 近傍排他的にとる仕様）
		/// </summary>
		/// <exception cref="ArgumentNullException">source が null</exception>
		/// <exception cref="InvalidOperationException">source が空</exception>
		public static BoundsInt ToBoundsInt(this IEnumerable<Vector3Int> source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			using var e = source.GetEnumerator();
			if (!e.MoveNext()) throw new InvalidOperationException("source contains no elements.");

			Vector3Int min = e.Current;
			Vector3Int max = e.Current;

			while (e.MoveNext())
			{
				Vector3Int p = e.Current;
				if (p.x < min.x) min.x = p.x;
				if (p.y < min.y) min.y = p.y;
				if (p.z < min.z) min.z = p.z;

				if (p.x > max.x) max.x = p.x;
				if (p.y > max.y) max.y = p.y;
				if (p.z > max.z) max.z = p.z;
			}

			Vector3Int size = (max - min) + Vector3Int.one; // max を含むため +1
			return new BoundsInt(min, size);
		}

		/// <summary>
		/// ignoreAxis 以外の 2 軸のうち一方が 0 でなければ、
		/// もう一方の軸インデックスを返す拡張メソッド。
		/// </summary>
		/// <param name="v">対象ベクトル</param>
		/// <param name="ignoreAxis">無視する軸 (0:x, 1:y, 2:z)。既定値 1</param>
		/// <returns>0〜2 の軸インデックス</returns>
		/// <exception cref="ArgumentOutOfRangeException">ignoreAxis が 0–2 以外</exception>
		/// <exception cref="ArgumentException">関連 2 軸とも 0 の場合</exception>
		public static int GetPairedAxis(this Vector3Int v, int ignoreAxis = 1)
		{
			switch (ignoreAxis)
			{
				case 0:                         // x を無視
					if (v.y != 0) return 2;     // y が非ゼロ → z
					if (v.z != 0) return 1;     // z が非ゼロ → y
					break;

				case 1:                         // y を無視
					if (v.z != 0) return 0;     // z が非ゼロ → x
					if (v.x != 0) return 2;     // x が非ゼロ → z
					break;

				case 2:                         // z を無視
					if (v.x != 0) return 1;     // x が非ゼロ → y
					if (v.y != 0) return 0;     // y が非ゼロ → x
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(ignoreAxis), "Axis must be 0, 1, or 2.");
			}

			throw new ArgumentException("Both relevant axes are zero.", nameof(v));
		}

		#region Bounds

		// ちょっとこの辺りどれぐらい正しいかは未検証
		public static Bounds Rotate(this Bounds bounds, Quaternion rotation)
		{
			Vector3 rotatedVertex = rotation * (GetCorner(bounds, 0) - bounds.center) + bounds.center;
			Bounds rotatedBounds = new Bounds(rotatedVertex, Vector3.zero);

			for (int i = 1; i < 8; i++)
			{
				rotatedVertex = rotation * (GetCorner(bounds, i) - bounds.center) + bounds.center;
				rotatedBounds.Encapsulate(rotatedVertex);
			}

			return rotatedBounds;
		}

		private static Vector3 GetCorner(Bounds bounds, int index)
		{
			return bounds.center + new Vector3(
				(index & 1) == 0 ? bounds.extents.x : -bounds.extents.x,
				(index & 2) == 0 ? bounds.extents.y : -bounds.extents.y,
				(index & 4) == 0 ? bounds.extents.z : -bounds.extents.z
			);
		}

		public static void DrawWireCube(this Bounds bounds, Transform transform)
		{
			Vector3 localCenter = bounds.center;
			Vector3 localSize = bounds.size;

			// ローカル座標からワールド座標へ変換
			Vector3 worldCenter = transform.TransformPoint(localCenter);
			Vector3 worldSize = Vector3.Scale(localSize, transform.lossyScale);

			// 8つの頂点を計算
			Vector3 corner0 = transform.TransformPoint(localCenter + new Vector3(-worldSize.x, -worldSize.y, -worldSize.z) * 0.5f);
			Vector3 corner1 = transform.TransformPoint(localCenter + new Vector3(worldSize.x, -worldSize.y, -worldSize.z) * 0.5f);
			Vector3 corner2 = transform.TransformPoint(localCenter + new Vector3(worldSize.x, -worldSize.y, worldSize.z) * 0.5f);
			Vector3 corner3 = transform.TransformPoint(localCenter + new Vector3(-worldSize.x, -worldSize.y, worldSize.z) * 0.5f);
			Vector3 corner4 = transform.TransformPoint(localCenter + new Vector3(-worldSize.x, worldSize.y, -worldSize.z) * 0.5f);
			Vector3 corner5 = transform.TransformPoint(localCenter + new Vector3(worldSize.x, worldSize.y, -worldSize.z) * 0.5f);
			Vector3 corner6 = transform.TransformPoint(localCenter + new Vector3(worldSize.x, worldSize.y, worldSize.z) * 0.5f);
			Vector3 corner7 = transform.TransformPoint(localCenter + new Vector3(-worldSize.x, worldSize.y, worldSize.z) * 0.5f);

			// 線を描画
			Gizmos.DrawLine(corner0, corner1);
			Gizmos.DrawLine(corner1, corner2);
			Gizmos.DrawLine(corner2, corner3);
			Gizmos.DrawLine(corner3, corner0);

			Gizmos.DrawLine(corner4, corner5);
			Gizmos.DrawLine(corner5, corner6);
			Gizmos.DrawLine(corner6, corner7);
			Gizmos.DrawLine(corner7, corner4);

			Gizmos.DrawLine(corner0, corner4);
			Gizmos.DrawLine(corner1, corner5);
			Gizmos.DrawLine(corner2, corner6);
			Gizmos.DrawLine(corner3, corner7);
		}

		public static Vector3 GetPositionOfGridCenter(this Bounds bounds, Vector3Int divisions, int x, int y, int z)
		{
			if (divisions.x <= 0 || divisions.y <= 0 || divisions.z <= 0) return bounds.center; // 無効な分割数なら元の中心を返す

			Vector3 sizePerDivision = new Vector3(
				bounds.size.x / divisions.x,
				bounds.size.y / divisions.y,
				bounds.size.z / divisions.z
			);

			Vector3 min = bounds.min;

			// 指定した (x, y, z) に対応する小分け区域の中心を計算
			Vector3 center = new Vector3(
				min.x + (x + 0.5f) * sizePerDivision.x,
				min.y + (y + 0.5f) * sizePerDivision.y,
				min.z + (z + 0.5f) * sizePerDivision.z
			);

			return center;
		}

		public static Vector3Int? GetGridPosition(this Bounds bounds, Vector3Int divisions, Vector3 point, bool clamp = false)
		{
			if (divisions.x <= 0 || divisions.y <= 0 || divisions.z <= 0) return null; // 無効な分割数ならnullを返す

			Vector3 min = bounds.min;
			Vector3 sizePerDivision = new Vector3(
				bounds.size.x / divisions.x,
				bounds.size.y / divisions.y,
				bounds.size.z / divisions.z
			);

			// 各軸ごとに区画インデックスを計算
			int x = Mathf.FloorToInt((point.x - min.x) / sizePerDivision.x);
			int y = Mathf.FloorToInt((point.y - min.y) / sizePerDivision.y);
			int z = Mathf.FloorToInt((point.z - min.z) / sizePerDivision.z);

			// 範囲外処理
			if (clamp)
			{
				x = Mathf.Clamp(x, 0, divisions.x - 1);
				y = Mathf.Clamp(y, 0, divisions.y - 1);
				z = Mathf.Clamp(z, 0, divisions.z - 1);
			}
			else
			{
				if (x < 0 || x >= divisions.x || y < 0 || y >= divisions.y || z < 0 || z >= divisions.z) return null; // 範囲外ならnull
			}

			return new Vector3Int(x, y, z);
		}

		public static bool Contains(this Bounds bounds, IEnumerable<Vector3> points)
		{
			foreach (var p in points)
			{
				if (!bounds.Contains(p))
				{
					return false;
				}
			}

			return true;
		}

		public static Bounds Merge(this IEnumerable<Bounds> bounds)
		{
			var mergedBounds = new Bounds();

			var count = 0;

			foreach (var b in bounds)
			{
				if (count == 0)
				{
					mergedBounds = b;
				}

				mergedBounds.Encapsulate(b);
				count++;
			}

			return mergedBounds;
		}
		
		#endregion

		#region BoundsInt
		public static void Foreach(this BoundsInt b, System.Action<Vector3Int> function, bool includeMax = false)
		{
			for (var x = b.xMin; includeMax ? x <= b.xMax : x < b.xMax; x++)
			{
				for (var y = b.yMin; includeMax ? y <= b.yMax : y < b.yMax; y++)
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
				for (var y = b.yMin; includeMax ? y <= b.yMax : y < b.yMax; y++)
				{
					for (var z = b.zMin; includeMax ? z <= b.zMax : z < b.zMax; z++)
					{
						yield return new Vector3Int(x, y, z);
					}
				}
			}
		}

		public static bool IsInBounds(this BoundsInt b, Vector3Int p)
		{
			return b.IsInBounds(p.x, p.y, p.z);
		}

		public static bool IsInBounds(this BoundsInt b, int x, int y, int z)
		{
			return b.xMin <= x && x < b.xMax && b.yMin <= y && y < b.yMax && b.zMin <= z && z < b.zMax;
		}

		public static BoundsInt GenerateRandomBounds(this Vector3Int maxBounds, Vector3Int size)
		{
			// sizeがmaxBoundsを超えないようにチェック
			if (size.x > maxBounds.x || size.y > maxBounds.y || size.z > maxBounds.z)
			{
				throw new ArgumentException("Size cannot be larger than maxBounds.");
			}

			// ランダムな開始位置を決定
			int x = UnityEngine.Random.Range(0, maxBounds.x - size.x + 1);
			int y = UnityEngine.Random.Range(0, maxBounds.y - size.y + 1);
			int z = UnityEngine.Random.Range(0, maxBounds.z - size.z + 1);
			Vector3Int position = new Vector3Int(x, y, z);

			// 新しいBoundsIntを生成
			BoundsInt randomBounds = new BoundsInt(position, size);

			return randomBounds;
		}

		public static BoundsInt ExpandToInclude(this BoundsInt bounds, Vector3Int point) // ChatGPTでつくった
		{
			int newMinX = Mathf.Min(bounds.min.x, point.x);
			int newMinY = Mathf.Min(bounds.min.y, point.y);
			int newMinZ = Mathf.Min(bounds.min.z, point.z);

			int newMaxX = Mathf.Max(bounds.max.x, point.x);
			int newMaxY = Mathf.Max(bounds.max.y, point.y);
			int newMaxZ = Mathf.Max(bounds.max.z, point.z);

			return new BoundsInt(newMinX, newMinY, newMinZ, newMaxX - newMinX, newMaxY - newMinY, newMaxZ - newMinZ);
		}

		public static BoundsInt ExpandToInclude(this BoundsInt bounds, IEnumerable<Vector3Int> points)
		{
			foreach (var p in points)
			{
				bounds = bounds.ExpandToInclude(p);
			}

			return bounds;
		}

		#endregion

		public static BoundsInt GetBoundsIntZero(this IEnumerable<Vector3Int> points)
		{
			var min = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
			var max = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);

			foreach (var p in points)
			{
				min.x = System.Math.Min(min.x, p.x);
				min.y = System.Math.Min(min.y, p.y);
				min.z = System.Math.Min(min.z, p.z);

				max.x = System.Math.Max(max.x, p.x);
				max.y = System.Math.Max(max.y, p.y);
				max.z = System.Math.Max(max.z, p.z);
			}

			return new BoundsInt(min, max - min);
		}

		/// <summary>
		/// 点群を必ず含む軸アラインドの BoundsInt を返す（最大点をインクルーシブに扱う）。
		/// 例: points が 1 点だけでも size は (1,1,1) になる。
		/// </summary>
		public static BoundsInt GetBoundsInt(this IEnumerable<Vector3Int> points)
		{
			var hasPoint = false;
			var min = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
			var max = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);

			foreach (var p in points)
			{
				hasPoint = true;

				if (p.x < min.x) min.x = p.x;
				if (p.y < min.y) min.y = p.y;
				if (p.z < min.z) min.z = p.z;

				if (p.x > max.x) max.x = p.x;
				if (p.y > max.y) max.y = p.y;
				if (p.z > max.z) max.z = p.z;
			}

			if (!hasPoint) throw new ArgumentException("The collection is empty.", nameof(points));

			// +Vector3Int.one で最大点を含め、size を 1 以上にする
			return new BoundsInt(min, max - min + Vector3Int.one);
		}

		public static IEnumerable<Vector3Int> GetBarePoints(this IEnumerable<Vector3Int> points)
		{
			foreach (var p in points)
			{
				if (IsBare(points, p))
				{
					yield return p;
				}
			}
		}

		public static IEnumerable<Vector3Int> GetInsidePoints(this IEnumerable<Vector3Int> points)
		{
			foreach (var p in points)
			{
				if (!IsBare(points, p))
				{
					yield return p;
				}
			}
		}

		public static bool IsBare(this IEnumerable<Vector3Int> points, Vector3Int p)
		{
			var bounds = points.GetBoundsIntZero();

			for (var x = -1; x <= 1; x++)
			{
				for (var y = -1; y <= 1; y++)
				{
					for (var z = -1; z <= 1; z++)
					{
						if (x == p.x && y == p.y && z == p.z)
						{
							continue;
						}

						var current = new Vector3Int(p.x + x, p.y + y, p.z + z);

						if (!bounds.IsInBounds(current))    // 範囲外に触れているなら、むき出しとする
						{
							UnityEngine.Debug.Log($"out of bounds: {current}");
							return true;
						}

						if (!points.Contains(current))
						{
							UnityEngine.Debug.Log($"not contains: {current}");
							return true;
						}
					}
				}
			}

			return false;
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

		/// <summary>
		/// 経路の中から指定した距離にある点を返す
		/// </summary>
		public static Vector3 GetPositionAtDistance(this IList<Vector3> path, float distance, bool isReversed, out int segmentIndex)
		{
			// ChatGPTで書いた

			if (path == null || path.Count < 2)
			{
				segmentIndex = -1;
				return Vector3.zero;
			}

			segmentIndex = 0;
			float accumulatedDistance = 0;

			for (int i = 0; i < path.Count - 1; i++)
			{
				int index = isReversed ? path.Count - i - 2 : i;
				int nextIndex = isReversed ? path.Count - i - 1 : i + 1;

				float segmentLength = Vector3.Distance(path[index], path[nextIndex]);
				if (accumulatedDistance + segmentLength >= distance)
				{
					segmentIndex = index;
					float t = (distance - accumulatedDistance) / segmentLength;
					return Vector3.Lerp(path[index], path[nextIndex], t);
				}
				accumulatedDistance += segmentLength;
			}

			segmentIndex = isReversed ? 0 : path.Count - 2;
			return path[isReversed ? 0 : path.Count - 1];
		}

		/// <summary>
		/// 経路の中から指定した距離にある点を返す。終点か経路上にいない場合はfalseを返す
		/// </summary>
		public static bool TryGetPositionAtDistance(this IList<Vector2> path, float distance, bool isReversed, out Vector2 position, out int segmentIndex)
		{
			// ChatGPTで書いた

			if (path == null || path.Count < 2)
			{
				segmentIndex = -1;
				position = Vector2.zero;
				return false;
			}

			segmentIndex = 0;
			float accumulatedDistance = 0;

			for (int i = 0; i < path.Count - 1; i++)
			{
				int index = isReversed ? path.Count - i - 1 : i;
				int nextIndex = isReversed ? path.Count - i - 2 : i + 1;

				float segmentLength = Vector2.Distance(path[index], path[nextIndex]);
				if (accumulatedDistance + segmentLength >= distance)
				{
					segmentIndex = index;
					float t = (distance - accumulatedDistance) / segmentLength;
					position = Vector2.Lerp(path[index], path[nextIndex], t);
					return true;
				}
				accumulatedDistance += segmentLength;
			}

			segmentIndex = isReversed ? 0 : path.Count - 2;
			position = path[isReversed ? 0 : path.Count - 1];
			return false;
		}

		public static Vector2 GetPositionByCell(Vector2Int gridSize, Vector2Int gridPosition, Vector2 unitPerGrid)
		{
			return GetPositionByCell(gridSize.ToV3Int(), gridPosition.ToV3Int(), unitPerGrid);
		}

		public static Vector3 GetPositionByCell(Vector3Int gridSize, Vector3Int gridPosition, Vector3 unitPerGrid)
		{
			var half = unitPerGrid / 2;

			var start = new Vector3(
				-half.x * Mathf.Max(gridSize.x - 1, 0),
				-half.y * Mathf.Max(gridSize.y - 1, 0),
				-half.z * Mathf.Max(gridSize.z - 1, 0)
			);

			return start + new Vector3(unitPerGrid.x * gridPosition.x, unitPerGrid.y * gridPosition.y, unitPerGrid.z * gridPosition.z);
		}

		public static Vector2Int GetCellPosition(Vector2Int gridSize, Vector2 unitPerGrid, Vector2 position)
		{
			Vector3Int v3 = new Vector3Int(gridSize.x, gridSize.y, 0);
			var result = GetCellPosition(v3, (Vector3)unitPerGrid, (Vector3)position);

			return new Vector2Int(result.x, result.y);
		}

		public static Vector3Int GetCellPosition(Vector3Int cells, Vector3 cellSize, Vector3 position)
		{

			// マスあたりのサイズ x マス数で全体の大きさを出す
			var size = new Vector3(cellSize.x * cells.x, cellSize.y * cells.y, cellSize.z * cells.z);

			// 原点(左下後)の座標を出す
			var origin = size / -2;

			// 原点から見た時の距離
			var p = position - origin;

			return new Vector3Int((int)Mathf.Floor(p.x / cellSize.x), (int)Mathf.Floor(p.y / cellSize.y), (int)Mathf.Floor(p.z / cellSize.z));
		}
		#endregion

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

			var perAngle = rad2 * cycle / (pointCount - 1);     // -1することで最後の点が始点と重なるようにしている

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
		public static int GetRandomIndex<T>(this IReadOnlyList<T> self)
		{
			return UnityEngine.Random.Range(0, self.Count);
		}

		public static int GetRandomIndex<T>(this IList<T> self)
		{
			return UnityEngine.Random.Range(0, self.Count);
		}

		// public static int GetRandomIndex(this IList self)
		// {
		// 	return UnityEngine.Random.Range(0, self.Count);
		// }

		// public static T GetRandom<T>(this IList self)
		// {
		// 	return (T)(self[self.GetRandomIndex()]);
		// }

		public static T GetRandom<T>(this IEnumerable<T> self)
		{
			var max = self.Count();

			if (max == 0)
			{
				return default(T);
			}

			return self.ElementAt(UnityEngine.Random.Range(0, max));
		}

		public static T GetRandom<T>(this IReadOnlyList<T> self)
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

		public static IEnumerable<T> PickRandom<T>(this IList<T> self, int amount, bool allowDuplicates = false)
		{
			if (allowDuplicates)
			{
				for (int i = 0; i < amount; i++)
				{
					yield return self.PickRandom();
				}
			}
			else
			{

				var count = 0;

				while (count < amount && self.Count > 0)
				{
					var item = self.PickRandom();

					yield return item;

					count++;
				}
			}
		}

		public static List<T> GenerateRandomSort<T>(this IReadOnlyList<T> self)
		{
			var myList = new List<T>(self);

			var list = new List<T>();

			while (myList.Count > 0)
			{
				list.Add(myList.PickRandom());
			}

			return list;
		}

		public static T[] GetRandom<T>(this IReadOnlyList<T> self, int count)
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

		public static void Foreach<T>(this IEnumerable<T> self, System.Action<T> function)
		{
			foreach (var item in self)
			{
				function(item);
			}
		}

		public static void Foreach<T, R>(this IEnumerable<T> self, System.Func<T, R> function)
		{
			foreach (var item in self)
			{
				function(item);
			}
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

			var index = Random.Range(0, array.Length);

			return (T)(array.GetValue(index));
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

		/// <summary>
		/// ランダムで一つを選択する。その際に確率を考慮する
		/// </summary>
		public static T GetRandomWithWeight<T>(this IEnumerable<T> list, Func<T, float> weightSelector)
		{
			if (list == null) throw new ArgumentNullException(nameof(list));
			if (!list.Any()) throw new ArgumentException("List cannot be null or empty.", nameof(list));

			float totalWeight = list.Sum(weightSelector);
			if (totalWeight <= 0) throw new ArgumentException("Total weight must be greater than zero.");

			float randomValue = Random.value * totalWeight;
			float cumulative = 0f;

			foreach (var item in list)
			{
				cumulative += weightSelector(item);
				if (randomValue < cumulative)
				{
					return item;
				}
			}

			throw new InvalidOperationException("No item was selected. This should never happen.");
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

		public static void ToLocalScale(this Transform self, int index, float p)
		{
			var s = self.localScale;
			s[index] = p;
			self.localScale = s;
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

		public static Rect TransformRect(this Transform self, Rect rect)
		{
			var result = new Rect();
			result.min = self.TransformPoint(rect.min);
			result.max = self.TransformPoint(rect.max);
			return result;
		}

		public static Rect InverseTransformRect(this Transform self, Rect rect)
		{
			var result = new Rect();
			result.min = self.InverseTransformPoint(rect.min);
			result.max = self.InverseTransformPoint(rect.max);
			return result;
		}

		public static Quaternion LookAt(this Transform transform, Vector3 to, float delta)
		{
			transform.rotation = transform.rotation.LookAt(transform.position, to, delta);
			return transform.rotation;
		}

		public static void LookAt2D(this Transform transform, Transform to)
		{
			transform.LookAt2D(to.position);
		}

		public static void LookAt2D(this Transform transform, Vector3 to)
		{
			// 対象物へのベクトルを算出
			Vector3 toDirection = to - transform.position;
			// 対象物へ回転する
			transform.rotation = Quaternion.FromToRotation(Vector3.up, toDirection);
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

		/// <summary>
		/// あるRectTransformのワールド矩形が別のRectTransformのワールド矩形を含むかを返す。
		/// 3D空間の回転が含まれると計算が崩壊する
		/// </summary>
		public static bool Overlaps(this RectTransform self, RectTransform other)
		{
			return self.TransformRect(self.rect).Overlaps(other.TransformRect(other.rect));
		}

		public static Rect GetWorldRect(this RectTransform self)
		{
			return self.TransformRect(self.rect);
		}

		public static Bounds ConvertRectTransformToLocalSpaceBounds(this RectTransform rectTransform, Vector2 localPosition, Vector2 size)
		{
			// ローカル空間での左下と右上の点を計算
			Vector2 scaledSize = new Vector2(size.x * rectTransform.lossyScale.x, size.y * rectTransform.lossyScale.y);
			Vector3 localBottomLeft = new Vector3(localPosition.x/* - (size.x * rectTransform.pivot.x)*/, localPosition.y /*- (size.y * rectTransform.pivot.y)*/, 0f);
			Vector3 localTopRight = localBottomLeft + new Vector3(scaledSize.x, scaledSize.y, 0f);

			// ワールド空間での左下と右上の点に変換
			Vector3 worldBottomLeft = rectTransform.TransformPoint(localBottomLeft);
			Vector3 worldTopRight = rectTransform.TransformPoint(localTopRight);

			// ワールド空間でのBoundsを計算
			Bounds bounds = new Bounds();
			bounds.SetMinMax(worldBottomLeft, worldTopRight);

			return bounds;
		}

		/// <summary>
		/// RectTransformからピクセルベースでワールド空間のBoundsを計算する。
		/// </summary>
		public static Bounds CreateWorldBounds(this RectTransform rectTransform, Vector3 offsetPositionFromAnchor, Vector3 size)
		{
			var bounds = new Bounds();
			var min = rectTransform.TransformPoint(rectTransform.rect.min);
			var max = rectTransform.TransformPoint(rectTransform.rect.max);

			bounds.SetMinMax(min, max);

			var sx = bounds.size.x / rectTransform.sizeDelta.x;
			var sy = bounds.size.y / rectTransform.sizeDelta.y;

			bounds.center = new Vector3(
				bounds.center.x + sx * offsetPositionFromAnchor.x,
				bounds.center.y + sy * offsetPositionFromAnchor.y,
				bounds.center.z
			);

			bounds.size = new Vector3(
				sx * size.x,
				sy * size.y,
				bounds.size.z
			);

			return bounds;
		}

		// ToastMessage.csからコピペしてきた
		public static Vector2 ScreenPointToRectTransformPoint(Vector2 screenPoint, RectTransform rectTransform, Canvas canvas)
		{
			Vector2 localPoint;

			switch (canvas.renderMode)
			{
				case RenderMode.ScreenSpaceOverlay:
				case RenderMode.ScreenSpaceCamera:
					RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, canvas.worldCamera, out localPoint);
					break;
				case RenderMode.WorldSpace:
					Vector3 worldPoint = canvas.worldCamera.ScreenToWorldPoint(screenPoint);
					rectTransform.InverseTransformPoint(worldPoint);
					localPoint = new Vector2(worldPoint.x, worldPoint.y);
					break;
				default:
					localPoint = Vector2.zero;
					break;
			}

			return localPoint;
		}


		/// <summary>
		/// rectTransformをclampRectTransform内に収めるように位置を調整する。
		/// </summary>
		public static RectTransform Clamp(this RectTransform rectTransform, RectTransform clampRectTransform, Vector2 margin)
		{
			var myWorldRect = rectTransform.GetWorldRect();
			var clampWorldRect = clampRectTransform.GetWorldRect();

			// marginのことは一旦考えなくてOK。

			if (myWorldRect.xMin < clampWorldRect.xMin)
			{
				rectTransform.position = new Vector3(rectTransform.position.x + (clampWorldRect.xMin - myWorldRect.xMin), rectTransform.position.y, rectTransform.position.z);
			}
			else if (myWorldRect.xMax > clampWorldRect.xMax)
			{
				rectTransform.position = new Vector3(rectTransform.position.x - (myWorldRect.xMax - clampWorldRect.xMax), rectTransform.position.y, rectTransform.position.z);
			}

			if (myWorldRect.yMin < clampWorldRect.yMin)
			{
				rectTransform.position = new Vector3(rectTransform.position.x, rectTransform.position.y + (clampWorldRect.yMin - myWorldRect.yMin), rectTransform.position.z);
			}
			else if (myWorldRect.yMax > clampWorldRect.yMax)
			{
				rectTransform.position = new Vector3(rectTransform.position.x, rectTransform.position.y - (myWorldRect.yMax - clampWorldRect.yMax), rectTransform.position.z);
			}

			var clampLocalRect = rectTransform.InverseTransformRect(clampWorldRect);

			var myLocalRect = rectTransform.rect;

			if (myLocalRect.xMin < clampLocalRect.xMin + margin.x)
			{
				rectTransform.localPosition = new Vector3(rectTransform.localPosition.x + (clampLocalRect.xMin + margin.x - myLocalRect.xMin), rectTransform.localPosition.y, rectTransform.localPosition.z);
			}
			else if (myLocalRect.xMax > clampLocalRect.xMax - margin.x)
			{
				rectTransform.localPosition = new Vector3(rectTransform.localPosition.x - (myLocalRect.xMax - clampLocalRect.xMax + margin.x), rectTransform.localPosition.y, rectTransform.localPosition.z);
			}

			if (myLocalRect.yMin < clampLocalRect.yMin + margin.y)
			{
				rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y + (clampLocalRect.yMin + margin.y - myLocalRect.yMin), rectTransform.localPosition.z);
			}
			else if (myLocalRect.yMax > clampLocalRect.yMax - margin.y)
			{
				rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y - (myLocalRect.yMax - clampLocalRect.yMax + margin.y), rectTransform.localPosition.z);
			}

			return rectTransform;
		}

		public static Vector2Int GetGridPosition(this GridLayoutGroup grid, Transform child)
		{
			RectTransform rectTransform = grid.GetComponent<RectTransform>();
			if (rectTransform == null || child.parent != rectTransform) return new Vector2Int(-1, -1);

			int index = child.GetSiblingIndex();
			int columns = Mathf.Max(1, Mathf.FloorToInt(rectTransform.rect.width / grid.cellSize.x));

			int x = index % columns;
			int y = index / columns;

			return new Vector2Int(x, y);
		}

		// AIとの合作だけど良くない実装なので消した
		// public static void Clamp(this RectTransform rectTransform, RectTransform canvasRect)
		// {
		// 	Vector2 margin = Vector2.one * 50;
		// 	rectTransform.ClampV2(canvasRect, margin);
		// }

		// public static void Clamp(this RectTransform rectTransform, RectTransform canvasRect, Vector2 margin)
		// {
		// 	rectTransform.ClampV2(canvasRect, margin);
		// 	return;

		// 	if (rectTransform == null || canvasRect == null)
		// 	{
		// 		Debug.LogWarning("RectTransform or canvasRect is null.");
		// 		return;
		// 	}

		// 	if (rectTransform.parent != canvasRect)
		// 	{

		// 		// ワールド座標 → canvasRect のローカル座標へ変換
		// 		Vector3 worldPos = rectTransform.position;
		// 		Vector3 localPos = canvasRect.InverseTransformPoint(worldPos);

		// 		// lossyScale を考慮した見た目上の幅・高さ
		// 		Vector3 scale = rectTransform.lossyScale;
		// 		float width = rectTransform.rect.width * scale.x;
		// 		float height = rectTransform.rect.height * scale.y;

		// 		// rectTransform 側の pivot (例: (0, 0)なら左下、(0.5, 0.5)なら中心)
		// 		// pivot により「localPos が要素のどの位置を示すか」が変わるので、その分をオフセット計算
		// 		//   pivotOffsetX = pivot.x * width
		// 		//   left = localPos.x - pivotOffsetX
		// 		float pivotOffsetX = rectTransform.pivot.x * width;
		// 		float pivotOffsetY = rectTransform.pivot.y * height;

		// 		// 子要素の左端, 右端, 下端, 上端を求める
		// 		float left = localPos.x - pivotOffsetX;
		// 		float right = left + width;
		// 		float bottom = localPos.y - pivotOffsetY;
		// 		float top = bottom + height;

		// 		// Canvas の幅・高さ
		// 		float canvasW = canvasRect.rect.width;
		// 		float canvasH = canvasRect.rect.height;

		// 		// Canvas 側の pivot も考慮した「ローカル座標での左端～右端」(上下も同様)
		// 		float canvasLeft = -canvasW * canvasRect.pivot.x + margin.x;
		// 		float canvasRight = canvasW * (1f - canvasRect.pivot.x) - margin.x;
		// 		float canvasBottom = -canvasH * canvasRect.pivot.y + margin.y;
		// 		float canvasTop = canvasH * (1f - canvasRect.pivot.y) - margin.y;

		// 		// はみ出しチェック & 修正
		// 		if (left < canvasLeft)
		// 		{
		// 			// left と canvasLeft の差を加算する
		// 			localPos.x += (canvasLeft - left);
		// 		}
		// 		else if (right > canvasRight)
		// 		{
		// 			localPos.x -= (right - canvasRight);
		// 		}

		// 		if (bottom < canvasBottom)
		// 		{
		// 			localPos.y += (canvasBottom - bottom);
		// 		}
		// 		else if (top > canvasTop)
		// 		{
		// 			localPos.y -= (top - canvasTop);
		// 		}

		// 		// 修正したローカル座標 → ワールド座標に戻して反映
		// 		rectTransform.position = canvasRect.TransformPoint(localPos);

		// 	}
		// 	else
		// 	{

		// 		// 画面からはみ出ないように位置を調整する

		// 		var ap = rectTransform.anchoredPosition;
		// 		var size = rectTransform.sizeDelta;

		// 		if (ap.x < 0)
		// 		{
		// 			ap.x += size.x / 2;
		// 		}
		// 		else
		// 		{
		// 			ap.x -= size.x / 2;
		// 		}

		// 		if (ap.y < 0)
		// 		{
		// 			ap.y += size.y / 2;
		// 		}
		// 		else
		// 		{
		// 			ap.y -= size.y / 2;
		// 		}

		// 		rectTransform.anchoredPosition = ap;

		// 		var left = rectTransform.anchoredPosition.x - rectTransform.sizeDelta.x / 2;

		// 		if (left < -canvasRect.sizeDelta.x / 2 + margin.x)
		// 		{
		// 			rectTransform.anchoredPosition += new Vector2(-canvasRect.sizeDelta.x / 2 - left + margin.x, 0);
		// 		}

		// 		var right = rectTransform.anchoredPosition.x + rectTransform.sizeDelta.x / 2;

		// 		if (right > canvasRect.sizeDelta.x / 2 - margin.x)
		// 		{
		// 			rectTransform.anchoredPosition -= new Vector2(right - canvasRect.sizeDelta.x / 2 - margin.x, 0);
		// 		}

		// 		var top = rectTransform.anchoredPosition.y + rectTransform.sizeDelta.y / 2;

		// 		if (top > canvasRect.sizeDelta.y / 2 - margin.y)
		// 		{
		// 			rectTransform.anchoredPosition -= new Vector2(0, top - canvasRect.sizeDelta.y / 2 - margin.y);
		// 		}

		// 		var bottom = rectTransform.anchoredPosition.y - rectTransform.sizeDelta.y / 2;

		// 		if (bottom < -canvasRect.sizeDelta.y / 2 + margin.y)
		// 		{
		// 			rectTransform.anchoredPosition += new Vector2(0, -canvasRect.sizeDelta.y / 2 - bottom + margin.y);
		// 		}
		// 	}
		// }

		#endregion

		#region Component

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

		public static IEnumerable<T> GetComponents<T>(this IEnumerable<Component> self) where T : Component
		{
			return self.Select(c => c.GetComponent<T>()).Where(c => c != null);
		}

		public static T GetOrAddComponent<T>(this GameObject self) where T : Component
		{
			if (!self.TryGetComponent<T>(out var c))
			{
				c = self.AddComponent<T>();
			}

			return c;
		}

		public static T GetOrAddComponent<T>(this Component self) where T : Component
		{
			if (!self.TryGetComponent<T>(out var c))
			{
				c = self.gameObject.AddComponent<T>();
			}

			return c;
		}

		public static bool TryGetComponentInParent<T>(this Component self, out T component, bool includeInactive = false)
		{
			component = self.GetComponentInParent<T>(includeInactive);

			return component != null;
		}

		public static bool TryGetComponentInParent<T>(this GameObject self, out T component, bool includeInactive = false)
		{
			component = self.GetComponentInParent<T>(includeInactive);

			return component != null;
		}

		public static IEnumerable<A> GetComponents<A, B>(this IEnumerable<B> self) where A : Component where B : IGameObject
		{
			return self.Select(c => c.gameObject.GetComponent<A>()).Where(c => c != null);
		}

		#endregion

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

		public static bool SetActiveIfMe(this GameObject self, GameObject other)
		{
			var result = self == other;
			self.SetActive(result);
			return result;
		}

		public static bool IsPrefab(this GameObject go)
		{
			return !go.scene.isLoaded;
		}

		#endregion

		public static Color SetAlpha(this Color self, float alpha)
		{
			self.a = alpha;
			return self;
		}

		public static void SetAlpha(this Graphic self, float alpha)
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

		public static Vector2 CalcNormalizedMovement(this PointerEventData self)
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

		public static Sequence DOMoveWithJumpByHighestPoint(
			this Transform transform, Vector3 to, float duration, float jumpHeight, Ease upEase = Ease.OutQuad, Ease downEase = Ease.InQuad)
		{
			var s = DOTween.Sequence();

			var center = Vector3.Lerp(transform.position, to, 0.5f);
			var maxY = Mathf.Max(transform.position.y, to.y) + jumpHeight;
			center.y = maxY;

			var halfDuration = duration * 0.5f;

			s.Append(transform.DOMoveX(center.x, halfDuration).SetEase(Ease.Linear));
			s.Join(transform.DOMoveY(center.y, halfDuration).SetEase(upEase));
			s.Join(transform.DOMoveZ(center.z, halfDuration).SetEase(Ease.Linear));
			s.Append(transform.DOMoveX(to.x, halfDuration).SetEase(Ease.Linear));
			s.Join(transform.DOMoveY(to.y, halfDuration).SetEase(downEase));
			s.Join(transform.DOMoveZ(to.z, halfDuration).SetEase(Ease.Linear));

			return s;
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

		public static Tweener DOFade(this LineRenderer lineRenderer, float to, float duration)
		{
			var gradient = lineRenderer.colorGradient;
			var keys = gradient.alphaKeys.ToArray();
			var currents = keys.ToArray();
			var t = 0f;
			return DOTween.To(() => t, v => t = v, 1, duration).OnUpdate(() =>
			{
				for (var i = 0; i < keys.Length; i++)
				{
					currents[i].alpha = Mathf.Lerp(keys[i].alpha, to, t);
				}

				gradient.SetKeys(gradient.colorKeys, currents);
				lineRenderer.colorGradient = gradient;
			}).SetTarget(lineRenderer);
		}

		public static void SetAlphas(this LineRenderer lineRenderer, float alpha)
		{
			var gradient = lineRenderer.colorGradient;
			var keys = gradient.alphaKeys.ToArray();
			for (var i = 0; i < keys.Length; i++)
			{
				keys[i].alpha = alpha;
			}

			gradient.SetKeys(gradient.colorKeys, keys);
			lineRenderer.colorGradient = gradient;
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

		#region SpriteRenderer

		public static Tweener DOFlash(this SpriteRenderer renderer, Color addColor, float addRatio, float duration)
		{
			return renderer.material.DOFlash(addColor, addRatio, duration).SetTarget(renderer);
		}

		public static Tweener DOFlash(this Material material, Color addColor, float addRatio, float duration)
		{
			var v = 0f;
			var beforeAddColor = material.GetAddColor();
			var beforeAddRatio = material.GetAddRatio();
			return DOTween.To(() => v, val => v = val, 1f, duration).OnUpdate(() =>
			{
				material.SetAddColor(Color.Lerp(beforeAddColor, addColor, v));
				material.SetAddRatio(Mathf.Lerp(beforeAddRatio, addRatio, v));
			});
		}

		public static Color GetAddColor(this Material material)
		{
			return material.GetColor("_AddColor");
		}

		public static float GetAddRatio(this Material material)
		{
			return material.GetFloat("_AddRatio");
		}

		public static void SetAddColor(this Material material, Color color)
		{
			material.SetColor("_AddColor", color);
		}

		public static void SetAddRatio(this Material material, float ratio)
		{
			material.SetFloat("_AddRatio", ratio);
		}

		public static void ResetAdd(this Material material)
		{
			material.SetAddColor(Color.clear);
			material.SetAddRatio(0f);
		}

		public static void ResetAdd(this SpriteRenderer renderer)
		{
			renderer.material.ResetAdd();
		}

		public static void SetAddColor(this SpriteRenderer renderer, Color color)
		{
			renderer.material.SetAddColor(color);
		}

		public static void SetAddRatio(this SpriteRenderer renderer, float ratio)
		{
			renderer.material.SetAddRatio(ratio);
		}
		#endregion

		#region Dictionary
		/// <summary>
		/// Dictionary<K, V>からDictionay<V, K>を生成する。
		/// Valueがnullだったり値が被る場合は除外
		/// </summary>
		public static Dictionary<V, K> BuildReverse<K, V>(this IDictionary<K, V> self)
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

		public static void NewAndAddList<K, V>(this IDictionary<K, List<V>> self, K key, V val)
		{
			if (!self.ContainsKey(key))
			{
				self.Add(key, new List<V>());
			}

			self[key].Add(val);
		}

		public static bool Append<K, V>(this IDictionary<K, V> self, IEnumerable<KeyValuePair<K, V>> pairs)
		{
			var result = true;

			foreach (var kvp in pairs)
			{
				if (self.ContainsKey(kvp.Key))
				{
					result = false;
				}
				else
				{
					self.Add(kvp.Key, kvp.Value);
				}
			}

			return result;
		}

		public static void Foreach<K, V>(this IDictionary<K, V> self, System.Action<K, V> function)
		{
			foreach (var kvp in self)
			{
				function?.Invoke(kvp.Key, kvp.Value);
			}
		}

		public static K GetRandomKey<K, V>(this IDictionary<K, V> self)
		{
			return self.Keys.ToList().GetRandom();
		}

		public static V GetRandomValue<K, V>(this IDictionary<K, V> self)
		{
			return self[self.GetRandomKey()];
		}

		/// <summary>
		/// 既に存在する場合は上書き、存在しない場合は追加する
		/// </summary>
		/// <returns>新しくキーを追加したらtrue</returns>
		public static bool AddOrSet<K, V>(this IDictionary<K, V> self, K key, V value)
		{
			if (self.ContainsKey(key))
			{
				self[key] = value;
				return false;
			}
			else
			{
				self.Add(key, value);
				return true;
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

		public static bool AddIfNecessaryNew<K, V, S>(this IDictionary<K, S> dict, K key, V value) where S : ISet<V>, new()
		{
			if (!dict.TryGetValue(key, out var setObj))
			{
				setObj = new S();
				dict.Add(key, setObj);
			}

			return setObj.Add(value);
		}

		public static bool TrySearchKey<K, V>(this IDictionary<K, V> dict, V val, out K key)
		{
			var kvp = dict.FirstOrDefault(kvp => kvp.Value.Equals(val));

			key = kvp.Key;

			return dict.ContainsKey(key);
		}

		public static string ToDebugString<K, V>(this IDictionary<K, V> dict)
		{
			var sb = new System.Text.StringBuilder();

			foreach (var kvp in dict)
			{
				sb.AppendLine($"{kvp.Key} : {kvp.Value}");
			}

			return sb.ToString();
		}

		public static Vector2Int GetLengths<T>(this IDictionary<Vector2Int, T> dictionary)
		{
			int maxCol = 0;
			int maxRow = 0;
			foreach (var key in dictionary.Keys)
			{
				if (key.x > maxCol) maxCol = key.x;
				if (key.y > maxRow) maxRow = key.y;
			}
			return new Vector2Int(maxCol + 1, maxRow + 1);
		}


		public static Dictionary<K, V> Build<K, V>(this IEnumerable<V> values, System.Func<V, K> keySelector, bool overrideValue = false)
		{
			// LinqにはToDictionaryがあるが、overrideValueがないので自作

			var dict = new Dictionary<K, V>();

			foreach (var v in values)
			{
				var key = keySelector(v);

				if (overrideValue)
				{
					dict.AddOrSet(key, v);
				}
				else
				{
					if (dict.ContainsKey(key))
					{
						Debug.LogError($"key {key} is already exists");
					}
					else
					{
						dict.Add(key, v);
					}
				}
			}

			return dict;
		}

		#endregion

		#region Collider
		public static Vector3 GetBottomPosition(this Collider self)
		{
			return new Vector3(self.bounds.center.x, self.bounds.min.y, self.bounds.center.z);
		}

		public static bool IsRayHitting(this Collider self, Ray ray, float distance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return Physics.Raycast(ray, out var hit, distance, layerMask, queryTriggerInteraction) && hit.collider == self;
		}


		#endregion

		#region  Collider2D

		public static void EnsureInsideBounds(this Collider2D collider, Bounds targetBounds)
		{
			var bounds = collider.bounds;
			bounds.EnsureInsideBounds(targetBounds);
			collider.transform.position = bounds.center;
		}

		public static Bounds EnsureInsideBounds(this Bounds bounds, Bounds targetBounds)
		{
			// Bの中心がAの範囲内に収まるように計算する。
			Vector3 newCenter = bounds.center;

			float adjustedX = Mathf.Clamp(bounds.center.x, targetBounds.min.x + bounds.extents.x, targetBounds.max.x - bounds.extents.x);
			float adjustedY = Mathf.Clamp(bounds.center.y, targetBounds.min.y + bounds.extents.y, targetBounds.max.y - bounds.extents.y);

			newCenter = new Vector3(adjustedX, adjustedY, bounds.center.z);
			bounds.center = newCenter;

			return bounds;
		}

		#endregion

		#region LayerMask
		// LayerMaskにレイヤーを追加する関数 by ChatGPT
		public static LayerMask AddToLayerMask(this LayerMask original, params string[] layerNamesToAdd)
		{
			LayerMask maskToAdd = LayerMask.GetMask(layerNamesToAdd);
			original.value = original.value | maskToAdd.value;
			return original;
		}

		// LayerMaskからレイヤーを削除する関数 by ChatGPT
		public static LayerMask RemoveFromLayerMask(this LayerMask original, params string[] layerNamesToRemove)
		{
			LayerMask maskToRemove = LayerMask.GetMask(layerNamesToRemove);
			original.value = original.value & ~maskToRemove.value;
			return original;
		}
		#endregion

		public static IEnumerable<T> FindAbove<T>(this Collider self, float height, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return Physics.OverlapBox(self.bounds.center + Vector3.up * self.bounds.size.y, self.bounds.extents, self.transform.rotation, layerMask, queryTriggerInteraction).Select(c => c.GetComponent<T>());
		}

		public static Collider[] Overlap(this BoxCollider self, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
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

		#region Vector2
		public static Vector2 RandomVector2(Vector2 center, float radius)
		{
			var d = new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f)).normalized;

			return center + d * Random.Range(0f, radius);
		}
		#endregion

		#region Vector3

		public static Vector3 RandomVector3FromZero(this Vector3 self)
		{
			return RandomVector3(Vector3.zero, self);
		}

		public static Vector3 RandomVector3WithMinus(this Vector3 self)
		{
			return RandomVector3(-self, self);
		}

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

		public static Transform GetClosest(this Vector3 worldPoint, IEnumerable<Transform> transforms)
		{
			return transforms.OrderBy(t => Vector3.Distance(worldPoint, t.position)).FirstOrDefault();
		}

		public static bool IsCloser(this Vector3 from, Vector3 close, Vector3 far)
		{
			return Vector3.Distance(from, close) < Vector3.Distance(from, far);
		}

		public static Vector3 Direction(this Vector3 forward, Vector3 direction)
		{
			Vector3 perpendicular = Vector3.Cross(forward, direction).normalized;
			Vector3 desiredVector = Vector3.Cross(perpendicular, direction).normalized;

			return desiredVector;
		}

		/// <summary>
		/// 指定した軸を中心にベクトルを回転させる拡張メソッド
		/// </summary>
		/// <param name="vector">回転させるベクトル</param>
		/// <param name="angle">回転角度（度単位）</param>
		/// <param name="axis">回転軸</param>
		/// <returns>回転後のベクトル</returns>
		public static Vector3 RotateAroundAxis(this Vector3 vector, float angle, Vector3 axis)
		{
			return Quaternion.AngleAxis(angle, axis) * vector;
		}

		/// <summary>
		/// とあるベクトルに対して角度をつけたベクトル
		/// </summary>
		/// <param name="index">x, y, zのindex</param>
		/// <returns></returns>
		public static Vector3 RotateVector(this Vector3 vector, float degrees, int index = 2)
		{
			Vector3 v = vector;
			v[index] = degrees;
			Quaternion rotation = Quaternion.Euler(v);
			return rotation * vector;
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

		public static void Foreach(this Vector3Int from, Vector3Int to, System.Action<Vector3> callback)
		{
			var signZ = to.z - from.z >= 0 ? 1 : -1;
			var countZ = Mathf.Abs(to.z - from.z);
			do
			{
				var signY = to.y - from.y >= 0 ? 1 : -1;
				var countY = Mathf.Abs(to.y - from.y);
				do
				{
					var signX = to.x - from.x >= 0 ? 1 : -1;
					var countX = Mathf.Abs(to.x - from.x);
					do
					{
						callback(new Vector3(from.x, from.y, from.z));
					} while (countX-- > 0);
				} while (countY-- > 0);
			} while (countZ-- > 0);
		}

		/// <summary>
		/// 各軸の回転ステップを Vector3Int(x, y, z) で指定し、整数のみで回転
		/// </summary>
		/// <param name="source">回転対象</param>
		/// <param name="center">回転の中心座標</param>
		/// <param name="rotation">各軸の回転ステップ（0～3）</param>
		public static IEnumerable<Vector3Int> RotateAround90(
			this IEnumerable<Vector3Int> source,
			Vector3Int center,
			Vector3Int rotation)
		{
			int xSteps = ((rotation.x % 4) + 4) % 4;
			int ySteps = ((rotation.y % 4) + 4) % 4;
			int zSteps = ((rotation.z % 4) + 4) % 4;

			foreach (var point in source)
			{
				Vector3Int offset = point - center;
				Vector3Int rotated = offset;

				if (xSteps != 0) rotated = RotateX(rotated, xSteps);
				if (ySteps != 0) rotated = RotateY(rotated, ySteps);
				if (zSteps != 0) rotated = RotateZ(rotated, zSteps);

				yield return rotated + center;
			}

			Vector3Int RotateX(Vector3Int v, int s)
			{
				switch (s)
				{
					case 1: return new Vector3Int(v.x, -v.z, v.y);
					case 2: return new Vector3Int(v.x, -v.y, -v.z);
					case 3: return new Vector3Int(v.x, v.z, -v.y);
					default: return v;
				}
			}

			Vector3Int RotateY(Vector3Int v, int s)
			{
				switch (s)
				{
					case 1: return new Vector3Int(v.z, v.y, -v.x);
					case 2: return new Vector3Int(-v.x, v.y, -v.z);
					case 3: return new Vector3Int(-v.z, v.y, v.x);
					default: return v;
				}
			}

			Vector3Int RotateZ(Vector3Int v, int s)
			{
				switch (s)
				{
					case 1: return new Vector3Int(-v.y, v.x, v.z);
					case 2: return new Vector3Int(-v.x, -v.y, v.z);
					case 3: return new Vector3Int(v.y, -v.x, v.z);
					default: return v;
				}
			}
		}
		#endregion

		#region Rect

		public static Vector2 GetRandomPoint(this Rect rect, bool includeMax = false)
		{
			return new Vector2(
				Random.Range(rect.min.x, rect.max.x + (includeMax ? 1 : 0)),
				Random.Range(rect.min.y, rect.max.y + (includeMax ? 1 : 0))
			);
		}

		public static Rect Encapsulate(this Rect original, Rect other)
		{
			float minX = Mathf.Min(original.xMin, other.xMin);
			float minY = Mathf.Min(original.yMin, other.yMin);
			float maxX = Mathf.Max(original.xMax, other.xMax);
			float maxY = Mathf.Max(original.yMax, other.yMax);

			return new Rect(minX, minY, maxX - minX, maxY - minY);
		}

		public static Rect EncapsulateAll(this IEnumerable<Rect> rects)
		{
			bool hasValue = false;
			Rect encapsulatedRect = new Rect();

			foreach (var rect in rects)
			{
				if (!hasValue)
				{
					encapsulatedRect = rect;
					hasValue = true;
				}
				else
				{
					encapsulatedRect = encapsulatedRect.Encapsulate(rect);
				}
			}

			if (!hasValue)
			{
				// このケースは、rectsが空の場合に適用されます。
				// 必要に応じて、適切なデフォルト値を返すか、例外を投げるかを決定してください。
				return new Rect(); // ここではデフォルトのRectを返しています。
			}

			return encapsulatedRect;
		}

		public static Vector2 GetMaxSizeToFitInsideVector2(this Rect subject, Rect container)
		{
			// 小さい方のスケールを選択して、アスペクト比を保持しながらsubjectをスケール
			float scale = subject.GetMaxSizeToFitInside(container);

			// スケーリングされたsubjectの新しい大きさを計算
			Vector2 scaledSize = new Vector2(subject.width * scale, subject.height * scale);

			return scaledSize;
		}

		public static float GetMaxSizeToFitInside(this Rect subject, Rect container)
		{
			// containerの大きさに対してsubjectがどれだけスケールされるべきか計算
			float widthScale = container.width / subject.width;
			float heightScale = container.height / subject.height;

			// 小さい方のスケールを選択して、アスペクト比を保持しながらsubjectをスケール
			return Mathf.Min(widthScale, heightScale);
		}

		#endregion

		#region RectInt

		public static Vector2Int GetRandomPoint(this RectInt rect, bool includeMax = false)
		{
			return new Vector2Int(
				Random.Range(rect.min.x, rect.max.x + (includeMax ? 1 : 0)),
				Random.Range(rect.min.y, rect.max.y + (includeMax ? 1 : 0))
			);
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
			// newする実装ではなくする
			// var points = new List<Vector3>();

			// foreach (var b in self)
			// {
			// 	points.Add(b.min);
			// 	points.Add(b.max);
			// }

			// return points.GetBounds();


			var minX = float.MaxValue;
			var minY = float.MaxValue;
			var minZ = float.MaxValue;

			var maxX = float.MinValue;
			var maxY = float.MinValue;
			var maxZ = float.MinValue;

			foreach (var p in self)
			{
				if (minX > p.min.x) minX = p.min.x;
				if (minY > p.min.y) minY = p.min.y;
				if (minZ > p.min.z) minZ = p.min.z;

				if (maxX < p.max.x) maxX = p.max.x;
				if (maxY < p.max.y) maxY = p.max.y;
				if (maxZ < p.max.z) maxZ = p.max.z;
			}

			var size = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
			var center = new Vector3(minX + size.x / 2, minY + size.y / 2, minZ + size.z / 2);

			return new Bounds(center, size);
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

		#region Gizmos
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
			var a = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
			var b = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
			Gizmos.DrawCube(a, b);
		}

		public static void DrawGizmosPoint(Vector3 point, float extents = 0.5f)
		{
			Gizmos.DrawLine(point + Vector3.down * extents, point + Vector3.up * extents);
			Gizmos.DrawLine(point + Vector3.left * extents, point + Vector3.right * extents);
			Gizmos.DrawLine(point + Vector3.back * extents, point + Vector3.forward * extents);
		}

		public static void DrawLineGizmos(this IList<Vector2> points)
		{
			for (var i = 0; i < points.Count - 1; i++)
			{
				var current = points[i];
				var next = points[i + 1];

				Gizmos.DrawLine(current, next);
			}
		}

		public static void DrawLineGizmos(this IList<Vector3> points)
		{
			for (var i = 0; i < points.Count - 1; i++)
			{
				var current = points[i];
				var next = points[i + 1];

				Gizmos.DrawLine(current, next);
			}
		}
		#endregion

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

		public static IEnumerator CoPlayAndWait(this Animator self, string stateName, float normalizedWaitTime = 1, /*System.Action onComplete = null,*/ int layerIndex = 0)
		{
			self.Play(stateName, layerIndex);

			yield return null;

			while (self.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName) && self.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime < normalizedWaitTime)
			{
				yield return null;
			}

			// コルーチンでの使用を想定しているので不要。callbackにしたいなら別の関数作ったほうが良さそう
			// onComplete?.Invoke();
		}

		public static IEnumerator CoPlayAndWait(this Animator self, string stateName, float[] normalizedWaitTimes, System.Action onComplete = null, int layerIndex = 0)
		{
			self.Play(stateName, layerIndex);

			yield return null;

			foreach (var normalizedWaitTime in normalizedWaitTimes)
			{
				while (self.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName) && self.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime < normalizedWaitTime)
				{
					yield return null;
				}

				onComplete?.Invoke();
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

		/// <summary>
		/// 指定されたアニメーションを再生して、且つ元のアニメーションステートにクロスフェードさせる
		/// </summary>
		public static Coroutine InsertAnimation(this MonoBehaviour self, Animator animator, string stateName, int layerIndex = 0)
		{
			var stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

			var hash = stateInfo.fullPathHash;
			var normalizedTime = stateInfo.normalizedTime;

			animator.Play(stateName, layerIndex);
			return self.StartCoroutine(CoInsert());

			IEnumerator CoInsert()
			{
				yield return null;
				animator.CrossFade(hash, 1f, layerIndex, normalizedTime);
			}
		}

		public static List<Rigidbody> GetRigidbodies(this Animator animator)
		{
			var list = new List<Rigidbody>();

			foreach (HumanBodyBones bone in System.Enum.GetValues(typeof(HumanBodyBones)))
			{
				if (bone == HumanBodyBones.LastBone)
				{
					continue;
				}

				var t = animator.GetBoneTransform(bone);
				if (t != null)
				{
					if (t.TryGetComponent<Rigidbody>(out var rb))
					{
						list.Add(rb);
					}
				}
			}

			return list;
		}

		public static List<Collider> GetColliders(this Animator animator)
		{
			var list = new List<Collider>();

			foreach (HumanBodyBones bone in System.Enum.GetValues(typeof(HumanBodyBones)))
			{
				if (bone == HumanBodyBones.LastBone)
				{
					continue;
				}

				var t = animator.GetBoneTransform(bone);
				if (t != null)
				{
					if (t.TryGetComponent<Collider>(out var c))
					{
						list.Add(c);
					}
				}
			}

			return list;
		}

		#endregion

		#region Vector2Int

		public static HashSet<Vector2Int> GetBorder(this IEnumerable<Vector2Int> points, bool diagonalCorner = false)
		{
			// HashSet.TryGetValueは.Net４.7.2から使えるらしいのでHashSetはむりぽ https://docs.microsoft.com/ja-jp/dotnet/api/system.collections.generic.hashset-1.trygetvalue?view=net-6.0#system-collections-generic-hashset-1-trygetvalue(-0-0@)
			var map = new Dictionary<Vector2Int, bool>();   //  new HashSet<Vector2Int>();

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

		public static RectInt GetRectInt(this IEnumerable<Vector2Int> points)
		{
			var minX = int.MaxValue;
			var minY = int.MaxValue;
			var maxX = int.MinValue;
			var maxY = int.MinValue;

			foreach (var point in points)
			{
				minX = Mathf.Min(minX, point.x);
				minY = Mathf.Min(minY, point.y);
				maxX = Mathf.Max(maxX, point.x);
				maxY = Mathf.Max(maxY, point.y);
			}

			return new RectInt(minX, minY, maxX - minX, maxY - minY);
		}

		public static HashSet<Vector2Int> GetInside(this IEnumerable<Vector2Int> points)
		{
			var results = new HashSet<Vector2Int>();

			var rect = points.GetRectInt();

			var dict = new Dictionary<Vector2Int, bool>();

			foreach (var p in points)
			{
				dict.TryAdd(p, true);
			}

			foreach (var p in points)
			{
				if (rect.xMin < p.x && p.x < rect.xMax && rect.yMin < p.y && p.y < rect.yMax)
				{
					var isInside = true;
					for (var x = -1; isInside && x <= 1; x++)
					{
						for (var y = -1; isInside && y <= 1; y++)
						{
							if (x == 0 && y == 0)
							{
								continue;
							}

							if (!dict.ContainsKey(p + new Vector2Int(x, y)))
							{
								isInside = false;
								break;
							}
						}
					}

					if (isInside)
					{
						results.Add(p);
					}
				}
			}

			return results;
		}


		#endregion


		#region LayerMask
		public static bool Contains(this LayerMask layerMask, int layer)
		{
			return ((1 << layer) & layerMask) != 0;
		}
		#endregion

		#region NavMeshAgent
		public static bool IsReachedDestinationOrGaveUp(this UnityEngine.AI.NavMeshAgent self)
		{

			if (!self.pathPending)
			{
				if (self.remainingDistance <= self.stoppingDistance)
				{
					if (!self.hasPath || self.velocity.sqrMagnitude == 0f)
					{
						return true;
					}
				}
			}

			return false;
		}
		#endregion

		public static Vector3Int ToV3IntXZ(this Vector2Int self)
		{
			return new Vector3Int(self.x, 0, self.y);
		}

		public static Vector3Int ToV3Int(this Vector2Int self, int z = 0)
		{
			return new Vector3Int(self.x, self.y, z);
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

			for (var i = 0; i < points.Count - 1; i++)
			{
				// 上向きの辺、下向きの辺によって処理が分かれる。
				// 上向きの辺。点Pがy軸方向について、始点と終点の間にある。ただし、終点は含まない。(ルール1)
				if ((points[i].y <= target.y) && (points[i + 1].y > target.y))
				{
					// 辺は点pよりも右側にある。ただし、重ならない。(ルール4)
					// 辺が点pと同じ高さになる位置を特定し、その時のxの値と点pのxの値を比較する。
					var vt = (target.y - points[i].y) / (points[i + 1].y - points[i].y);
					if (target.x < (points[i].x + (vt * (points[i + 1].x - points[i].x))))
					{
						++wn;  //ここが重要。上向きの辺と交差した場合は+1
					}
				}
				// 下向きの辺。点Pがy軸方向について、始点と終点の間にある。ただし、始点は含まない。(ルール2)
				else if ((points[i].y > target.y) && (points[i + 1].y <= target.y))
				{
					// 辺は点pよりも右側にある。ただし、重ならない。(ルール4)
					// 辺が点pと同じ高さになる位置を特定し、その時のxの値と点pのxの値を比較する。
					var vt = (target.y - points[i].y) / (points[i + 1].y - points[i].y);
					if (target.x < (points[i].x + (vt * (points[i + 1].x - points[i].x))))
					{
						--wn;  //ここが重要。下向きの辺と交差した場合は-1
					}
				}
				// ルール1,ルール2を確認することで、ルール3も確認できている。
			}

			return wn != 0;
		}

		public static bool TryGetDirectionFromTouch(
			this Camera camera, Vector2 fromScreenPoint, Vector2 toScreenPoint, out Vector3 direction, float distance = 1000f, params string[] layerNames)
		{
			return TryGetDirectionFromTouch(camera, fromScreenPoint, toScreenPoint, out direction, LayerMask.GetMask(layerNames), distance);
		}

		#region LayoutGroup

		public static void DOLayout(this LayoutGroup layoutGroup, float duration, Ease ease = Ease.Linear)
		{
			var before = new Dictionary<Transform, Vector3>();

			foreach (Transform t in layoutGroup.transform)
			{
				before.Add(t, t.position);
			}

			layoutGroup.enabled = true;
			LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.transform as RectTransform);

			var after = new Dictionary<Transform, Vector3>();
			
			foreach (Transform t in layoutGroup.transform)
			{
				after.Add(t, t.position);
			}
			
			layoutGroup.enabled = false;

			foreach (Transform t in layoutGroup.transform)
			{
				if (before.TryGetValue(t, out var beforePos) && after.TryGetValue(t, out var afterPos))
				{
					var tween = t.DOMove(afterPos, duration).SetEase(ease).From(beforePos);
					// yield return tween;
				}
			}
		}

		#endregion

		#region DOTween

		/// <summary>
		/// DOMoveの際にそれぞれの軸毎に別のEaseを指定できるようにした関数。OnUpdateとSetTargetは使用済みです。
		/// </summary>
		public static Tweener DOMove(this Transform transform, Vector3 to, float duration, Ease easeX, Ease easeY, Ease easeZ)
		{
			var from = transform.position;
			var current = 0f;
			return DOTween.To(() => current, v => current = v, 1, duration).OnUpdate(() =>
			{
				var p = new Vector3(
					DOVirtual.EasedValue(from.x, to.x, current, easeX),
					DOVirtual.EasedValue(from.y, to.y, current, easeY),
					DOVirtual.EasedValue(from.z, to.z, current, easeZ)
				);

				transform.position = p;
			}).SetTarget(transform);
		}

		/// <summary>
		/// DOAnchorPosの際にそれぞれの軸毎に別のEaseを指定できるようにした関数。OnUpdateとSetTargetは使用済みです。
		/// </summary> 
		public static Tweener DOAnchorPos(this RectTransform rectTransform, Vector2 to, float duration, Ease easeX, Ease easeY)
		{
			var from = rectTransform.anchoredPosition;
			var current = 0f;
			return DOTween.To(() => current, v => current = v, 1, duration).OnUpdate(() =>
			{
				var p = new Vector2(
					DOVirtual.EasedValue(from.x, to.x, current, easeX),
					DOVirtual.EasedValue(from.y, to.y, current, easeY)
				);

				rectTransform.anchoredPosition = p;
			}).SetTarget(rectTransform);
		}

		/// <summary>
		/// DOScaleをした時にOnUpdateでLayoutRebuilder.ForceRebuildLayoutImmediateを呼ぶ関数。OnUpdateは使用済みとなる。主にLayoutGroupのuseChildScaleをアニメで使う場合。
		/// </summary>		
		public static TweenerCore<Vector3, Vector3, VectorOptions> DOScaleWithRebuildOnUpdate(this RectTransform rectTransform, Vector3 to, float duration)
		{
			return rectTransform.DOScale(to, duration).OnUpdate(() => LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform.parent as RectTransform));
		}

		public static TweenerCore<Vector3, Vector3, VectorOptions> DOScaleWithRebuildOnUpdate(this RectTransform rectTransform, float to, float duration)
		{
			return rectTransform.DOScale(to, duration).OnUpdate(() => LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform.parent as RectTransform));
		}

		#endregion

		#region Camera
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

		public static Vector3 TryRectTransformToWorldPosition(this Camera camera, RectTransform rectTransform, float distance = 0)
		{
			Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(camera, rectTransform.position);  // positionを好きに指定したいを考える
			screenPoint.z = distance;
			return camera.ScreenToWorldPoint(screenPoint);
		}

		public static Vector3 ConvertToWorldPoint(this Camera camera, Graphic graphic)
		{
			return graphic.ConvertToCameraWorldPoint(camera);
		}

		public static Vector3 ConvertToCameraWorldPoint(this Graphic graphic, Camera camera)
		{
			// Check if Graphic and Camera are not null
			if (graphic == null || camera == null)
			{
				// Debug.LogError("Graphic and/or Camera is null.");
				return Vector3.zero;
			}

			Canvas canvas = graphic.canvas;
			RectTransform rectTransform = graphic.rectTransform;
			Vector2 screenPoint = Vector2.zero;
			Vector3 worldPoint = Vector3.zero;

			// Check if the Canvas render mode is ScreenSpaceOverlay
			if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				// Convert RectTransform position to screen position
				screenPoint = RectTransformUtility.WorldToScreenPoint(null, rectTransform.position);
			}

			// Check if the Canvas render mode is ScreenSpaceCamera or WorldSpace
			else if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
			{
				// Convert RectTransform position to screen position
				screenPoint = RectTransformUtility.WorldToScreenPoint(camera, rectTransform.position);
			}

			// Convert the screen point to world point in the camera
			worldPoint = camera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, camera.nearClipPlane));

			return worldPoint;
		}

		public static void AdjustDistanceToMatchRatio(this Camera camera, Bounds bounds, float targetRatio, bool useVertical = true)
		{
			float objectSize = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z) * 2f;
			float fov = camera.fieldOfView * Mathf.Deg2Rad;
			float aspectRatio = camera.aspect;

			float requiredDistance;

			if (useVertical)
			{
				float screenHeightAtDistance = 2.0f * Mathf.Tan(fov / 2.0f);  // カメラの視野角に基づく画面の縦の大きさ
				requiredDistance = (objectSize / targetRatio) / screenHeightAtDistance;
			}
			else
			{
				float screenWidthAtDistance = 2.0f * Mathf.Tan(fov / 2.0f) * aspectRatio;  // カメラの視野角に基づく画面の横の大きさ
				requiredDistance = (objectSize / targetRatio) / screenWidthAtDistance;
			}

			Vector3 direction = (bounds.center - camera.transform.position).normalized;
			camera.transform.position = bounds.center - direction * requiredDistance;
		}

		public static Vector3[] GetFrustumCorners(this Camera camera, float distance)
		{
			GetFrustumCorners(camera, distance, out var bottomLeft, out var bottomRight, out var topLeft, out var topRight);
			return new Vector3[] { bottomLeft, bottomRight, topLeft, topRight };
		}

		public static void GetFrustumCorners(this Camera camera, float distance,
			out Vector3 bottomLeft, out Vector3 bottomRight, out Vector3 topLeft, out Vector3 topRight)
		{
			bottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, distance));
			bottomRight = camera.ViewportToWorldPoint(new Vector3(1, 0, distance));
			topLeft = camera.ViewportToWorldPoint(new Vector3(0, 1, distance));
			topRight = camera.ViewportToWorldPoint(new Vector3(1, 1, distance));
		}

		#endregion

		#region LineRenderer
		public static float GetLineLength(this LineRenderer lineRenderer)
		{
			float length = 0;

			for (int i = 1; i < lineRenderer.positionCount; i++)
			{
				length += Vector3.Distance(lineRenderer.GetPosition(i - 1), lineRenderer.GetPosition(i));
			}

			return length;
		}
		#endregion

		#region Generic
		public static bool IsDefault<T>(T value)
		{
			return EqualityComparer<T>.Default.Equals(value, default(T));
		}

		public static bool IsEquals<T>(this T value, T other)
		{
			return EqualityComparer<T>.Default.Equals(value, other);
		}
		#endregion

		/// <summary>
		/// Rayが仮想の高さの平面と交わる位置を返す
		/// </summary>
		public static bool TryGetPositionOnRay(this Ray ray, int vector3Index, float targetPoint, out Vector3 position)
		{
			// Rayの方向ベクトルを正規化する
			Vector3 direction = ray.direction.normalized;

			// Rayの始点と目標の高さの差分を求める
			float delta = targetPoint - ray.origin[vector3Index];

			// Rayが指定された高さに到達しない場合はfalseを返す
			if (Mathf.Approximately(delta, 0f) || Mathf.Approximately(direction[vector3Index], 0f))
			{
				position = Vector3.zero;
				return false;
			}

			// Rayの始点から、指定された高さまでの距離を計算する
			float distance = delta / direction[vector3Index];

			// Rayの始点に、高さが指定された高さとなる位置を求める
			position = ray.origin + direction * distance;

			// XZ座標を返す
			position[vector3Index] = 0f;
			return true;
		}

		public static bool TryGetWorldPositionFromPressCurrentRaycast(this PointerEventData eventData, int vector3Index, out Vector3 worldPosition)
		{
			worldPosition = Vector3.zero;

			// pointerCurrentRaycastのgameObjectが存在する場合
			if (eventData.pointerCurrentRaycast.gameObject != null)
			{
				worldPosition = eventData.pointerCurrentRaycast.worldPosition;
				worldPosition[vector3Index] = eventData.pointerCurrentRaycast.gameObject.transform.position[vector3Index];
				return true;
			}
			// pointerCurrentRaycastのgameObjectが存在しない場合
			else if (eventData.pointerPressRaycast.gameObject != null)
			{
				var target = eventData.pointerPressRaycast.gameObject.transform.position[vector3Index];
				Ray ray = eventData.pressEventCamera.ScreenPointToRay(eventData.position);

				if (TryGetPositionOnRay(ray, vector3Index, target, out worldPosition))
				{
					return true;
				}
			}

			return false;
		}

		public static bool IsLeftClick(this PointerEventData eventData)
		{
			return eventData.button == PointerEventData.InputButton.Left;
		}

		public static bool IsRightClick(this PointerEventData eventData)
		{
			return eventData.button == PointerEventData.InputButton.Right;
		}

		public static bool AddRange<T>(this HashSet<T> self, IEnumerable<T> collection)
		{
			return self.Add(collection);
		}

		public static bool Add<T>(this HashSet<T> self, IEnumerable<T> collection)
		{
			var result = true;

			foreach (var item in collection)
			{
				result &= self.Add(item);
			}

			return result;
		}


		public static class CustomDebug
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

			public static void DrawOctahedron(Vector3 position, float size, Color color, float duration = 1f)
			{
				float scale = size / Mathf.Sqrt(2f);

				Vector3 vertex1 = position + new Vector3(0f, scale, 0f);
				Vector3 vertex2 = position + new Vector3(scale, 0f, 0f);
				Vector3 vertex3 = position + new Vector3(0f, 0f, scale);
				Vector3 vertex4 = position + new Vector3(-scale, 0f, 0f);
				Vector3 vertex5 = position + new Vector3(0f, 0f, -scale);
				Vector3 vertex6 = position + new Vector3(0f, -scale, 0f);

				UnityEngine.Debug.DrawLine(vertex1, vertex2, color, duration);
				UnityEngine.Debug.DrawLine(vertex2, vertex3, color, duration);
				UnityEngine.Debug.DrawLine(vertex3, vertex1, color, duration);

				UnityEngine.Debug.DrawLine(vertex1, vertex4, color, duration);
				UnityEngine.Debug.DrawLine(vertex4, vertex3, color, duration);
				UnityEngine.Debug.DrawLine(vertex3, vertex5, color, duration);

				UnityEngine.Debug.DrawLine(vertex5, vertex3, color, duration);
				UnityEngine.Debug.DrawLine(vertex3, vertex4, color, duration);
				UnityEngine.Debug.DrawLine(vertex4, vertex6, color, duration);

				UnityEngine.Debug.DrawLine(vertex6, vertex4, color, duration);
				UnityEngine.Debug.DrawLine(vertex4, vertex1, color, duration);
				UnityEngine.Debug.DrawLine(vertex1, vertex5, color, duration);

				UnityEngine.Debug.DrawLine(vertex5, vertex1, color, duration);
				UnityEngine.Debug.DrawLine(vertex1, vertex2, color, duration);
				UnityEngine.Debug.DrawLine(vertex2, vertex6, color, duration);

				UnityEngine.Debug.DrawLine(vertex6, vertex2, color, duration);
				UnityEngine.Debug.DrawLine(vertex2, vertex5, color, duration);
				UnityEngine.Debug.DrawLine(vertex5, vertex6, color, duration);
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
		private GUIStyle _style;
		private float _fontSize = 0.05f;

		public IMGrid()
		{

		}

		public IMGrid SetSize(int width, int height)
		{
			_width = width;
			_height = height;
			return this;
		}

		public IMGrid SetFontSize(float size)
		{
			Init();
			_style.fontSize = (int)(Mathf.Min(Screen.width, Screen.height) * size);
			return this;
		}

		public IMGrid Foreach(ForeachDelegate callback)
		{
			for (var x = 0; x < _width; x++)
			{
				for (var y = 0; y < _height; y++)
				{
					callback.Invoke(x, y);
				}
			}

			return this;
		}

		public IMGrid Button(int x, int y, string label, System.Action onClick)
		{
			if (Button(x, y, label))
			{
				onClick?.Invoke();
			}

			return this;
		}

		public bool Button(int x, int y, string label)
		{
			Init();
			_style.fontSize = (int)(Mathf.Min(Screen.width, Screen.height) * _fontSize);
			return GUI.Button(new Rect(_gridWidth * x, _gridHeight * y, _gridWidth, _gridHeight), label, _style);
		}

		private void Init()
		{
			if (_style == null)
			{
				_style = new GUIStyle(GUI.skin.button);
			}
		}
	}
}
