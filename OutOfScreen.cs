namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class OutOfScreen : MonoBehaviour
	{
		private Renderer _rendererInternal;

		public Renderer _renderer => _rendererInternal ?? (_rendererInternal = GetComponent<Renderer>());

		private CameraAABB _cameraAABB;

		public void SetCamera(CameraAABB cameraAABB)
		{
			_cameraAABB = cameraAABB;
		}

		private void Update()
		{
			if (_cameraAABB != null)
			{
				_renderer.enabled = _cameraAABB.IsInBounds(_renderer.bounds);
			}
		}
	}
}