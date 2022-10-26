namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Events;

	public class DoodleCollider2DMaker : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		[SerializeField, Header("線のプレハブを設定する")]
		private LineRenderer _linePrefab;

		[SerializeField]
		private Transform _lineParent;

		[SerializeField]
		private float _lineColliderEdgeRadius = 0.1f;

		[System.Serializable]
		private class DrawnMesh
		{
			public LineRenderer line { get; private set; }
			public EdgeCollider2D collider { get; private set; }

			public List<Vector2> points { get; private set; }

			public DrawnMesh(LineRenderer line, float colliderEdgeRadius)
			{
				this.line = line;
				collider = line.gameObject.AddComponent<EdgeCollider2D>();
				collider.edgeRadius = colliderEdgeRadius;
				points = new List<Vector2>();
			}

			public void AddPoint(Vector3 point)
			{
				points.Add(point);

				line.positionCount += 1;
				line.SetPosition(line.positionCount - 1, point);
				collider.SetPoints(points);
			}
		}

		private HashSet<DrawnMesh> _drawnMeshes = new HashSet<DrawnMesh>();

		private Dictionary<int, DrawnMesh> currentDrawnMeshes = new Dictionary<int, DrawnMesh>();

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (!currentDrawnMeshes.ContainsKey(eventData.pointerId))
			{
				var line = Instantiate(_linePrefab, _lineParent);
				line.positionCount = 0;
				currentDrawnMeshes.Add(eventData.pointerId, new DrawnMesh(line, _lineColliderEdgeRadius));

				AddPoint(eventData.pointerId, eventData.pointerPressRaycast.worldPosition);
				AddPoint(eventData.pointerId, eventData.pointerCurrentRaycast.worldPosition);
			}
		}

		private void AddPoint(int pointerId, Vector3 point)
		{
			if (currentDrawnMeshes.TryGetValue(pointerId, out var drawnMesh))
			{
				drawnMesh.AddPoint(point);
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			AddPoint(eventData.pointerId, eventData.pointerCurrentRaycast.worldPosition);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			AddPoint(eventData.pointerId, eventData.pointerCurrentRaycast.worldPosition);

			if (currentDrawnMeshes.ContainsKey(eventData.pointerId))
			{
				_drawnMeshes.Add(currentDrawnMeshes[eventData.pointerId]);
				currentDrawnMeshes.Remove(eventData.pointerId);
			}
		}
	}
}