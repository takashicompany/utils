namespace TakashiCompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Position Rotation Scale
	/// </summary>
	public struct NullablePRS
	{
		public Vector3? position { get; private set; }
		public Quaternion? rotation { get; private set; }
		public Vector3? scale { get; private set; }

		public NullablePRS(Vector3? position, Quaternion? rotation, Vector3? scale)
		{
			this.position = position;
			this.rotation = rotation;
			this.scale = scale;
		}

		public NullablePRS(Vector3 position)
		{
			this.position = position;
			this.rotation = null;
			this.scale = null;
		}

		public NullablePRS(Vector3 position, Quaternion rotation) : this(position)
		{
			this.rotation = rotation;
		}

		public NullablePRS(Vector3 position, Quaternion rotation, Vector3 scale) : this (position, rotation)
		{
			this.scale = scale;
		}

		public void Set(Transform transform)
		{
			if (this.position.TryGet(out var position))
			{
				transform.position = position;
			}

			if (this.rotation.TryGet(out var rotation))
			{
				transform.rotation = rotation;
			}

			if (this.scale.TryGet(out var scale))
			{
				transform.localScale = scale;
			}
		}
	}

}