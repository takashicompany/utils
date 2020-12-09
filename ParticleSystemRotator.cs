namespace TakashiCompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class ParticleSystemRotator : MonoBehaviour
	{
		[SerializeField]
		private ParticleSystem[] _targets;
		
		private ParticleSystem _rootInternal;
		private ParticleSystem _root => _rootInternal ?? (_rootInternal = GetComponent<ParticleSystem>());

		public void Play(float angle, float range = 0)
		{
			foreach (var ps in _targets)
			{
				var main = ps.main;
				angle -= 70;	// この-70の意味が分からない...

				var halfRange = range / 2;
				var r = new ParticleSystem.MinMaxCurve((angle - halfRange) / 180f * Mathf.PI, (angle + halfRange) / 180f * Mathf.PI);
				main.startRotation = r;
			}

			_root.Play(true);
		}

		public void Play(Vector3 position, float angle, float range = 0)
		{
			transform.position = position;
			Play(angle, range);
		}
	}
}