namespace takashicompany.Unity
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class AddForceKicker : MonoBehaviour
	{
		private enum ForceType
		{
			[InspectorName("特定の方向\"へ\"力を加える")]
			Direction,

			[InspectorName("特定の方向\"から\"力を加える")]
			FromPosition,
		}

		[SerializeField, Header("発動するキー。このオブジェクトが無効状態だと入力を受け付けない。")]
		private KeyCode _key = KeyCode.T;

		[SerializeField, Header("力の種類")]
		private ForceType _forceType;

		[SerializeField, Header("力の方向 or 力の地点")]
		private Vector3 _p = Vector3.up;
		
		[SerializeField, Header("力の大きさ")]
		private float _force = 200f;

		[SerializeField, Header("力のかけ方の種類")]
		private ForceMode _forceMode = ForceMode.Impulse;

		[SerializeField, Header("力をかけたいオブジェクト。未指定ならこのGameObjectになる")]
		private Rigidbody _rigidbody;

		private void FixedUpdate()
		{
			if (Input.GetKeyDown(_key))
			{
				if (_rigidbody == null)
				{
					_rigidbody = GetComponent<Rigidbody>();

					if (_rigidbody == null)
					{
						_rigidbody = gameObject.AddComponent<Rigidbody>();
					}
				}

				if (_rigidbody != null)
				{
					switch (_forceType)
					{
						case ForceType.Direction:
							_rigidbody.AddForce(_p * _force, _forceMode);
							break;

						case ForceType.FromPosition:
							_rigidbody.AddForce((transform.position - _p).normalized * _force, _forceMode);
							break;
					}
				}
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;

			switch (_forceType)
			{
				case ForceType.Direction:
					Gizmos.DrawRay(transform.position, transform.position +  (_p * _force) / 100f);
					Gizmos.DrawWireSphere(transform.position, 1f);
					break;

				case ForceType.FromPosition:
					Gizmos.DrawRay(_p, transform.position - _p);
					Gizmos.DrawWireSphere(_p, 1f);
					break;
			}
		}
	}
}
