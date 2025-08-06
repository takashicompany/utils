namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using DG.Tweening;

	/// <summary>
	/// 指定したTransformの位置を覚えておく
	/// </summary>
	public class TransformKeeper : MonoBehaviour
	{
		[System.Serializable]
		protected class Record
		{
			[SerializeField]
			private Transform _transform;
			public Transform transform => _transform;

			[SerializeField]
			private PRS _prs;
			public PRS prs => _prs;

			public Record(Transform transform) : this(transform, transform.GetLocalPRS())
			{

			}

			public Record(Transform transform, PRS prs)
			{
				_transform = transform;
				_prs = prs;
			}
		}

		[SerializeField]
		protected Record[] _records;

		public IEnumerable<Transform> GetTransforms()
		{
			foreach(var r in _records)
			{
				yield return r.transform;
			}
		}

		[ContextMenu("子改装を保存")]
		private void SaveChildren()
		{
			var transforms = GetComponentsInChildren<Transform>();

			var records = new List<Record>();
			
			foreach (var t in transforms)
			{
				if (transform == t) continue;
				records.Add(new Record(t));
			}

			_records = records.ToArray();
		}

		public List<Tweener> Back(float duration, Ease ease = Ease.OutSine)
		{
			var tweeners = new List<Tweener>();
			
			foreach (var r in _records)
			{
				var t = r.transform.DOLocalPRS(r.prs, duration).SetEase(ease);
				tweeners.Add(t);
			}

		return tweeners;
		}
	}

	public class TransformParentKeeper
	{
		private Transform _transform;
		private Transform _parent;
		private int _index;

		public TransformParentKeeper(Transform transform)
		{
			_transform = transform;
			_parent = transform.parent;
			_index = transform.GetSiblingIndex();
		}

		public void Restore(bool withIndex = true)
		{
			_transform.SetParent(_parent);

			if (withIndex) _transform.SetSiblingIndex(_index);
		}
	}
}
