namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class Tilter : MonoBehaviour
	{
		[Header("傾けたい確度")]
		public Vector3 tiltRotate = Vector3.right * 30;

		private void LateUpdate()
		{
			Tilt(transform, tiltRotate);
		}

		public static void Tilt(Transform transform, Vector3 tiltRotate)
		{
			transform.localRotation = Quaternion.identity;
			transform.Rotate(tiltRotate, Space.World);
		}
	}
}