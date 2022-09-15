namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public interface ITransform
	{
		GameObject gameObject { get; }
	}

	public interface IGameObject
	{
		Transform transform { get; }
	}
}