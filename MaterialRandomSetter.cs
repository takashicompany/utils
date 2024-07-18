namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class MaterialRandomSetter : MonoBehaviour
	{
		[System.Serializable]
		private class MaterialList
		{
			[SerializeField, Header("ランダムで選択されるマテリアル。変えたくない場合は空にする。")]
			private Material[] _materials;
			public Material[] materials => _materials;

			public bool HasMaterials()
			{
				return _materials != null && _materials.Length > 0;
			}

			public bool TryGetMaterial(out Material material)
			{
				if (HasMaterials())
				{
					material = _materials[Random.Range(0, _materials.Length)];
					return true;
				}

				material = null;
				return false;
			}
		}

		[SerializeField, Header("マテリアルを変えたい対象。未指定の場合はこのGameObjectのRendererが対象になる。")]
		private Renderer _renderer;

		[SerializeField, Header("順番は、Renderer.Materialsに対応している。")]
		private MaterialList[] _materialLists;

		private void Start()
		{
			if (_renderer == null)
			{
				_renderer = GetComponent<Renderer>();
			}

			if (_renderer == null)
			{
				Debug.LogError("Rendererが見つかりませんでした。");
				return;
			}

			if (_materialLists == null || _materialLists.Length == 0)
			{
				Debug.LogError("MaterialListが見つかりませんでした。");
				return;
			}

			if (_renderer.materials.Length != _materialLists.Length)
			{
				Debug.LogError("RendererのMaterialsの数とMaterialListの数が一致しません。");
				return;
			}

			var materials = _renderer.materials;

			for (int i = 0; i < materials.Length; i++)
			{
				if (_materialLists[i].TryGetMaterial(out var material))
				{
					Debug.Log(material.name);
					materials[i] = material;
				}
			}

			_renderer.materials = materials;
		}
	}
}
