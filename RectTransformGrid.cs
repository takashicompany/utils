namespace takashicompany.Unity
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.UIElements;

	public class RectTransformGrid : MonoBehaviour
	{
		[SerializeField]
		private Vector2 _cellSize = new Vector2(100, 100);

		[SerializeField]
		private Vector2 _offsetForAll = Vector2.zero;

		[SerializeField]
		private Vector2 _offsetPerX = Vector2.zero;

		[SerializeField]
		private Vector2 _offsetPerY = Vector2.zero;

		private Dictionary<Vector2Int, RectTransform> _cells = new ();
		public IReadOnlyDictionary<Vector2Int, RectTransform> cells => _cells;

		public bool Add(Vector2Int position,  RectTransform rectTransform)
		{
			if (_cells.ContainsKey(position))
			{
				Debug.LogError("Already exists");
				return false;
			}

			_cells.Add(position, rectTransform);

			UpdatePosition(position, rectTransform);

			return true;
		}

		private void UpdatePosition(Vector2Int position, RectTransform rectTransform)
		{
			rectTransform.anchoredPosition = new Vector2(_offsetForAll.x + position.x * _cellSize.x, _offsetForAll.y + position.y * _cellSize.y);
		}

		public void Remove(Vector2Int position)
		{
			if (_cells.ContainsKey(position))
			{
				_cells.Remove(position);
			}
		}

		public void Remove(RectTransform rectTransform)
		{
			var key = _cells.FirstOrDefault(x => x.Value == rectTransform).Key;
			if (key != null)
			{
				_cells.Remove(key);
			}
		}

		public void Clear()
		{
			_cells.Clear();
		}

		[ContextMenu("ToCenter")]
		public void ToCenter()
		{
			// _cellsのkeyの最大値と最小値を取得
			float minX = _cells.Keys.Min(x => x.x);
			float maxX = _cells.Keys.Max(x => x.x);
			float minY = _cells.Keys.Min(x => x.y);
			float maxY = _cells.Keys.Max(x => x.y);

			var center = new Vector2((minX + maxX) / 2, (minY + maxY) / 2);
			var offset = new Vector2(-center.x * _cellSize.x, -center.y * _cellSize.y);

			foreach (var cell in _cells)
			{
				var offsetPerX = new Vector2(_offsetPerX.x * cell.Key.x, _offsetPerX.y * cell.Key.x);
				var offsetPerY = new Vector2(_offsetPerY.x * cell.Key.y, _offsetPerY.y * cell.Key.y);
				cell.Value.anchoredPosition = new Vector2(
					offsetPerX.x + offsetPerY.x + _offsetForAll.x + offset.x + cell.Key.x * _cellSize.x,
					offsetPerX.y + offsetPerY.y + _offsetForAll.y + offset.y + cell.Key.y * _cellSize.y
				);
			}

		}
	}
}
