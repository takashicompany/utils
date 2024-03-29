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

		private static int _lastUpdateFrame = int.MinValue;

		public static event System.Action<int> onUpdateFPS;

		static FPS()
		{
			_remainNextCalc = seconds;
		}

		public static void Update()
		{
			if (_lastUpdateFrame == Time.frameCount)
			{
				Debug.LogError("同じフレームでUpdateが複数呼ばれています");
				return;
			}

			_lastUpdateFrame = Time.frameCount;
			_remainNextCalc -= Time.unscaledDeltaTime;
			
			_count++;

			if (_remainNextCalc <= 0f)
			{
				_fps = (int)(_count / seconds);
				_count = 0;
				_remainNextCalc = seconds;
				onUpdateFPS?.Invoke(_fps);
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

	public class FPSWatcher
	{
		private List<int> _fpsList = new List<int>();

		private int _count;

		public event System.Action<int> onWatchFPS;

		public FPSWatcher(int count)
		{
			_count = count;
			FPS.onUpdateFPS += OnUpdateFPS;
		}

		private void OnUpdateFPS(int fps)
		{
			_fpsList.Add(fps);

			if (_fpsList.Count >= _count)
			{
				var sum = _fpsList.Sum();
				var calc = (sum / _fpsList.Count());

				onWatchFPS?.Invoke(calc);

				_fpsList.Clear();
			}
		}
	}

	public class FPSView : MonoBehaviour
	{
		[SerializeField]
		private UnityEngine.UI.Text _text;

		private void Update()
		{
			FPS.Update();

			if (FPS.TryCurrent(out var fps))
			{
				_text.text = "FPS:" + fps;
			}
			else
			{
				_text.text = "";
			}
		}
	}
}