namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.EventSystems;

	public class DrawnMeshMaker : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		[SerializeField, Header("線のプレハブを設定する")]
		private LineRenderer _linePrefab;

		[SerializeField]
		private Transform _lineParent;

		private HashSet<LineRenderer> _lines = new HashSet<LineRenderer>();

		private Dictionary<int, LineRenderer> _currentLines = new Dictionary<int, LineRenderer>();

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (!_currentLines.ContainsKey(eventData.pointerId))
			{
				var line = Instantiate(_linePrefab, _lineParent);
				line.positionCount = 0;
				_currentLines.Add(eventData.pointerId, line);

				AddPoint(eventData.pointerId, eventData.pointerPressRaycast.worldPosition);
				AddPoint(eventData.pointerId, eventData.pointerCurrentRaycast.worldPosition);
			}
		}

		private void AddPoint(int pointerId, Vector3 point)
		{
			if (_currentLines.TryGetValue(pointerId, out var line))
			{
				line.positionCount += 1;
				line.SetPosition(line.positionCount - 1, point);
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			AddPoint(eventData.pointerId, eventData.pointerCurrentRaycast.worldPosition);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			AddPoint(eventData.pointerId, eventData.pointerCurrentRaycast.worldPosition);

			if (_currentLines.ContainsKey(eventData.pointerId))
			{
				_lines.Add(_currentLines[eventData.pointerId]);
				_currentLines.Remove(eventData.pointerId);
			}
		}
	}
}