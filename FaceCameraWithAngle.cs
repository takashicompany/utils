namespace takashicompany.Unity
{
	using UnityEngine;

	public class FaceCameraWithAngle : MonoBehaviour
	{
		[SerializeField]
		private Vector3 _angleOffset; // インスペクタから設定する角度のオフセット
		
		[SerializeField]
		private Camera _camera;

		void Start()
		{
			if (_camera == null)
			{
				_camera = Camera.main;
			}
		}

		void LateUpdate()
		{
			// カメラが存在するかを確認
			if (_camera == null) return;

			// Canvasをカメラに向けて、オフセット分の角度を追加する
			Quaternion targetRotation = _camera.transform.rotation * Quaternion.Euler(_angleOffset);
			transform.rotation = targetRotation;
		}
	}
}