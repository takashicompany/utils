namespace takashicompany.Unity
{
	using UnityEngine;

	[ExecuteAlways]
	[RequireComponent(typeof(RectTransform))]
	public class RectTransformMatcher : MonoBehaviour
	{
		[SerializeField]
		private RectTransform _target;

		[SerializeField, EnumFlag]
		private UpdateType _updateType = UpdateType.LateUpdate;

		[Header("Padding Settings")]
		[SerializeField]
		private float _paddingLeft = 0f;

		[SerializeField]
		private float _paddingTop = 0f;

		[SerializeField]
		private float _paddingRight = 0f;

		[SerializeField]
		private float _paddingBottom = 0f;

		private RectTransform _rectTransform;

		private void Start()
		{
			_rectTransform = GetComponent<RectTransform>();
		}

		private void Update()
		{
			if (_updateType.IsUpdate()) Execute();
		}

		private void LateUpdate()
		{
			if (_updateType.IsLateUpdate()) Execute();
		}

		private void FixedUpdate()
		{
			if (_updateType.IsFixedUpdate()) Execute();
		}

		public void Execute()
		{
			if (_target != null && _rectTransform != null)
			{
				// ターゲットの四隅の座標を取得
				Vector3[] targetCorners = new Vector3[4];
				_target.GetWorldCorners(targetCorners);

				// 自身の親のRectTransformを取得
				RectTransform parentRectTransform = transform.parent as RectTransform;
				if (parentRectTransform == null)
				{
					return;
				}

				// ワールド座標をローカル座標に変換
				Vector2 minPos = parentRectTransform.InverseTransformPoint(targetCorners[0]);
				Vector2 maxPos = parentRectTransform.InverseTransformPoint(targetCorners[2]);

				// paddingを適用
				minPos.x += _paddingLeft;
				minPos.y += _paddingBottom;
				maxPos.x -= _paddingRight;
				maxPos.y -= _paddingTop;

				// アンカーを四隅に設定
				_rectTransform.anchorMin = Vector2.zero;
				_rectTransform.anchorMax = Vector2.one;

				// 位置とサイズを計算
				Vector2 size = maxPos - minPos;
				Vector2 center = (minPos + maxPos) * 0.5f;

				// offsetMinとoffsetMaxを設定
				_rectTransform.offsetMin = new Vector2(minPos.x, minPos.y);
				_rectTransform.offsetMax = new Vector2(maxPos.x - parentRectTransform.rect.width, maxPos.y - parentRectTransform.rect.height);
			}
		}
	}
}