namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class Heaper : MonoBehaviour
	{
		[Header("オブジェクトを山盛りにするコンポーネント")]
		[SerializeField, Header("山盛りにしたいオブジェクト。Projectビューのプレハブを指定してください。")]
		private GameObject _object;

		[SerializeField, Header("未指定ならこのコンポーネント以下に配置する")]
		private Transform _headpedRoot;

		[SerializeField, Header("山盛りにしたいオブジェクトの原点の範囲")]
		private Bounds _heapBounds = new Bounds(Vector3.zero, Vector3.one);

		[SerializeField]
		private AnimationCurve _heapBoundsCurve = AnimationCurve.Linear(0, 1, 1, 0);

		[SerializeField]
		private Vector3 _randomRotationMin = new Vector3(0, -180, 0);

		[SerializeField]
		private Vector3 _randomRotationMax = new Vector3(0, 180, 0);

		[SerializeField]
		private AnimationCurve _rotationCurve = AnimationCurve.Linear(0, 1, 1, 0);


		[SerializeField]
		private int _amount = 1;

		[SerializeField]
		private List<GameObject> _heaped = new List<GameObject>();

		[ContextMenu("Heap")]
		public void Heap()
		{
			Heap(_object, _amount);
		}
		
		public void Heap(GameObject obj, int amount)
		{
			if (_heaped != null)
			{
				foreach(var h in _heaped)
				{
					if (h == null)
					{
						continue;
					}

					DestroyImmediate(h);
				}
			}

			_heaped = new List<GameObject>();

			var bounds = GetBounds();
			var root = GetHeapedRoot();

			for (int i = 0; i < amount; i++)
			{
				GameObject go = null;
				
#if UNITY_EDITOR
				if (!Application.isPlaying)
				{
					go = UnityEditor.PrefabUtility.InstantiatePrefab(obj, root) as GameObject;
				}
#endif
				if (go == null)
				{
					go = Instantiate(obj, root);
				}

				if (go == null)
				{
					Debug.LogError("トラブルが発生しました。");
					continue;
				}

				var normalized = Random.Range(0f, 1f);

				var y = Mathf.Lerp(_heapBounds.min.y, _heapBounds.max.y, normalized);

				var posXZ = GetBoundsByCurve(normalized).RandomPoint();

				var p = new Vector3(posXZ.x, y, posXZ.z);

				go.transform.position = p;
				go.transform.rotation = GetRotationByCurve(normalized);


				_heaped.Add(go);

			}

			Bounds GetBoundsByCurve(float normalized)
			{
				normalized = Mathf.Clamp01(normalized);
				var ratio = _heapBoundsCurve.Evaluate(normalized);
				var size = new Vector3(bounds.size.x * ratio, 0f, bounds.size.z * ratio);

				return new Bounds(bounds.center, size);
			}

			Quaternion GetRotationByCurve(float normalized)
			{
				normalized = Mathf.Clamp01(normalized);
				var ratio = _heapBoundsCurve.Evaluate(normalized);

				var rot = Utils.RandomRotation(_randomRotationMin * ratio, _randomRotationMax * ratio);

				return rot;
			}

			
		}

		private Transform GetHeapedRoot()
		{
			return _headpedRoot == null ? transform : _headpedRoot;
		}

		private Bounds GetBounds()
		{
			return _heapBounds.Transform(GetHeapedRoot());
		}
		
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			var b = GetBounds();
			Gizmos.DrawWireCube(b.center, b.size);
		}
	}
}