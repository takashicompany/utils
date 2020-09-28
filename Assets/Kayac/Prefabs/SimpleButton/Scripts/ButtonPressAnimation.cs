namespace Kayac.UI
{
	using UnityEngine;
	using UnityEngine.EventSystems;
	using DG.Tweening;

	public class ButtonPressAnimation : MonoBehaviour,
		IPointerDownHandler,
		IPointerExitHandler,
		IPointerUpHandler
	{
		[SerializeField]
		private RectTransform _root = null;

		[SerializeField]
		private float _pressY = -15f;

		private Tweener _tweener;

		private Vector3 _baseAnchorPosition = Vector3.zero;

		private void Awake()
		{
			_baseAnchorPosition = _root.anchoredPosition;
		}

		private void OnEnable()
		{
			_root.anchoredPosition = _baseAnchorPosition;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			KillTween();
			_tweener = _root.DOAnchorPosY(_pressY, 0.05f).OnComplete(() =>
			{
				_tweener = null;
			});
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			Release();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			Release();
		}

		private void Release()
		{
			KillTween();

			_tweener = _root.DOAnchorPosY(0, 0.25f).SetEase(Ease.OutBounce).OnComplete(() =>
			{
				_tweener = null;
			});
		}

		private void KillTween()
		{
			if (_tweener != null && _tweener.IsPlaying())
			{
				_tweener.Kill();
			}

			_tweener = null;
		}
	}
}
