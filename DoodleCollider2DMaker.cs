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

		[SerializeField, Header("コライダーの線の太さ")]
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

		private Dictionary<int, DrawnMesh> _drawingMeshes = new Dictionary<int, DrawnMesh>();

		[SerializeField]
		private List<Vector2> _lastDrawPoints;

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (BeginPoint(eventData.pointerId))
			{
				AddPoint(eventData.pointerId, eventData.pointerPressRaycast.worldPosition);
				AddPoint(eventData.pointerId, eventData.pointerCurrentRaycast.worldPosition);
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			AddPoint(eventData.pointerId, eventData.pointerCurrentRaycast.worldPosition);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			AddPoint(eventData.pointerId, eventData.pointerCurrentRaycast.worldPosition);
			EndPoint(eventData.pointerId);
		}

		public bool BeginPoint(int pointerId)
		{
			if (!_drawingMeshes.ContainsKey(pointerId))
			{
				var line = Instantiate(_linePrefab, _lineParent);
				line.positionCount = 0;
				_drawingMeshes.Add(pointerId, new DrawnMesh(line, _lineColliderEdgeRadius));
				return true;
			}

			return false;
		}

		public void AddPoint(int pointerId, Vector3 point)
		{
			if (_drawingMeshes.TryGetValue(pointerId, out var drawnMesh))
			{
				drawnMesh.AddPoint(point);
			}
		}

		public void EndPoint(int pointerId)
		{
			if (_drawingMeshes.ContainsKey(pointerId))
			{
				_drawnMeshes.Add(_drawingMeshes[pointerId]);
				_lastDrawPoints = _drawingMeshes[pointerId].points;
				_drawingMeshes.Remove(pointerId);
			}
		}

		public LineRenderer GetDrawingLine(int pointerId)
		{
			_drawingMeshes.TryGetValue(pointerId, out var mesh);
			return mesh.line;
		}
	}
}