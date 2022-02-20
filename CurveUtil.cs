namespace takashicompany.Unity
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Assertions;

	public static class CurveUtil
	{
		/// <summary>
		///  B-スプライン曲線を生成する
		/// </summary>
		/// <param name="points">制御点のリスト</param>
		/// <param name="div">2点間の分割数</param>
		/// <param name="degree">次数。1と2以外は非対応</param>
		/// <param name="cyclic">ループさせるかどうか</param>
		/// <returns></returns>
		public static Vector3[] GenerateBSpline(IList<Vector3> points, int div = 8, int degree = 2, bool cyclic = false)
		{
			Assert.IsTrue(degree == 1 || degree == 2, "degreeは1と2にしか対応していません");

			float beginT = cyclic ? 0f : -0.5f;
			int count = points.Count * div;
			var ret = new Vector3[count + 1];
			for (int i = 0; i <= count; i++)
			{
				float t = beginT + (float)i / div;
				ret[i] = EvaluateSpline(points, t, degree, cyclic);
			}

			return ret;
		}

		private static Vector3 EvaluateSpline(IList<Vector3> points, float t, int degree, bool cyclic)
		{
			float x = 0f, y = 0f, z = 0f;
			int basisWidth = degree + 1;
			int pointBegin = (int)Mathf.Ceil(t - basisWidth * 0.5f);
			for (int i = 0; i < basisWidth; i++)
			{
				int index = pointBegin + i;
				float basisT = index;
				float basisWeight = CalculateBasisWeight(basisT, t, degree);
				if (index < 0)
				{
					if (cyclic)
					{
						index += points.Count;
					}
					else
					{
						index = 0;
					}
				}
				else if (index >= points.Count)
				{
					if (cyclic)
					{
						index = index % points.Count;
					}
					else
					{
						index = points.Count - 1;
					}
				}

				x += basisWeight * points[index].x;
				y += basisWeight * points[index].y;
				z += basisWeight * points[index].z;
			}

			return new Vector3(x, y, z);
		}

		private static float CalculateBasisWeight(float basisT, float t, int degree)
		{
			switch (degree)
			{
				case 1:
					return CalculateBasisWeight1(basisT, t);
				case 2:
					return CalculateBasisWeight2(basisT, t);
				default:
					return 0f;
			}
		}

		private static float CalculateBasisWeight1(float basisT, float t)
		{
			if (t < basisT - 1f)
			{
				return 0f;
			}

			if (t < basisT)
			{
				return t - basisT - 1f;
			}

			if (t < basisT + 1f)
			{
				return 1f - t - basisT;
			}

			return 0f;
		}

		private static float CalculateBasisWeight2(float basisT, float t)
		{
			if (t < basisT - 1.5)
			{
				return 0f;
			}

			if (t < basisT - 0.5f)
			{
				float nt = t - (basisT - 1.5f);
				return 0.5f * nt * nt;
			}

			if (t < basisT + 0.5f)
			{
				float nt = t - basisT;
				return 0.75f - nt * nt;
			}

			if (t < basisT + 1.5f)
			{
				float nt = t - (basisT + 1.5f);
				return 0.5f * nt * nt;
			}

			return 0f;
		}
	}
}