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
		private float _widthPerOrbit = 1f;

		[SerializeField]
		private float _minOrbitDistance = 2f;

		[SerializeField]
		private float _distancePerDepth = 1.25f;

		[SerializeField]
		private Axis _rotateAxis = Axis.Y;

		[SerializeField, EnumFlag]
		private UpdateType _updateType = UpdateType.LateUpdate;

		[SerializeField]
		private float _rotationPerSecond = 360f;

		[SerializeField]
		private bool _isRotateReverseForDepth = true;

		private List<Transform> _rotators = new List<Transform>();
		private List<Transform> _orbits = new List<Transform>();
		private Dictionary<Transform, Transform> _orbitAndWrappers = new Dictionary<Transform, Transform>();

		private void OnDestroy()
		{
			foreach (var wrapper in _orbitAndWrappers.Values)
			{
				Destroy(wrapper.gameObject);
			}

			foreach (var rotator in _rotators)
			{
				Destroy(rotator.gameObject);
			}
		}

		private void Update()
		{
			if (_updateType.HasFlag(UpdateType.Update)) Rotate();
		}

		private void LateUpdate()
		{
			if (_updateType.HasFlag(UpdateType.LateUpdate)) Rotate();
		}

		private void FixedUpdate()
		{
			if (_updateType.HasFlag(UpdateType.FixedUpdate)) Rotate();
		}

		private void Rotate()
		{
			// foreach (var orbit in _orbits)
			// {

			// 	if (!_orbitAndWrappers.TryGetValue(orbit, out var wrapper)) continue;
				
			// 	var v = Vector3.zero;
			// 	v[(int)_rotateAxis] = _rotationPerSecond * Time.deltaTime;
			// 	wrapper.Rotate(v);

			// 	wrapper.position = transform.position;
			// }

			for (int i = 0; i < _rotators.Count; i++)
			{
				var rotator = _rotators[i];
				var v = Vector3.zero;
				v[(int)_rotateAxis] = _rotationPerSecond * Time.deltaTime;
				rotator.Rotate(_isRotateReverseForDepth && i % 2 == 1 ? -v : v);
				rotator.position = transform.position;
			}
		}

		private Transform GetRotator(int index)
		{
			while (_rotators.Count <= index)
			{
				var rotator = new GameObject(name + " Rotator:" + index).transform;
				rotator.SetParent(transform.parent);
				_rotators.Add(rotator);
			}

			return _rotators[index];
		}

		public void AddOrbit(Transform orbit)
		{
			if (_orbitAndWrappers.ContainsKey(orbit)) return;
			
			_orbits.Add(orbit);
			var wrapper = new GameObject("OrbitWrapper:" + orbit.name).transform;
			wrapper.SetParent(transform.parent);
			orbit.SetParent(wrapper);
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
			var depth = 0;

			int index = 0;

			while (index < _orbits.Count)
			{
				var 円周 = Mathf.PI * 2f * (_minOrbitDistance + _widthPerOrbit * depth);
				var count = (int)(円周 / _widthPerOrbit) - 1;
				var anglePerOne = 360f / Mathf.Min(count, _orbits.Count - index);

				var rotator = GetRotator(depth);

				for (int i = index; i < Mathf.Min(index + count, _orbits.Count); i++)
				{
					var orbit = _orbits[i];
					var wrapper = _orbitAndWrappers[orbit];
					wrapper.SetParent(rotator);
					var v = Vector3.zero;
					v[(int)_rotateAxis] = anglePerOne * i;
					wrapper.localPosition = Vector3.zero;
					wrapper.localRotation = Quaternion.Euler(v);
					orbit.localPosition = Vector3.forward * (_minOrbitDistance + _widthPerOrbit * depth);
				}

				index += count;
				depth++;
			}

			// var anglePerOne = 360f / _orbits.Count;

			// for (int i = 0; i < _orbits.Count; i++)
			// {
			// 	var orbit = _orbits[i];
			// 	var wrapper = _orbitAndWrappers[orbit];
			// 	var v = Vector3.zero;
			// 	v[(int)_rotateAxis] = anglePerOne * i;
			// 	wrapper.transform.position = transform.position;
			// 	wrapper.localRotation = Quaternion.Euler(v);
			// }
		}
	}
}