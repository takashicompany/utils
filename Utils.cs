namespace TakashiCompany
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

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

		public static Vector3 GetPositionOnGrid(Vector3Int gridSize, Vector3Int gridPosition, Vector3 size)
		{
			var start = (Vector3)gridSize / -2 + size / 2;
			
			return start + new Vector3(size.x * gridPosition.x, size.y * gridPosition.y, size.z * gridPosition.z);
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
#endregion

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
	}

}