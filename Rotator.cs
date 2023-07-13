namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class Rotator : MonoBehaviour
	{
		[SerializeField]
		private Vector3 _rotatePerSecond = Vector3.forward * 360;

		private void Update()
		{
			transform.Rotate(_rotatePerSecond * Time.deltaTime);
		}
	}
}