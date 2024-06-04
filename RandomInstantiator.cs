namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.Events;

	public class RandomInstantiator : MonoBehaviour
	{
		[SerializeField, Header("生成したいオブジェクトのプレハブ。複数指定可能")]
		private GameObject[] _prefabs;

		[SerializeField, Header("生成したプレハブを置く階層")]
		private Transform _parent;

		[SerializeField, Header("生成する数の乱数最小値")]
		private int _minAmount = 1;

		[SerializeField, Header("生成する数の乱数最大値")]
		private int _maxAmount = 10;

		[SerializeField, Header("生成する範囲")]
		private Bounds _bounds = new Bounds(Vector3.zero, Vector3.one * 10.0f);

		[SerializeField, Header("生成するオブジェクトの回転の乱数最小値")]
		private Vector3 _minRotation = Vector3.zero;

		[SerializeField, Header("生成するオブジェクトの回転の乱数最大値")]
		private Vector3 _maxRotation = Vector3.one * 360.0f;

		[SerializeField, Header("生成するオブジェクトのスケールの乱数最小値")]
		private Vector3 _minScale = Vector3.one * 0.5f;

		[SerializeField, Header("生成するオブジェクトのスケールの乱数最大値")]
		private Vector3 _maxScale = Vector3.one * 1.5f;

		[SerializeField, Header("手動で生成を実行する場合はチェックをつける")]
		private bool _manualInstantiate;

		[SerializeField, Header("他のプログラムに複製したものを渡したい場合に使う")]
		private UnityEvent<GameObject> _onInstantiate = new();
		public UnityEvent<GameObject> onInstantiate => _onInstantiate;

		private void Awake()
		{
			if (!_manualInstantiate)
			{
				InstantiateAll();
			}
		}

		[ContextMenu(nameof(InstantiateAll))]
		private void InstantiateAll()
		{
			for (int i = 0; i < Random.Range(_minAmount, _maxAmount); i++)
			{
				Instantiate();
			}
		}

		private void Instantiate()
		{
			var prefab = _prefabs[Random.Range(0, _prefabs.Length)];

			var instance = Instantiate(prefab, GetRandomPoint(), Quaternion.Euler(Random.Range(_minRotation.x, _maxRotation.x), Random.Range(_minRotation.y, _maxRotation.y), Random.Range(_minRotation.z, _maxRotation.z)), _parent);
			var scale = new Vector3(Random.Range(_minScale.x, _maxScale.x), Random.Range(_minScale.y, _maxScale.y), Random.Range(_minScale.z, _maxScale.z));
			instance.transform.localScale = scale;
			_onInstantiate?.Invoke(instance);
		}

		private Vector3 GetRandomPoint()
		{
			Vector3 center = _bounds.center;
			Vector3 size = _bounds.size;

			float x = Random.Range(center.x - size.x * 0.5f, center.x + size.x * 0.5f);
			float y = Random.Range(center.y - size.y * 0.5f, center.y + size.y * 0.5f);
			float z = Random.Range(center.z - size.z * 0.5f, center.z + size.z * 0.5f);

			return new Vector3(x, y, z);
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			DrawWireCube(_bounds, transform);
		}

		private static void DrawWireCube(Bounds bounds, Transform transform)
		{
			Vector3 localCenter = bounds.center;
			Vector3 localSize = bounds.size;

			// ローカル座標からワールド座標へ変換
			Vector3 worldCenter = transform.TransformPoint(localCenter);
			Vector3 worldSize = Vector3.Scale(localSize, transform.lossyScale);

			// 8つの頂点を計算
			Vector3 corner0 = transform.TransformPoint(localCenter + new Vector3(-worldSize.x, -worldSize.y, -worldSize.z) * 0.5f);
			Vector3 corner1 = transform.TransformPoint(localCenter + new Vector3(worldSize.x, -worldSize.y, -worldSize.z) * 0.5f);
			Vector3 corner2 = transform.TransformPoint(localCenter + new Vector3(worldSize.x, -worldSize.y, worldSize.z) * 0.5f);
			Vector3 corner3 = transform.TransformPoint(localCenter + new Vector3(-worldSize.x, -worldSize.y, worldSize.z) * 0.5f);
			Vector3 corner4 = transform.TransformPoint(localCenter + new Vector3(-worldSize.x, worldSize.y, -worldSize.z) * 0.5f);
			Vector3 corner5 = transform.TransformPoint(localCenter + new Vector3(worldSize.x, worldSize.y, -worldSize.z) * 0.5f);
			Vector3 corner6 = transform.TransformPoint(localCenter + new Vector3(worldSize.x, worldSize.y, worldSize.z) * 0.5f);
			Vector3 corner7 = transform.TransformPoint(localCenter + new Vector3(-worldSize.x, worldSize.y, worldSize.z) * 0.5f);

			// 線を描画
			Gizmos.DrawLine(corner0, corner1);
			Gizmos.DrawLine(corner1, corner2);
			Gizmos.DrawLine(corner2, corner3);
			Gizmos.DrawLine(corner3, corner0);

			Gizmos.DrawLine(corner4, corner5);
			Gizmos.DrawLine(corner5, corner6);
			Gizmos.DrawLine(corner6, corner7);
			Gizmos.DrawLine(corner7, corner4);

			Gizmos.DrawLine(corner0, corner4);
			Gizmos.DrawLine(corner1, corner5);
			Gizmos.DrawLine(corner2, corner6);
			Gizmos.DrawLine(corner3, corner7);
		}
	}
}