namespace takashicompany.Unity
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

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
		/// 周期付き線形補間列の “index 番目” を計算して返します。<br/>
		/// ─ 各周期は「周期始点 → 周期終点 (始点×<paramref name="endMultiplier"/>)」を
		///   <paramref name="segmentCount"/> 等分で線形補間します。<br/>
		/// ─ 周期終了後、次周期の始点は「周期終点×<paramref name="recycleFactor"/>」。<br/>
		/// 例：start=2, endMultiplier=2, segmentCount=3, recycleFactor=0.75 → 2, 3, 4, 3, 4.5, 6 …<br/>
		/// </summary>
		/// <param name="startValue">最初の始点値 (例: 2)</param>
		/// <param name="endMultiplier">周期終点を決める倍率 (例: 2 → 始点×2 が終点)</param>
		/// <param name="segmentCount">周期を何分割するか (>=1)</param>
		/// <param name="recycleFactor">終点を次周期始点に縮小／拡大する倍率 (例: 0.75)</param>
		/// <param name="index">求めたいインデックス (0 以上)</param>
		/// <returns>指定インデックスに対応する値</returns>
		public static float CalculatePeriodicValue
		(
			float startValue,
			float endMultiplier,
			int segmentCount,
			float recycleFactor,
			int index
		)
		{
			if (segmentCount < 1) throw new System.ArgumentOutOfRangeException(nameof(segmentCount));
			if (index < 0) throw new System.ArgumentOutOfRangeException(nameof(index));

			int cycle = index / segmentCount;                       // 何周期目か
			int pos = index % segmentCount;                       // 周期内位置
			float cycleStart = startValue *
							   Mathf.Pow(endMultiplier * recycleFactor, cycle); // 今周期始点

			if (segmentCount == 1) return cycleStart;                      // 分割1なら補間不要

			float cycleEnd = cycleStart * endMultiplier;                   // 今周期終点
			float t = (float)pos / (segmentCount - 1);              // 0〜1
			return Mathf.Lerp(cycleStart, cycleEnd, t);                    // 線形補間
		}
	}
}
