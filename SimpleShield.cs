namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	/// <summary>
	///  簡易的な操作をブロックするシールド
	/// </summary>
	public class SimpleShield : MonoBehaviour
	{
		private HashSet<object> _objects = new ();

		public bool IsActive()
		{
			return gameObject.activeSelf;
		}

		private void Update()
		{
			if (_objects.Count == 0)
			{
				gameObject.SetActiveIfNot(false);
			}
		}

		public bool Display(object obj)
		{
			if (!_objects.Add(obj))
			{
				return false;
			}

			gameObject.SetActiveIfNot(true);
			return true;
		}

		public bool Hide(object obj)
		{
			if (!_objects.Remove(obj))
			{
				return false;
			}

			if (_objects.Count == 0)
			{
				gameObject.SetActiveIfNot(false);
			}

			return true;
		}
	}
}
