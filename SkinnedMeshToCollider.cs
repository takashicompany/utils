namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class SkinnedMeshToCollider : MonoBehaviour
	{
		[SerializeField]
		private SkinnedMeshRenderer _renderer;

		[SerializeField]
		private MeshCollider _collider;
		public new MeshCollider collider => _collider;

		private Mesh _mesh;

		void Awake()
		{
			if (_renderer == null)
			{
				_renderer = GetComponentInChildren<SkinnedMeshRenderer>();

				if (_renderer == null)
				{
					Debug.LogError($"{nameof(SkinnedMeshRenderer)}が見つかりません。");
					this.enabled = false;
					return;
				}
			}

			if (_collider == null)
			{
				_collider = GetComponentInChildren<MeshCollider>();

				if (_collider == null)
				{
					_collider = gameObject.AddComponent<MeshCollider>();
				}
			}

			_mesh = new Mesh();
		}

		private int _frameCount = -1;

		public void LateUpdate()
		{
			if (_frameCount >= 0 && _frameCount + 2 < Time.frameCount)
			{
				_frameCount = -1;
				UpdateCollider();
			}
		}

		public void RequestUpdateCollider()
		{
			_frameCount = Time.frameCount;
		}

		private void UpdateCollider()
		{
			_renderer.BakeMesh(_mesh);
			Debug.Log("Collider Updated");
			_collider.sharedMesh = null;	// おまじないだってサ
			_collider.sharedMesh = _mesh;
		}
	}
}
