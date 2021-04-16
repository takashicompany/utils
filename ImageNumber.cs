namespace TakashiCompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	public class ImageNumber : MonoBehaviour
	{
		[SerializeField]
		private RectTransform _root;

		[SerializeField]
		private Image _prefab;

		[SerializeField]
		private List<Image> _images = new List<Image>();
		
		[SerializeField]
		private string _text;
		
		public void UpdateImages(string text)
		{
			_text = text;
		}

		public void UpdateImages()
		{
			for (int i = 0; i < _text.Length; i++)
			{
				
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

			return image;
		}

		private Image GetUnusedImage()
		{
			return _images.Find(m => m.gameObject.activeSelf);
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