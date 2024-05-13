namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class KickerScript : KeyDownKicker
	{
		[System.Serializable]
		private class Timeline
		{
			[Header("時間(秒)")]
			public float time = 1;

			[Header("キー")]			
			public KeyCode key = KeyCode.T;
		}

		[SerializeField, Header("キー入力を時間で指定できるコンポーネント。自動で台本を再生するイメージ。")]
		private Timeline[] _timelines;

		protected override bool _managed => false;

		private float _time;
		private float _fixedTime;

		private bool _isRun;


		[ContextMenu("時間順にソート")]
		private void Sort()
		{
			_timelines = _timelines.OrderBy(x => x.time).ToArray();
		}

		protected override void OnPressKey()
		{
			_time = 0;
			_fixedTime = 0;

			_isRun = true;
		}

		protected override void Update()
		{
			base.Update();

			if (_isRun)
			{
				var nextTime = _time + Time.deltaTime;

				foreach (var timeline in _timelines)
				{
					if (_time <= timeline.time && timeline.time < nextTime)
					{
						TryEmulateAll(timeline.key, UpdateType.Update);
					}
				}

				// _timeへの加算はLateUpdateで行う
			}
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();

			if (_isRun)
			{
				var nextTime = _time + Time.deltaTime;

				foreach (var timeline in _timelines)
				{
					if (_time <= timeline.time && timeline.time < nextTime)
					{
						TryEmulateAll(timeline.key, UpdateType.LateUpdate);
					}
				}

				_time = nextTime;
			}
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();

			if (_isRun)
			{
				var nextTime = _fixedTime + Time.fixedDeltaTime;

				foreach (var timeline in _timelines)
				{
					if (_time <= timeline.time && timeline.time < nextTime)
					{
						TryEmulateAll(timeline.key, UpdateType.FixedUpdate);
					}
				}

				_fixedTime = nextTime;
			}
		}

		private void TryEmulateAll(KeyCode key, UpdateType updateType)
		{
			foreach (var kicker in _kickers)
			{
				kicker.TryEmulateKey(key, updateType);
			}
		}
	}
}
