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

		public enum RotateType
		{
			Local,
			World
		}

		public RotateType rotateType;

		private void LateUpdate()
		{
			Tilt(transform, rotateType, tiltRotate);
		}

		public static void Tilt(Transform transform, RotateType rotateType, Vector3 tiltRotate)
		{
			switch (rotateType)
			{
				case RotateType.Local:
					transform.localRotation = Quaternion.identity;
					transform.Rotate(tiltRotate, Space.World);
					break;

				case RotateType.World:
					transform.rotation = Quaternion.Euler(tiltRotate);
					break;

			}
		}
	}
}