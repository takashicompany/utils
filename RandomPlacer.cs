namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class RandomPlacer : MonoBehaviour
	{
		[SerializeField]
		private int _min = 10;

		[SerializeField]
		private int _max = 20;
		
		[SerializeField]
		private Bounds _bounds = new Bounds(Vector3.zero, Vector3.one);
		
		[SerializeField]
		private Transform[] _prefabs;

		[SerializeField]
		private Transform _root;

		private List<Transform> _placed = new List<Transform>();

		void Awake()
		{
			var count = Random.Range(_min, _max);

			for (int i = 0; i < count; i++)
			{
				var prefab = _prefabs[Random.Range(0, _prefabs.Length)];

				var obj = Instantiate(prefab, _root);

				obj.transform.position = GetBounds().RandomPoint();

				_placed.Add(obj);
			}
		}

		private Bounds GetBounds()
		{
			return _bounds.Transform(_root);
		}

		private void OnDrawGizmos()
		{
			if (_root == null)
			{
				return;
			}

			Gizmos.color = Color.red;
			
			var b = GetBounds();

			Gizmos.DrawWireCube(b.center, b.size);
		}
	}
}
