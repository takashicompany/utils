namespace takashicompany.Unity
{
	using UnityEngine;
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

				line.positionCount += 1;
				line.SetPosition(line.positionCount - 1, point);
				collider.SetPoints(points);
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