namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class CenterSetter : MonoBehaviour
	{
		[SerializeField]
		private Transform _targetRoot;

		[ContextMenu(nameof(Place))]
		private void Place()
		{
			var renderers = _targetRoot.GetComponentsInChildren<Renderer>();

			var bounds = new Bounds();

			foreach (var renderer in renderers)
			{
				bounds.Encapsulate(renderer.bounds);
			}

			transform.position = bounds.center;
		}
	}
}