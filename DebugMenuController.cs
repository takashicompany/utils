namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class IMDebugMenu : IMGrid
	{
		private float _debugOpenTime;
		private float _pressTime;

		public bool isOpened { get; private set; }

		private KeyCode _keyCode;

		public IMDebugMenu(float debugOpenTime = 10f, KeyCode keyCode = KeyCode.P) : base()
		{
			_debugOpenTime = debugOpenTime;
			_keyCode = keyCode;
		}

		public void Update(float deltaTime)
		{
			var success = false;

			if (Input.touchCount == 4)
			{
				var leftTop = false;
				var rightTop = false;
				var leftBottom = false;
				var rightBottom = false;

				var cellWidth = Screen.width / 4;
				var cellHeight = Screen.height / 4;

				var leftX = cellWidth;
				var rightX = Screen.width - cellWidth;
				var bottomY = cellHeight;
				var topY = Screen.height - cellHeight;

		
				for (int i = 0; i < Input.touchCount; i++)
				{
					var touch = Input.GetTouch(i);

					// 左上
					if (touch.position.x <= leftX && touch.position.y >= topY)
					{
						leftTop = true;
					}
					// 右上
					else if (touch.position.x >= rightX && touch.position.y >= topY)
					{
						rightTop = true;
					}
					// 左下
					else if (touch.position.x <= leftX && touch.position.y <= bottomY)
					{
						leftBottom = true;
					}
					else if (touch.position.x >= rightX && touch.position.y <= bottomY)
					{
						rightBottom = true;
					}
				}

				success = leftTop && rightTop && leftBottom && rightBottom;
			}

			if (success)
			{
				_pressTime += deltaTime;
			}
			else
			{
				_pressTime = 0;
			}

			if ((_pressTime > _debugOpenTime && !isOpened) || Input.GetKeyDown(_keyCode))
			{
				_pressTime = 0;
				isOpened = true;
			}
		}

		public bool OnGUI(System.Action callback)
		{
			if (isOpened)
			{
				callback();
				return true;
			}
			else
			{
				return false;
			}
		}

		public IMDebugMenu Close()
		{
			_pressTime = 0;
			isOpened = false;
			return this;
		}
	}
}
