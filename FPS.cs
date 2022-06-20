namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public static class FPS
	{
		private static int _count = 0;
		private static float _remainNextCalc = 0;

		public static float seconds = 1f;

		private static int _fps = int.MaxValue;

		static FPS()
		{
			_remainNextCalc = seconds;
		}

		public static void Update()
		{
			_remainNextCalc -= Time.unscaledDeltaTime;
			
			_count++;

			if (_remainNextCalc <= 0f)
			{
				_fps = (int)(_count / seconds);
				_count = 0;
			}
		}

		public static int Current()
		{
			return _fps;
		}

		public static bool TryCurrent(out int fps)
		{
			if (_fps < 0)
			{
				fps = int.MinValue;
				return false;
			}

			fps = _fps;

			return true;
		}
	}
}