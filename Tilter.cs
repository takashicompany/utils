namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class Tilter : MonoBehaviour
	{
		[SerializeField, Header("傾けたい確度")]
		private Vector3 _tiltRotate;

		private void LateUpdate()
		{
			transform.localRotation = Quaternion.identity;
			transform.Rotate(_tiltRotate, Space.World);
		}
	}
}