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
				if (_syncAnchorMin) _rectTransform.anchorMin = _target.anchorMin;
				if (_syncAnchorMax) _rectTransform.anchorMax = _target.anchorMax;
				if (_syncPivot) _rectTransform.pivot = _target.pivot;
				
				if (_syncSizeDelta)
				{
					var targetSizeDelta = _target.sizeDelta;
					var paddedSizeDelta = new Vector2(
						targetSizeDelta.x - _paddingLeft - _paddingRight,
						targetSizeDelta.y - _paddingTop - _paddingBottom
					);
					_rectTransform.sizeDelta = paddedSizeDelta;
				}
				
				if (_syncAnchoredPosition)
				{
					var basePosition = _target.anchoredPosition;
					
					// paddingによるサイズ変更に応じて位置を補正
					if (_syncSizeDelta)
					{
						var pivot = _rectTransform.pivot;
						var positionOffset = new Vector2(
							_paddingLeft - _paddingRight * (1f - pivot.x * 2f),
							_paddingBottom - _paddingTop * (1f - pivot.y * 2f)
						) * 0.5f;
						
						_rectTransform.anchoredPosition = basePosition + positionOffset;
					}
					else
					{
						_rectTransform.anchoredPosition = basePosition;
					}
				}
			}
		}
	}
}