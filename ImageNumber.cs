namespace TakashiCompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	public class ImageNumber : MonoBehaviour
	{
		[SerializeField]
		private CharSpriteBundle _bundle;

		[SerializeField]
		private RectTransform _root;

		[SerializeField]
		private Image _prefab;

		[SerializeField]
		private List<Image> _images = new List<Image>();
		
		[SerializeField]
		private string _text;

		[SerializeField]
		private Vector2 _offsetSizeRatio = Vector2.one;

		private Dictionary<char, Sprite> _dictInternal;

		private Dictionary<char, Sprite> _dict => _dictInternal ?? (_dictInternal = _bundle.BuildDictionary());

		public void UpdateImages(string text)
		{
			_text = text;
			UpdateImages();
		}

		[ContextMenu("Update Image")]
		public void UpdateImages()
		{
			CollectAll();
			for (int i = 0; i < _text.Length; i++)
			{
				var key = _text[i];

				if (_dict.TryGetValue(key, out var sprite))
				{
					var image = GetOrGenerateImage();
					image.sprite = sprite;
					image.SetNativeSize();
					image.rectTransform.sizeDelta *= _offsetSizeRatio;
					image.transform.SetAsLastSibling();
				}
			}
		}

		private Image GetOrGenerateImage()
		{
			var image = GetUnusedImage();

			if (image == null)
			{
				image = Instantiate(_prefab, _root);
				_images.Add(image);
			}

			image.gameObject.SetActive(true);

			return image;
		}

		private Image GetUnusedImage()
		{
			return _images.Find(m => !m.gameObject.activeSelf);
		}

		private void CollectAll()
		{
			foreach (var image in _images)
			{
				image.gameObject.SetActive(false);
			}
		}
	}
}