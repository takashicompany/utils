namespace takashicompany.Unity
{
	using UnityEngine;

	[ExecuteAlways]
	[RequireComponent(typeof(RectTransform))]
	public class RectTransformSynchronizer : MonoBehaviour
	{
		[SerializeField]
		private RectTransform _target;

		[SerializeField, EnumFlag]
		private UpdateType _updateType = UpdateType.Update;

		[SerializeField]
		private bool _syncAnchoredPosition = true;

		[SerializeField]
		private bool _syncSizeDelta = true;

		[SerializeField]
		private bool _syncAnchorMin = true;

		[SerializeField]
		private bool _syncAnchorMax = true;

		[SerializeField]
		private bool _syncPivot = true;

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
			// このスクリプトがアタッチされているオブジェクトのRectTransformを取得
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
				// 参照元のRectTransformからサイズと位置をコピー
				if (_syncAnchoredPosition) _rectTransform.anchoredPosition = _target.anchoredPosition;
				
				if (_syncSizeDelta)
				{
					var targetSizeDelta = _target.sizeDelta;
					var paddedSizeDelta = new Vector2(
						targetSizeDelta.x - _paddingLeft - _paddingRight,
						targetSizeDelta.y - _paddingTop - _paddingBottom
					);
					_rectTransform.sizeDelta = paddedSizeDelta;
				}
				
				if (_syncAnchorMin) _rectTransform.anchorMin = _target.anchorMin;
				if (_syncAnchorMax) _rectTransform.anchorMax = _target.anchorMax;
				if (_syncPivot) _rectTransform.pivot = _target.pivot;
			}
		}
	}
}