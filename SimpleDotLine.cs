namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class SimpleDotLine : MonoBehaviour
	{
		[SerializeField]
		private LineRenderer _lineRenderer;
		public LineRenderer lineRenderer => _lineRenderer;

		[SerializeField]
		private Vector2 _dotPerUnit = Vector2.right;

		[SerializeField]
		private Vector2 _speed = Vector2.left;

		private void Update()
		{
			var length = _lineRenderer.GetLineLength();

			var x = length / _dotPerUnit.x;
			var y = length / _dotPerUnit.y;

			_lineRenderer.material.mainTextureScale = new Vector2(x, y);
			_lineRenderer.material.mainTextureOffset = new Vector2(Time.time * _speed.x, Time.time * _speed.y);
		}
	}
}
