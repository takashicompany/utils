namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	/// <summary>
	/// Spriteを分割してSpriteRendererに割り当てる
	/// </summary>
	public class SpriteSeparator : MonoBehaviour
	{
		[SerializeField, Header("分割するSprite")]
		private Sprite _sprite;

		[SerializeField, Header("分割数")]
		private Vector2Int _separates = Vector2Int.one * 2;

		[SerializeField, Header("1グリッドあたりの大きさ")]
		private Vector2 _unitPerGrid = new Vector2(1, 1);

		[SerializeField, Header("分割したSpriteを割り当てるSpriteRendererのプレハブ")]
		private SpriteRenderer _prefab;

		[SerializeField, Header("生成されたSpriteRenderer")]
		private SpriteRenderer[] _renderers;


		[ContextMenu(nameof(Separate))]
		private void Separate()
		{
			if (_renderers != null)
			{
				foreach (var r in _renderers)
				{
					if (r == null) continue;
					DestroyImmediate(r.gameObject);
				}
			}

			_renderers = Separate(_sprite, _separates, _unitPerGrid, _prefab, transform);
		}

		public static SpriteRenderer[] Separate(Sprite _sprite, Vector2Int _separates, Vector2 _unitPerGrid, SpriteRenderer _prefab, Transform parent)
		{
			var texture = _sprite.texture;

			var width = texture.width / _separates.x;
			var height = texture.height / _separates.y;
			var pivot = new Vector2(0.5f, 0.5f);

			var renderers = new List<SpriteRenderer>();

			for (int x = 0; x < _separates.x; x++)
			{
				for (int y = 0; y < _separates.y; y++)
				{
					var sprite = Sprite.Create(texture, new Rect(x * width, y * height, width, height), pivot, _sprite.pixelsPerUnit / Mathf.Max(_separates.x, _separates.y));
					var p = parent.TransformPoint(Utils.GetPositionByCell(_separates, new Vector2Int(x, y), _unitPerGrid));

					var s = Utils.InstantiateWithPrefabReference(_prefab, parent);
					s.sprite = sprite;
					s.transform.position = p;

					renderers.Add(s);
				}
			}
			
			return renderers.ToArray();
		}
	}
}