namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public static class GameMath
	{
		/// <summary>
		/// 初期値に対して指定された回数だけ、指定された値の平方根に基づいた乗算を行う。
		/// </summary>
		/// <param name="initialValue">乗算の開始値。</param>
		/// <param name="repeatCount">乗算を繰り返す回数。</param>
		/// <param name="rootBase">平方根を取る値。</param>
		/// <returns>計算後の値。</returns>
		public static int MultiplyRootByToInitialValueRepeatedly(int initialValue, int repeatCount, int rootBase)
		{
			var multiplier = Mathf.Sqrt(rootBase);
			for (var i = 0; i < repeatCount; i++)
			{
				initialValue += Mathf.RoundToInt(initialValue * multiplier);
			}
			return initialValue;
		}

		/// <summary>
		/// 現在の値に対して指定された回数だけ、指定された値の平方根に基づいた乗算を行う。
		/// 各ステップでの計算結果は次のステップの入力値として使用される。
		/// </summary>
		/// <param name="currentValue">現在の値。</param>
		/// <param name="repeatCount">乗算を繰り返す回数。</param>
		/// <param name="rootBase">平方根を取る値。</param>
		/// <returns>計算後の値。</returns>
		public static int MultiplyRootBySequentially(int currentValue, int repeatCount, int rootBase)
		{
			var multiplier = Mathf.Sqrt(rootBase);
			for (var i = 0; i < repeatCount; i++)
			{
				currentValue = Mathf.RoundToInt(currentValue * multiplier);
			}
			return currentValue;
		}
	}
}