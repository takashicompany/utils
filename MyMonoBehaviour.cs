namespace TakashiCompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// よく使う機能を入れたMonoBehaviour
	/// </summary>
	public abstract class MyMonoBehaviour : UnityEngine.MonoBehaviour
	{
		private Collider _colliderInternal;

		protected Collider _collider => GetOrFetchComponent(ref _colliderInternal);

		private Rigidbody _rigidbodyInternal;

		protected Rigidbody _rigidbody => GetOrFetchComponent(ref _rigidbodyInternal);
		
		private T GetOrFetchComponent<T>(ref T component)
		{
			return component ?? (component = this.GetComponent<T>());
		}
	}
}