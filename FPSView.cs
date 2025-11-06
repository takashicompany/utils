namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class FPS
	{
		private int _count = 0;
		private float _remainNextCalc = 0;

		public float seconds { get; protected set; } = 1f;

		private int _fps = int.MinValue;

		private int _lastUpdateFrame = int.MinValue;

		public event System.Action<int> onUpdateFPS;

		public FPS(float seconds = 1f)
		{
			this.seconds = seconds;
			_remainNextCalc = seconds;
		}

		public void Update()
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

		public int Current()
		{
			return _fps;
		}

		public bool TryCurrent(out int fps)
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
		private FPS _fps;
		private List<int> _fpsList = new List<int>();

		private int _count;

		public event System.Action<int> onWatchFPS;

		public FPSWatcher(float seconds, int count)
		{
			_fps = new FPS(seconds);
			_count = count;
			_fps.onUpdateFPS += OnUpdateFPS;
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
		private TextWrapper _text;

		[SerializeField]
		private string _format = "FPS:{0}";

		private FPS _fps;

		private void Awake()
		{
			_fps = new FPS();
		}

		private void Update()
		{
			_fps.Update();

			if (_fps.TryCurrent(out var fps))
			{
				_text.text = string.Format(_format, fps);
			}
			else
			{
				_text.text = "";
			}
		}
	}
}
