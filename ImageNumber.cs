namespace TakashiCompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	public class ImageNumber : MonoBehaviour
	{
		[SerializeField]
		private ObjectBundle _bundle;

		[SerializeField]
		private RectTransform _root;

		[SerializeField]
		private Image _prefab;

		[SerializeField]
		private List<Image> _images = new List<Image>();
		
		[SerializeField]
		private string _text;

		private Dictionary<string, Sprite> _dictInternal;

		private Dictionary<string, Sprite> _dict => _dictInternal ?? (_dictInternal = _bundle.BuildDictionary());

		
		public void UpdateImages(string text)
		{
			_text = text;
		}

		[ContextMenu("Update Image")]
		public void UpdateImages()
		{
			Debug.Log(_dict.Count);
			for (int i = 0; i < _text.Length; i++)
			{
				var key = _text[i].ToString();

				if (_dict.TryGetValue(key, out var sprite))
				{
					var image = GetOrGenerateImage();
					image.sprite = sprite;
					image.SetNativeSize();
					image.transform.SetAsLastSibling();

					Debug.Log("gl");
				}

				Debug.Log(key);
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