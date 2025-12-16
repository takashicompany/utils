namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public abstract class UpdateTypeSelector : MonoBehaviour
	{
		[SerializeField]
		protected UpdateType _updateType = UpdateType.Update;

		protected void Update()
		{
			if (_updateType.IsUpdate()) OnUpdate();
		}

		protected void LateUpdate()
		{
			if (_updateType.IsLateUpdate()) OnLateUpdate();
		}

		protected void FixedUpdate()
		{
			if (_updateType.IsFixedUpdate()) OnFixedUpdate();
		}

		protected virtual void OnUpdate()
		{

		}

		protected virtual void OnLateUpdate()
		{

		}

		protected virtual void OnFixedUpdate()
		{

		}
	}
}
