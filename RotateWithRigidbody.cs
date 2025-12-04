namespace takashicompany.Unity
{
	using System.Collections;
	using UnityEngine;

	public class RotateWithRigidbody : MonoBehaviour
	{
		[SerializeField, Header("未指定ならこのコンポーネントについているRigidbodyを使う")]
		private Rigidbody _rigidbody;

		[SerializeField, Header("1ループあたりの回転量（度）")]
		private Vector3 _rotation = new Vector3(0f, 90f, 0f);

		[SerializeField, Header("1ループにかける時間（秒）")]
		private float _duration = 1f;

		[SerializeField]
		private float _delay = 0f;

		[SerializeField, Header("true: ローカル軸 / false: ワールド軸")]
		private bool _isLocalAxis = true;

		[SerializeField, Tooltip("-1 のとき無限ループ")]
		private int _loopCount = -1;

		private Coroutine _rotateCoroutine;

		private void Awake()
		{
			if (_rigidbody == null)
			{
				_rigidbody = GetComponent<Rigidbody>();
			}
		}

		private void OnEnable()
		{
			if (_rigidbody == null)
			{
				Debug.LogError("RotateWithRigidbody: Rigidbody is not assigned.", this);
				return;
			}

			if (_rotateCoroutine != null)
			{
				StopCoroutine(_rotateCoroutine);
			}

			_rotateCoroutine = StartCoroutine(RotateRoutine());
		}

		private void OnDisable()
		{
			if (_rotateCoroutine != null)
			{
				StopCoroutine(_rotateCoroutine);
				_rotateCoroutine = null;
			}
		}

		private IEnumerator RotateRoutine()
		{
			if (_duration <= 0f)
			{
				yield break;
			}

			// 1秒あたりの回転量（度）
			Vector3 angularPerSecond = _rotation / _duration;

			int doneLoops = 0;

			while (_loopCount < 0 || doneLoops < _loopCount)
			{
				// 各ループごとに delay を挟む
				if (_delay > 0f)
				{
					yield return new WaitForSeconds(_delay);
				}

				float elapsed = 0f;

				while (elapsed < _duration)
				{
					yield return new WaitForFixedUpdate();

					float dt = Time.fixedDeltaTime;
					elapsed += dt;

					Vector3 deltaAngle = angularPerSecond * dt;
					Quaternion deltaRot = Quaternion.Euler(deltaAngle);

					if (_isLocalAxis)
					{
						_rigidbody.MoveRotation(_rigidbody.rotation * deltaRot);
					}
					else
					{
						_rigidbody.MoveRotation(deltaRot * _rigidbody.rotation);
					}
				}

				doneLoops++;
			}
		}
	}
}
