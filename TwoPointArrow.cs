namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class TwoPointArrow : MonoBehaviour
	{
		[SerializeField]
		private Transform _lookTransform;

		[SerializeField]
		private Renderer _renderer;

		[SerializeField, Header("負数を選択するとアニメーション方向を逆にできる")]
		private float _animationSpeed = 1f;

		[SerializeField, Header("Run On UpdateかRun On LateUpdateが有効な時に指定される")]
		private Vector3 _to;

		[SerializeField]
		private bool _runOnUpdate;

		[SerializeField]
		private bool _runOnLateUpdate;

		private void Update()
		{
			if (_runOnUpdate) Look();
		}

		private void LateUpdate()
		{
			if (_runOnLateUpdate) Look();
		}

		public void Look(Vector3 from, Vector3 to)
		{
			transform.position = from;
			_to = to;
			Look();
		}

		private void Look()
		{
			transform.LookAt(_to);

			var distance = Vector3.Distance(_lookTransform.position, _to);

			var scale = _lookTransform.localScale;

			scale.z = distance;

			_lookTransform.localScale = scale;

			// TODO X方向だけにしているの、どうにかする
			_renderer.material.mainTextureScale = new Vector2(distance, _renderer.material.mainTextureScale.y);
			_renderer.material.mainTextureOffset = new Vector2(Time.time * _animationSpeed, _renderer.material.mainTextureOffset.y);
		}
	}
}