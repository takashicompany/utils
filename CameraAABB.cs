namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class CameraAABB : MonoBehaviour
	{
		[SerializeField]
		private Camera _camera;

		private Plane[] _planes; 

		public void SetCamera(Camera camera)
		{
			_camera = camera;
		}

		private void Update()
		{
			if (_camera != null)
			{
				_planes = GeometryUtility.CalculateFrustumPlanes(_camera);
			}
		}

		public bool IsInBounds(Bounds bounds)
		{
			if (_planes == null)
			{
				return false;
			}

			return GeometryUtility.TestPlanesAABB(_planes, bounds);
		}
	}
}