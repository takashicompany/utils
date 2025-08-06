namespace takashicompany.Unity
{
	using UnityEngine;
	using UnityEngine.UI;

	[RequireComponent(typeof(Image))]
	public class UIScrollTexture : MonoBehaviour
	{
		private Image _image;
		
		public Vector2 scrollSpeed = new Vector2(0.1f, 0); // X方向にスクロール

		private Material _material;
		private Vector2 _offset;

		private void Awake()
		{
			_image = GetComponent<Image>();
		}

		private void Start()
		{
			// Imageのマテリアルを複製（変更を他のUIに影響させないため）
			_material = Instantiate(_image.material);
			_image.material = _material;
			_offset = _material.mainTextureOffset;
		}

		private void Update()
		{
			// UVスクロール
			_offset += scrollSpeed * Time.deltaTime;
			_material.mainTextureOffset = _offset;
		}
	}

}
