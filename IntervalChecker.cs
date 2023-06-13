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
		public float nextRemainTime;
		public bool manualReset;

		public IntervalChecker(float interval) : this(interval, false) {}

		public IntervalChecker(float interval, bool manualReset)
		{
			this.interval = interval;
			this.nextRemainTime = 0;
			this.manualReset = manualReset;
		}

		public bool Update(float deltaTime)
		{
			nextRemainTime -= deltaTime;

			if (nextRemainTime <= 0)
			{
				if (!manualReset)
				{
					nextRemainTime = interval;
				}

				return true;
			}

			return false;
		}

		public void Reset()
		{
			nextRemainTime = interval;
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
			return 1f - (nextRemainTime / interval);
		}
	}
}