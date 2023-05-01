namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class AnimationReplacer : MonoBehaviour
	{	
		[SerializeField]
		private List<SerializableDictionary<string, AnimationClip>> _layers;

		[SerializeField]
		private List<SerializableDictionary<string, string>> _maps;

		[SerializeField]
		private Animator _animator;

		private void Awake()
		{
			var c = _animator.runtimeAnimatorController;
			
		}

#if UNITY_EDITOR
		[ContextMenu("Generate Dictionary")]
		private void GenerateDictionary()
		{
			if (_animator.runtimeAnimatorController is UnityEditor.Animations.AnimatorController controller)
			{
				while (controller.layers.Length > _layers.Count)
				{
					_layers.Add(new SerializableDictionary<string, AnimationClip>());
				}

				for (int i = 0; i < controller.layers.Length; i++)
				{
					var layer = _layers[i];
					var map = _maps[i];
					
					foreach (var state in controller.layers[i].stateMachine.states)
					{
						if (!layer.ContainsKey(state.state.name))
						{
							layer.Add(state.state.name, null);
						}
					}
				}
			}
		}

		[ContextMenu("Delete Dictionary")]
		private void Delete()
		{
			if (_animator.runtimeAnimatorController is UnityEditor.Animations.AnimatorController controller)
			{
				for (int i = 0; i < _layers.Count; i++)
				{
					var dict = _layers[i];
					
					var list = new List<string>();

					foreach (var kvp in dict)
					{
						var state = controller.layers[i].stateMachine.states.Where(s => s.state.name == kvp.Key);

						if (state == null)
						{
							list.Add(kvp.Key);
						}
					}

					foreach (var stateName in list)
					{
						dict.Remove(stateName);
						Debug.Log("Delete:" + stateName);
					}
				}
			}
		}
#endif
	}
}