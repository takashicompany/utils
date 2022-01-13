namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class RandomPlacer : MonoBehaviour
	{

		[System.Serializable]
		private class Param
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

			public List<Transform> Place()
			{
				var placed = new List<Transform>();

				var count = Random.Range(_min, _max);

				for (int i = 0; i < count; i++)
				{
					var prefab = _prefabs[Random.Range(0, _prefabs.Length)];

					var obj = Instantiate(prefab, _root);

					obj.name = prefab.name + "(" + i + ")";
					obj.transform.position = GetBounds().RandomPoint();

					placed.Add(obj);
				}
				
				return placed;
			}

			private Bounds GetBounds()
			{
				return _bounds.Transform(_root);
			}

			public void DrawGizmos()
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
		
		[SerializeField]
		private Param[] _paramList;

		private List<Transform> _placed = new List<Transform>();

		void Awake()
		{
			foreach (var p in _paramList)
			{
				_placed.AddRange(p.Place());
			}
		}

		// https://hwks.hatenadiary.jp/entry/2014/06/13/022747 のため
		void Start()
		{

		} 

		private void OnDrawGizmos()
		{
			foreach (var p in _paramList)
			{
				p.DrawGizmos();
			}
		}
	}
}
