namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[System.Serializable]
	public class IntervalChecker
	{
		public float interval;

		[Header("インターバル代入する際のゆらぎ。0からこの値の間をランダムで追加する")]
		public float intervalJitter;

		[HideInInspector]
		public float nextRemainTime;

		public bool manualReset;

		public IntervalChecker(float interval) : this(interval, false) {}

		public IntervalChecker(float interval, bool manualReset, float intervalJitter = 0f)
		{
			this.interval = interval;
			this.manualReset = manualReset;
			this.intervalJitter = intervalJitter;
			Reset();
		}

		public bool Update(float deltaTime)
		{
			nextRemainTime -= deltaTime;

			if (nextRemainTime <= 0)
			{
				if (!manualReset)
				{
					Reset();
				}

				return true;
			}

			return false;
		}

		private float CalcJitter()
		{
			if (intervalJitter == 0)
			{
				return 0;
			}

			var min = Mathf.Min(0, intervalJitter);
			var max = Mathf.Max(0, intervalJitter);

			return Random.Range(min, max);
		}

		public void Reset()
		{
			nextRemainTime = interval + CalcJitter();
		}

		public bool Update()
		{
			return Update(Time.deltaTime);
		}

		public bool FixedUpdate()
		{
			return Update(Time.fixedDeltaTime);
		}

		public float GetRemainNormalized()
		{
			return Mathf.Clamp01(1f - (nextRemainTime / interval));
		}

		public void Full()
		{
			nextRemainTime = 0;
		}
	}
}