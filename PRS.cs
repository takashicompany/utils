namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using DG.Tweening;
	
	[System.Serializable]
	public struct PRS
	{
		public Vector3 position;
		public Quaternion rotation;
		public Vector3 scale;

		public void SetLocal(Transform transform)
		{
			transform.localPosition = position;
			transform.rotation = rotation;
			transform.localScale = scale;
		}

		public void SetWorld(Transform transform)
		{
			transform.position = position;
			transform.rotation = rotation;
			transform.localScale = scale;
		}

		public static PRS Lerp(PRS a, PRS b, float t)
		{
			return new PRS()
			{
				position = Vector3.Lerp(a.position, b.position, t),
				rotation = Quaternion.Lerp(a.rotation, b.rotation, t),
				scale = Vector3.Lerp(a.scale, b.scale, 1f)
			};
		}
	}

	/// <summary>
	/// Nullable Position Rotation Scale
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

	public static class PRSExtensions
	{
		public static void SetLocal(this Transform transform, PRS prs)
		{
			prs.SetLocal(transform);
		}

		public static void SetWorld(this Transform transform, PRS prs)
		{
			prs.SetWorld(transform);
		}

		public static PRS GetLocalPRS(this Transform transform)
		{
			return new PRS()
			{
				position = transform.localPosition,
				rotation = transform.localRotation,
				scale = transform.localScale
			};
		}

		public static PRS GetWorldPRS(this Transform transform)
		{
			return new PRS()
			{
				position = transform.position,
				rotation = transform.rotation,
				scale = transform.localScale,
			};
		}

		public static Tweener DOLocalPRS(this Transform transform, PRS to, float duration)
		{
			var from = transform.GetLocalPRS();
			var t = 0f;
			return DOTween.To(() => t, v => t = v, 1, duration).OnUpdate(() =>
			{
				transform.SetLocal(PRS.Lerp(from, to, t));
			});
		}
	}

}