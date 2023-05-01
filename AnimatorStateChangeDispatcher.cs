namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[RequireComponent(typeof(Animator))]
	public class AnimatorStateChangeDispatcher : MonoBehaviour
	{
		public bool checkOnUpdate = false;

		public bool checkOnLateUpdate = true;

		private Animator _animator;

		public Animator animator => TaBehaviour.ReturnOrGet<Animator>(this, ref _animator);

		public delegate void ChangeStateDelegate(int layer, int prevStateHash, int currentStateHash);

		public event ChangeStateDelegate onChangeState;

		private Dictionary<int, int> _dict;

		private void Awake()
		{
			_dict = new Dictionary<int, int>();

			for (int i = 0; i < animator.layerCount; i++)
			{
				_dict.Add(i, GetCurrentStateName(i));
			}
		}

		private void Update()
		{
			if (checkOnUpdate) Check();
		}

		private void LateUpdate()
		{
			if (checkOnLateUpdate) Check();
		}

		private void Check()
		{
			for (int i = 0; i < animator.layerCount; i++)
			{
				if (_dict.TryGetValue(i, out var prevStateName))
				{
					var currentStateName = GetCurrentStateName(i);

					if (prevStateName != currentStateName)
					{
						onChangeState?.Invoke(i, prevStateName, currentStateName);
					}

					_dict[i] = currentStateName;
				}
			}
		}

		private int GetCurrentStateName(int index)
		{
			return animator.GetCurrentAnimatorStateInfo(index).fullPathHash;
		}
	}
}