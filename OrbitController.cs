namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using DG.Tweening;
	
	public class OrbitController : MonoBehaviour
	{
		private enum Axis
		{
			X = 0,
			Y,
			Z,
		}

		[SerializeField, Header("指定なければ同階層に生成する。回転をロックするのでこのGameObjectを指定する時は注意。")]
		private Transform _root;

		[SerializeField, Header("軌道オブジェクトの幅。")]
		private float _widthPerOrbit = 1f;

		[SerializeField, Header("最小の軌道半径")]
		private float _minOrbitDistance = 2f;

		[SerializeField, Header("回転する層が追加される毎に増加する半径")]
		private float _distancePerLayer = 1.5f;

		[SerializeField]
		private Axis _rotateAxis = Axis.Y;

		[SerializeField, Header("軌道オブジェクトの接近の周期の間隔。数字大きくすると間隔が短くなる。")]
		private float _wrapperBreathInterval = 2f;

		[SerializeField, Header("軌道オブジェクトの接近の大きさ")]
		private float _wrapperBreathRatio = 0.1f;

		[SerializeField, EnumFlag]
		private UpdateType _updateType = UpdateType.LateUpdate;

		[SerializeField]
		private float _rotationPerSecond = 360f;

		[SerializeField]
		private bool _isRotateReverseForLayer = true;

		private List<Transform> _rotators = new List<Transform>();
		private List<Transform> _orbits = new List<Transform>();
		private Dictionary<Transform, Transform> _orbitAndWrappers = new Dictionary<Transform, Transform>();
		private Dictionary<Transform, int> _wrapperAndLayers = new Dictionary<Transform, int>();

		private List<Chaser.Updater> _chaseUpdaters = new List<Chaser.Updater>();

		public float maxOrbit { get; private set; }

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

			if (_root != null)
			{
				_root.rotation = Quaternion.identity;
			}

			for (int i = 0; i < _rotators.Count; i++)
			{
				var rotator = _rotators[i];
				var v = Vector3.zero;
				v[(int)_rotateAxis] = _rotationPerSecond * Time.deltaTime;
				rotator.Rotate(_isRotateReverseForLayer && i % 2 == 1 ? -v : v);
				rotator.position = transform.position;
			}

			for (int i = _chaseUpdaters.Count - 1; i >= 0; i--)
			{
				var updater = _chaseUpdaters[i];
				if (updater.Update(Time.deltaTime))
				{
					_chaseUpdaters.RemoveAt(i);
				}
			}

			foreach (var wrapper in _orbitAndWrappers.Values)
			{
				var back = wrapper.forward * -1f;
				var sign = 1f; // wrapper.GetSiblingIndex() % 2 == 0 ? 1f : -1f;
				var byTime = Mathf.Sin(Time.time * _wrapperBreathInterval);
				wrapper.position =  wrapper.parent.position + back * byTime * _wrapperBreathRatio * sign;
			}
		}

		private Transform GetRotator(int index)
		{
			while (_rotators.Count <= index)
			{
				var rotator = new GameObject(name + " Rotator:" + index).transform;
				rotator.SetParent(_root != null ? _root : transform.parent);
				_rotators.Add(rotator);
			}

			var result = _rotators[index];
			result.transform.position = _rotators != null ? _root.position : transform.position;
			return result;
		}

		public void AddOrbit(Transform orbit, float duration = 0)
		{
			if (_orbitAndWrappers.ContainsKey(orbit)) return;

			// Debug.Log("A AddOrbit:" + orbit.name + " position:" + orbit.position);

			var useAnimation = duration > 0;
			
			var wrapper = new GameObject("OrbitWrapper:" + orbit.name).transform;
			wrapper.position = transform.position;

			wrapper.SetParent(_root != null ? _root : transform.parent);
			orbit.SetParent(wrapper);
			_orbitAndWrappers.Add(orbit, wrapper);

			// Debug.Log("B AddOrbit:" + orbit.name + " position:" + orbit.position);

			var index = _orbits.Count;

			var layer = GetLayer(index, out var maxCountOnLayer);

			Transform nearestOrbit = null;

			foreach (var o in _orbits)
			{
				var w = _orbitAndWrappers[o];
				
				if (_wrapperAndLayers[w] == layer)
				{
					if (nearestOrbit == null)
					{
						nearestOrbit = o;
					}
					else
					{
						var distance = Vector3.Distance(orbit.position, o.position);
						var nearestDistance = Vector3.Distance(orbit.position, nearestOrbit.position);

						if (distance < nearestDistance)
						{
							nearestOrbit = o;
						}
					}
				}
			}

			if (nearestOrbit == null)
			{
				_orbits.Add(orbit);
			}
			else
			{
				var nearestIndex = _orbits.IndexOf(nearestOrbit);
				_orbits.Insert(nearestIndex, orbit);
			}

			Align(duration);

			// Debug.Log("C AddOrbit:" + orbit.name + " position:" + orbit.position);

		}

		public void RemoveOrbit(Transform orbit)
		{
			if (!_orbitAndWrappers.TryGetValue(orbit, out var wrapper)) return;
			
			_orbits.Remove(orbit);
			_orbitAndWrappers.Remove(orbit);
			_wrapperAndLayers.Remove(wrapper);
			
			wrapper.DOKill();

			Destroy(wrapper.gameObject);
			Align(0.25f);
		}

		public void Align(float duration = 0)
		{
			_chaseUpdaters.Clear();

			var layer = 0;

			int index = 0;

			var useAnimation = duration > 0;

			maxOrbit = 0;

			while (index < _orbits.Count)
			{
				var 半径 = _minOrbitDistance + _distancePerLayer * layer;
				var 円周 = Mathf.PI * 2f * 半径;
				var count = (int)(円周 / _widthPerOrbit) - 1;
				var anglePerOne = 360f / Mathf.Min(count, _orbits.Count - index);

				var rotator = GetRotator(layer);

				for (int i = index; i < Mathf.Min(index + count, _orbits.Count); i++)
				{
					var orbit = _orbits[i];
					var wrapper = _orbitAndWrappers[orbit];
					
					wrapper.SetParent(rotator);
					_wrapperAndLayers.AddOrSet(wrapper, layer);

					var v = Vector3.zero;
					v[(int)_rotateAxis] = anglePerOne * i;
					wrapper.localPosition = Vector3.zero;

					var rot = Quaternion.Euler(v);
					var localPosition = Vector3.forward * (_minOrbitDistance + _distancePerLayer * layer);

					if (useAnimation)
					{
						if (wrapper.gameObject != null)
						{
							wrapper.DOKill();
							wrapper.DOLocalRotateQuaternion(rot, duration);
						}

						var chaser = new Chaser.Updater(orbit, wrapper, localPosition, 100);
						_chaseUpdaters.Add(chaser);
					}
					else
					{
						wrapper.localRotation = rot;
						orbit.localPosition = localPosition;
					}
				}

				index += count;
				layer++;

				maxOrbit = Mathf.Max(半径, maxOrbit);
			}
		}

		public int GetLayer(int index, out int maxCountOnLayer)
		{
			var depth = 0;

			do
			{
				var 円周 = Mathf.PI * 2f * (_minOrbitDistance + _distancePerLayer * depth);
				
				maxCountOnLayer = (int)(円周 / _widthPerOrbit) - 1;
				
				index -= maxCountOnLayer;
				
				if (index > 0) depth++;
			} while (index > 0);

			return depth;
		}
	}
}