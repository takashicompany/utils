namespace takashicompany.Unity
{
	using UnityEngine;
	using System.Linq;
	using System.Collections.Generic;

	public class DoodleCollider2DMaker : DoodleManager<DoodleCollider2DMaker.DrawnMesh>
	{
		[SerializeField, Header("コライダーの線の太さ")]
		private float _lineColliderEdgeRadius = 0.1f;

		[System.Serializable]
		public class DrawnMesh : DoodleManager.ITouchEvent
		{
			public LineRenderer line { get; private set; }
			public EdgeCollider2D collider { get; private set; }

			public List<Vector2> points { get; private set; }

			private List<Vector2> _tempPoints = new List<Vector2>();

			private float _colliderEdgeRadius;

			public DrawnMesh()
			{
				points = new List<Vector2>();
			}

			public void SetLine(LineRenderer line)
			{
				this.line = line;
				collider = line.gameObject.AddComponent<EdgeCollider2D>();
				SetColliderRadius(_colliderEdgeRadius);
			}

			public void SetColliderRadius(float radius)
			{
				_colliderEdgeRadius = radius;

				if (collider != null)
				{
					collider.edgeRadius = _colliderEdgeRadius;
				}
			}

			public void AddPoint(Vector3 point)
			{
				points.Add(point);
				_tempPoints.Add(point);

				line.positionCount += 1;
				line.SetPosition(line.positionCount - 1, point);
				collider.SetPoints(points);
			}

			public void Reverse()
			{
				points.Reverse();
				_tempPoints.Reverse();
				line.SetPositions(points.Select(p => new Vector3(p.x, p.y, 0)).ToArray());
				collider.SetPoints(points);
			}

			public void Kieru(float distance)
			{
				// 末端から消していく
				var position = points.GetPositionAtDistance(distance, true, out var index);

				if (index < 0)
				{
					return;
				}
				
				while (index + 1 < _tempPoints.Count)
				{
					_tempPoints.RemoveAt(_tempPoints.Count - 1);
				}
				
				_tempPoints[index] = position;

				line.positionCount = index + 1;
				line.SetPosition(index, position);

				collider.SetPoints(_tempPoints);
			}

			public void Delete()
			{
				var line = this.line;

				this.line = null;
				this.collider = null;
				
				Destroy(line.gameObject);
			}
		}

		protected override DrawnMesh GetTouchEvent()
		{
			var touch = base.GetTouchEvent();
			touch.SetColliderRadius(_lineColliderEdgeRadius);
			return touch;
		}

		
	}
}