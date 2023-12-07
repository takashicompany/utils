namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using DG.Tweening;

	public class Dice : MonoBehaviour
	{
		[SerializeField]
		protected int _right = 3;

		[SerializeField]
		protected int _top = 1;

		[SerializeField]
		protected int _forward = 2;

		// 片面が決まれば他の面は自動的に決まるというサイコロの仕様
		protected int _bottom => 7 - _top;
		protected int _left => 7 - _right;
		protected int _back => 7 - _forward;

		public int GetTopNumber()
		{
			Vector3 diceUp = transform.up;
			Vector3 diceForward = transform.forward;
			Vector3 diceRight = transform.right;

			// 各方向ベクトルとY軸との角度を計算
			float upAngle = Vector3.Angle(diceUp, Vector3.up);
			float downAngle = Vector3.Angle(-diceUp, Vector3.up);
			float leftAngle = Vector3.Angle(-diceRight, Vector3.up);
			float rightAngle = Vector3.Angle(diceRight, Vector3.up);
			float forwardAngle = Vector3.Angle(diceForward, Vector3.up);
			float backAngle = Vector3.Angle(-diceForward, Vector3.up);

			// 最もY軸に近い面を判定
			float minAngle = Mathf.Min(new float[] { upAngle, downAngle, leftAngle, rightAngle, forwardAngle, backAngle });
			if (minAngle == upAngle) return _top;
			if (minAngle == downAngle) return _bottom;
			if (minAngle == leftAngle) return _left;
			if (minAngle == rightAngle) return _right;
			if (minAngle == forwardAngle) return _forward;
			if (minAngle == backAngle) return _back;

			return 0; // 何らかのエラーが発生した場合
		}

		public Sequence Throw(Vector3 from, Vector3 to, float duration)
		{
			var endRotation = Quaternion.Euler(new Vector3(90 * Random.Range(0, 4), 90 * Random.Range(0, 4), 90 * Random.Range(0, 4)));
			return Throw(from, to, duration, endRotation);
		}

		public Sequence Throw(Vector3 from, Vector3 to, float duration, int diceNumber)
		{
			Debug.Log($"Throw: {diceNumber}");
			return Throw(transform.position, to, duration, CalculateRotationForNumber(diceNumber));
		}

		private Sequence Throw(Vector3 from, Vector3 to, float duration, Quaternion endRotation)
		{
			gameObject.SetActive(true);
			transform.position = from;

			var rot = Quaternion.LookRotation((from - to).normalized).eulerAngles * 360;

			transform.rotation = Quaternion.Euler(-rot);

			var seq = DOTween.Sequence();

			seq.Append(transform.DORotateQuaternion(endRotation, duration).SetEase(Ease.OutSine));
			seq.Join(transform.DOMove(to, duration, Ease.OutSine, Ease.OutBounce, Ease.OutSine));

			return seq;
		}

		public Quaternion CalculateRotationForNumber(int number)
		{
			Vector3 upDirection;
			Vector3 forwardDirection;

			// 各数値に応じて上向きと前向きの方向を決定
			switch (number)
			{
				case 1: upDirection = Vector3.up; forwardDirection = Vector3.forward; break; // 上面が1
				case 2: upDirection = Vector3.forward; forwardDirection = Vector3.up; break; // 上面が2
				case 3: upDirection = Vector3.right; forwardDirection = Vector3.back; break; // 上面が3
				case 4: upDirection = Vector3.left; forwardDirection = Vector3.back; break; // 上面が4
				case 5: upDirection = Vector3.back; forwardDirection = Vector3.down; break; // 上面が5
				case 6: upDirection = Vector3.down; forwardDirection = Vector3.forward; break; // 上面が6
				default: Debug.LogError("Invalid number provided"); return Quaternion.identity;
			}

			// 指定された方向を向くための回転を計算
			return Quaternion.LookRotation(forwardDirection, upDirection);
		}

		public void RotateByNumber(int number)
		{
			transform.rotation = CalculateRotationForNumber(number);
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}
	}


}