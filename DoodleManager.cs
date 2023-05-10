namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Events;

	public class DoodleManager : DoodleManager<DoodleManager.TouchEvent>
	{
		public interface ITouchEvent
		{
			LineRenderer line { get; }
			List<Vector2> points { get; }
			void AddPoint(Vector3 point);
			void SetLine(LineRenderer line);
		}

		public class TouchEvent : ITouchEvent
		{
			public LineRenderer line { get; private set; }
			public EdgeCollider2D collider { get; private set; }

			public List<Vector2> points { get; private set; }

			public TouchEvent()
			{
				points = new List<Vector2>();
			}

			public void SetLine(LineRenderer line)
			{
				this.line = line;
				collider = line.gameObject.AddComponent<EdgeCollider2D>();
			}

			public void AddPoint(Vector3 point)
			{
				points.Add(point);

				line.positionCount += 1;
				line.SetPosition(line.positionCount - 1, point);
				collider.SetPoints(points);
			}
		}
	}

	public class DoodleManager<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler where T : DoodleManager.ITouchEvent, new()
	{
		
		[SerializeField, Header("線のプレハブを設定する")]
		private LineRenderer _linePrefab;

		[SerializeField]
		private Transform _lineParent;

		[SerializeField]
		private UnityEvent<int> _onBeginPoint;

		public UnityEvent<int> onBeginPoint => _onBeginPoint;

		private HashSet<T> _touched = new HashSet<T>();

		private Dictionary<int, T> _touching = new Dictionary<int, T>();

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
			if (!_touching.ContainsKey(pointerId))
			{
				var line = Instantiate(_linePrefab, _lineParent);
				line.positionCount = 0;
				var touchEvent = GetTouchEvent();
				touchEvent.SetLine(line);
				_touching.Add(pointerId, touchEvent);
				_onBeginPoint?.Invoke(pointerId);
				return true;
			}

			return false;
		}

		public void AddPoint(int pointerId, Vector3 point)
		{
			if (_touching.TryGetValue(pointerId, out var drawnMesh))
			{
				drawnMesh.AddPoint(point);
			}
		}

		public void EndPoint(int pointerId)
		{
			if (_touching.ContainsKey(pointerId))
			{
				_touched.Add(_touching[pointerId]);
				_lastDrawPoints = _touching[pointerId].points;
				_touching.Remove(pointerId);
			}
		}

		public LineRenderer GetDrawingLine(int pointerId)
		{
			_touching.TryGetValue(pointerId, out var mesh);
			return mesh.line;
		}

		protected virtual T GetTouchEvent()
		{
			return new T();
		}
	}

}