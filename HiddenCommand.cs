namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.Events;

	public class HiddenCommand : MonoBehaviour
	{
		[SerializeField, Header("この秒数の間、画面の4隅を長押しし続けると隠しコマンドが発動する。")]
		private float _waitDuration = 10f;

		[SerializeField, Header("隠しコマンドが実行された際に呼ばれるコールバック")]
		private UnityEvent _onOpen;

		public UnityEvent onOpen => _onOpen;

		private float _debuggerTime;

		private void Update()
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
				_debuggerTime += Time.deltaTime;
			}
			else
			{
				_debuggerTime = 0;
			}

			if (_debuggerTime > _waitDuration)
			{
				_debuggerTime = 0;
				onOpen?.Invoke();
			}
		}
	}
}