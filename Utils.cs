namespace TakashiCompany.Unity
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.EventSystems;
	using DG.Tweening;

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

		/// <summary>
		/// CamelCase => camel_case
		/// </summary>
		/// <returns></returns>
		public static string ToSnakeCase(this string str)
		{
			var regex = new System.Text.RegularExpressions.Regex("[a-z][A-Z]");
			return regex.Replace(str, s => $"{s.Groups[0].Value[0]}_{s.Groups[0].Value[1]}").ToLower();
		}

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

		public static Vector3 GetPointOfProgress(this IList<Vector3> path, float progress)
		{
			progress = Mathf.Clamp01(progress);

			var totalLength = path.GetTotalLength();

			var lengthOnProgress = totalLength * progress;

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
			}

			return path[path.Count - 1];
		}

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
			return new Color(Random.Range(0f, 1f), Random.Range(0, 1f), Random.Range(0, 1), 1);
		}
#endregion

		public static bool Include(this Vector3Int self, Vector3Int target)
		{
			return 0 <= target.x && target.x < self.x && 0 <= target.y && target.y < self.y && 0 <= target.z && target.z < self.z;
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
			var origin = new Vector3(gridSize.x * unitPerGrid.x, gridSize.y * unitPerGrid.y, gridSize.z * unitPerGrid.z) / 2;
			var p = position + origin;
			return new Vector3Int((int)Mathf.Floor(p.x), (int)Mathf.Floor(p.y), (int)Mathf.Floor(p.z));
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
		
#region リストをランダムで処理する
		public static int GetRandomIndex<T>(this IList<T> self)
		{
			return Random.Range(0, self.Count);
		}

		public static T GetRandom<T>(this IList<T> self)
		{
			return self[self.GetRandomIndex()];
		}

		public static T RemoveRandom<T>(this IList<T> self)
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
				list.Add(myList.RemoveRandom());
			}

			return list;
		}

		public static T[] PickRandom<T>(this IList<T> self, int count)
		{
			var result = new T[count];
			
			for (int i = 0; i < count; i++)
			{
				result[i] = self.GetRandom();
			}

			return result;
		}
#endregion

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

		public static Vector3 RandomVector3(float min, float max)
		{
			return new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
		}

		public static Quaternion RandomRotation()
		{
			return Quaternion.Euler(RandomVector3(0, 360));
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

		public static Vector3Int ToV3IntXZ(this Vector2Int self)
		{
			return new Vector3Int(self.x, 0, self.y);
		}

		public static Vector3Int ToV3Int(this Vector2Int self)
		{
			return new Vector3Int(self.x, self.y, 0);
		}

		public static Vector3 ToXZ(this Vector3 self)
		{
			return new Vector3(self.x, 0, self.y);
		}

		public static Vector3 ToV3XZ(this Vector2 self)
		{
			return new Vector3(self.x, 0, self.y);
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