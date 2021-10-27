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
		private Dictionary<Transform, Rigidbody> _rigidbodyDict = new Dictionary<Transform, Rigidbody>();

		private Dictionary<HumanBodyBones, bool> _boneActiveDict = new Dictionary<HumanBodyBones, bool>();
		
		public void Init(Animator origin, Animator copied)
		{
			_animator = copied;

			_boneDict.Clear();
			_rigidbodyDict.Clear();
			_boneActiveDict.Clear();

			foreach (HumanBodyBones boneName in System.Enum.GetValues(typeof(HumanBodyBones)))
			{
				var originBone = origin.GetBoneTransform(boneName);
				var copiedBone = origin.GetBoneTransform(boneName);

				if (originBone != copiedBone)
				{
					_boneActiveDict[boneName] = true;	// とりあえずtrue入れておくか
					_boneDict.Add(copiedBone, originBone);

					if (originBone.TryGetComponent<Rigidbody>(out var rigidbody))
					{
						_rigidbodyDict.Add(originBone, rigidbody);
					}
				}
			}
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

					// Rigidbodyがあればそちらを優先
					if (_rigidbodyDict.TryGetValue(originBone, out var rigidbody))
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
			}
		}


		public static ActiveRagdoll GenerateActiveRagdoll(Animator origin)
		{
			var copiedAnimator = Instantiate(origin, origin.transform.parent);

			var gameObjects = copiedAnimator.GetComponentsInChildren<GameObject>();

			foreach (var go in gameObjects)
			{
				if (go.TryGetComponent<Renderer>(out var renderer))
				{
					Destroy(renderer);
				}

				if (go.TryGetComponent<Rigidbody>(out var rigidbody))
				{
					Destroy(rigidbody);
				}

				if (go.TryGetComponent<Collider>(out var collider))
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