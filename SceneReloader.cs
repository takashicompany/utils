namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class SceneReloader : MonoBehaviour
	{
		[SerializeField]
		private KeyCode _resetKeyCode = KeyCode.R;

		[SerializeField]
		private int _sceneIndex = 0;

		private void Update()
		{
			if (Input.GetKeyDown(_resetKeyCode))
			{
				UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneIndex);
			}
		}
	}
}