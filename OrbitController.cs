namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	
	public class OrbitController : MonoBehaviour
	{
		private enum Axis
		{
			X = 0,
			Y,
			Z,
		}

		[SerializeField]
		private Axis _rotateAxis = Axis.Y;

		[SerializeField, EnumFlag]
		private UpdateType _updateType = UpdateType.LateUpdate;

		[SerializeField]
		private float _rotationPerSecond = 360f;

		private List<Transform> _orbits = new List<Transform>();
		private Dictionary<Transform, Transform> _orbitAndWrappers = new Dictionary<Transform, Transform>();

		private void Update()
		{
			if (_updateType.HasFlag(UpdateType.Update))Orbit();
		}

		private void LateUpdate()
		{
			if (_updateType.HasFlag(UpdateType.LateUpdate))Orbit();
		}

		private void FixedUpdate()
		{
			if (_updateType.HasFlag(UpdateType.FixedUpdate))Orbit();
		}

		private void Orbit()
		{
			foreach (var orbit in _orbits)
			{

				if (!_orbitAndWrappers.TryGetValue(orbit, out var wrapper)) continue;
				
				var v = Vector3.zero;
				v[(int)_rotateAxis] = _rotationPerSecond * Time.deltaTime;
				wrapper.Rotate(v);

				wrapper.position = transform.position;
			}
		}

		public void AddOrbit(Transform orbit, float distance)
		{
			if (_orbitAndWrappers.ContainsKey(orbit)) return;
			
			_orbits.Add(orbit);
			var wrapper = new GameObject("OrbitWrapper:" + orbit.name).transform;
			wrapper.SetParent(transform.parent);
			orbit.SetParent(wrapper);
			orbit.localPosition = Vector3.forward * distance;
			_orbitAndWrappers.Add(orbit, wrapper);

			Align();
		}

		public void RemoveOrbit(Transform orbit)
		{
			if (!_orbitAndWrappers.ContainsKey(orbit)) return;
			
			_orbits.Remove(orbit);
			Destroy(_orbitAndWrappers[orbit].gameObject);
			_orbitAndWrappers.Remove(orbit);

			Align();
		}

		public void Align()
		{
			var anglePerOne = 360f / _orbits.Count;

			for (int i = 0; i < _orbits.Count; i++)
			{
				var orbit = _orbits[i];
				var wrapper = _orbitAndWrappers[orbit];
				var v = Vector3.zero;
				v[(int)_rotateAxis] = anglePerOne * i;
				wrapper.transform.position = transform.position;
				wrapper.localRotation = Quaternion.Euler(v);
			}
		}
	}
}