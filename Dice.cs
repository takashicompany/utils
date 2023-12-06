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
			gameObject.SetActive(true);
			transform.position = from;

			var endRotation = new Vector3(90 * Random.Range(0, 4), 90 * Random.Range(0, 4), 90 * Random.Range(0, 4));

			var rot = Quaternion.LookRotation((from - to).normalized).eulerAngles * 360;

			transform.rotation = Quaternion.Euler(-rot);

			var seq = DOTween.Sequence();

			seq.Append(transform.DORotate(endRotation, duration, RotateMode.FastBeyond360).SetEase(Ease.OutSine));
			seq.Join(transform.DOMove(to, duration, Ease.OutSine, Ease.OutBounce, Ease.OutSine));

			return seq;
		}
		
		public void Hide()
		{
			gameObject.SetActive(false);
		}
	}


}