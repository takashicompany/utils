namespace takashicompany.Unity
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using DG.Tweening;

	public static class GameMath
	{
		/// <summary>
		/// 指定された値の平方根に基づいた乗算を行い、その結果を加算する。これを指定回数増やす。
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
			// 50、111.8033989、250、559.0169944、1250、2795.084972、6250、13975.42486、31250、69877.1243、156250、349385.6215
			// 5の平方根 2.236067977

			var multiplier = Mathf.Sqrt(rootBase);
			for (var i = 0; i < repeatCount; i++)
			{
				currentValue = Mathf.RoundToInt(currentValue * multiplier);
			}
			return currentValue;
		}

		public static T RandomKeyByWeight<T>(this IDictionary<T, float> dictionary)
		{
			if (dictionary == null || dictionary.Count == 0)
			{
				throw new ArgumentException("Dictionary is null or empty.");
			}

			// 確率の合計値を算出
			float totalWeight = dictionary.Sum(pair => pair.Value);

			if (totalWeight <= 0)
			{
				throw new InvalidOperationException("Total weight must be greater than zero.");
			}

			// ランダムな値を生成（0から合計重みの範囲で）
			float randomValue = UnityEngine.Random.Range(0f, totalWeight);

			// 適切なキーを選択
			foreach (var pair in dictionary)
			{
				randomValue -= pair.Value;
				if (randomValue <= 0)
				{
					return pair.Key;
				}
			}

			// 理論上はここに到達しないが、念のため最後のキーを返す
			return dictionary.Last().Key;
		}

		/// <summary>
		/// 内部的に辞書を再生成しているので、乱発に注意。多分そんなに問題になることはないと思うけど。
		/// </summary>
		public static (T, R) RandomKeyByWeight<T, R>(this IDictionary<T, (float, R)> dictionaryWithTuple)
		{
			var dictionary = dictionaryWithTuple.ToDictionary(pair => pair.Key, pair => pair.Value.Item1);
			var result = RandomKeyByWeight(dictionary);
			return (result, dictionaryWithTuple[result].Item2);
		}

		/// <summary>
		/// 2^s を起点に「p+1」分割で作った階段数列から、0 始まり i 番目の値を返す。
		/// 例: s=4, p=1 のとき数列は 16,24,32,40,48,56,64... → i=5 なら 56
		/// </summary>
		public static int GetStairValue(int s, int p, int i)
		{
			if (s < 0) throw new ArgumentOutOfRangeException(nameof(s));
			if (p < 1) throw new ArgumentOutOfRangeException(nameof(p));
			if (i < 0) throw new ArgumentOutOfRangeException(nameof(i));

			int baseVal = 1 << s;          // 2^s
			int step = baseVal / (p + 1); // 階段幅 (端数は切り捨て)
			return baseVal + step * i;
		}

		/// <summary>
		/// DOTween の <see cref="Ease"/> で補間カーブを選べる周期関数。<br/>
		/// ・各周期は「始点 → 始点×<paramref name="endMultiplier"/>」を <paramref name="segmentCount"/> 等分で補間。<br/>
		/// ・周期終了後、次周期の始点は「周期終点×<paramref name="recycleFactor"/>」。<br/>
		/// ・補間には <see cref="DOVirtual.EasedValue(float,float,float,Ease)"/> を使用。<br/>
		/// <br/>例: start=2, endMult=2, seg=3, rec=0.75, ease=Ease.OutBounce → 2,3.5,4,3,4.8,6 …
		/// </summary>
		/// <param name="startValue">最初の始点値</param>
		/// <param name="endMultiplier">周期終点を決める倍率</param>
		/// <param name="segmentCount">周期分割数 (>=1)</param>
		/// <param name="recycleFactor">周期終点から次周期始点を得る倍率</param>
		/// <param name="index">0 からのインデックス</param>
		/// <param name="ease">DOTween の Ease 種別 (既定 Linear)</param>
		/// <returns><paramref name="index"/> 番目の値</returns>
		public static float CalculatePeriodicValue
		(
			float startValue,
			float endMultiplier,
			int segmentCount,
			float recycleFactor,
			int index,
			Ease ease = Ease.Linear
		)
		{
			if (segmentCount < 1) throw new System.ArgumentOutOfRangeException(nameof(segmentCount));
			if (index < 0) throw new System.ArgumentOutOfRangeException(nameof(index));

			int cycle = index / segmentCount;                          // 何周期目か
			int pos = index % segmentCount;                          // 周期内位置
			float cycleStart = startValue * Mathf.Pow(endMultiplier * recycleFactor, cycle);

			if (segmentCount == 1) return cycleStart;                         // 分割数 1 → 補間不要

			float cycleEnd = cycleStart * endMultiplier;                      // 周期終点
			float t = (float)pos / (segmentCount - 1);                 // 0〜1
			return DOVirtual.EasedValue(cycleStart, cycleEnd, t, ease);       // DOTween 補間
		}
	}
}
