namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public interface ITransform
	{
		Transform transform { get; }
	}

	public interface IGameObject
	{
		GameObject gameObject { get; }
	}
}