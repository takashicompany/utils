namespace TakashiCompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class ActiveRagdoll : MonoBehaviour
	{
		private Animator _animator;

		public Animator animator => _animator;

		private Dictionary<Transform, Transform> _boneDict = new Dictionary<Transform, Transform>();
		private Dictionary<HumanBodyBones, Rigidbody> _rigidbodyDict = new Dictionary<HumanBodyBones, Rigidbody>();

		private Dictionary<HumanBodyBones, bool> _boneActiveDict = new Dictionary<HumanBodyBones, bool>();

		public HumanBodyBones targetBone;

		[ContextMenu("Test")]
		private void Test()
		{
			SetBoneActive(targetBone, false);
		}
		
		public void Init(Animator origin, Animator copied)
		{
			_animator = copied;

			_boneDict.Clear();
			_rigidbodyDict.Clear();
			_boneActiveDict.Clear();

			foreach (HumanBodyBones boneName in System.Enum.GetValues(typeof(HumanBodyBones)))
			{
				if (boneName == HumanBodyBones.LastBone)
				{
					continue;
				}

				var originBone = origin.GetBoneTransform(boneName);
				var copiedBone = copied.GetBoneTransform(boneName);

				if (originBone != null && copiedBone != null)
				{
					_boneDict.Add(copiedBone, originBone);
					_boneActiveDict.Add(boneName, true);

					if (originBone.TryGetComponent<Rigidbody>(out var rigidbody))
					{
						_rigidbodyDict.Add(boneName, rigidbody);
					}

					SetBoneActive(boneName, true);
				}
			}

			origin.enabled = false;

			// var colliders = origin.GetComponentsInChildren<Collider>();

			// foreach (var c in colliders)
			// {
			// 	foreach (var a in colliders)
			// 	{
			// 		Physics.IgnoreCollision(c, a, true);
			// 	}
			// }
		}

		private void LateUpdate()
		{
			foreach (var kvp in _boneActiveDict)
			{
				var boneName = kvp.Key;
				var active = kvp.Value;

				if (active)
				{
					var copiedBone = _animator.GetBoneTransform(boneName);
					var originBone = _boneDict[copiedBone];

					/*
					// Rigidbodyがあればそちらを優先
					if (_rigidbodyDict.TryGetValue(boneName, out var rigidbody))
					{
						rigidbody.position = copiedBone.position;
						rigidbody.rotation = copiedBone.rotation;
					}
					else
					{
						originBone.position = copiedBone.position;
						originBone.rotation = copiedBone.rotation;
					}

					originBone.localScale = copiedBone.localScale;
					*/
					

					// 回転だけにしてみる
					originBone.localRotation = copiedBone.localRotation;
				}
			}
		}

		public void SetAllBoneActive(bool active)
		{
			foreach (var kvp in _boneActiveDict)
			{
				var boneName = kvp.Key;
				SetBoneActive(boneName, active);
			}
		}

		public void SetBoneActive(HumanBodyBones bone, bool active)
		{
			if (_boneActiveDict.ContainsKey(bone))
			{
				_boneActiveDict[bone] = active;

				if (_rigidbodyDict.TryGetValue(bone, out var rigidbody))
				{
					rigidbody.isKinematic = active;
				}
			}
		}

		public static ActiveRagdoll GenerateActiveRagdoll(Animator origin)
		{
			var copiedAnimator = Instantiate(origin, origin.transform.parent);

			var transforms = copiedAnimator.GetComponentsInChildren<Transform>();

			foreach (var t in transforms)
			{
				if (t.TryGetComponent<Renderer>(out var renderer))
				{
					Destroy(renderer);
				}

				if (t.TryGetComponent<Joint>(out var joint))
				{
					Destroy(joint);
				}

				if (t.TryGetComponent<Rigidbody>(out var rigidbody))
				{
					Destroy(rigidbody);
				}

				if (t.TryGetComponent<Collider>(out var collider))
				{
					Destroy(collider);
				}
			}

			var activeRagdoll = copiedAnimator.gameObject.AddComponent<ActiveRagdoll>();
			activeRagdoll.Init(origin, copiedAnimator);

			return activeRagdoll;
		}
	}
}