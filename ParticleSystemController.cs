namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class ParticleSystemController : MonoBehaviour
	{
		[Header("PlayOnAwakeは無効にしておいてください。")]
		[SerializeField, Header("メインのパーティクル。大抵はパーティクルのルートになるはず。")]
		private ParticleSystem _mainParticleSystem;

		public ParticleSystem mainParticleSystem => _mainParticleSystem;

		[Header("PlayOnAwakeは無効にしておいてください。")]
		[SerializeField, Header("色を変えたいParticle")]
		private ParticleSystem[] _particleSystems;

		[ContextMenu("子コンポーネントからPSを探索")]
		private void FindParticleSystemsInChildren()
		{
			_particleSystems = GetComponentsInChildren<ParticleSystem>();
		}

		public void SetMaterialColor(Color color)
		{
			foreach (var p in _particleSystems)
			{
				if (p.gameObject.TryGetComponent<Renderer>(out var r))
				{
					r.material.color = color;
				}
			}
		}

		public void SetStartColor(Color color)
		{
			foreach (var p in _particleSystems)
			{
				var main = p.main;
				main.startColor = color;
			}
		}

		/// <summary>
		/// メインのパーティクルを再生する
		/// </summary>
		public void Play()
		{
			_mainParticleSystem.Play(true);
		}
	}
}