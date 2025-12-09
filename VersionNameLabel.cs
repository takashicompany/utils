namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class VersionNameLabel : MonoBehaviour
	{
		[SerializeField]
		private string _format = "Version {0}";

		private void OnEnable()
		{
			var label = GetComponent<TMPro.TextMeshProUGUI>();
			if (label != null)
			{
				label.text = string.Format(_format, Application.version);
			}
		}
	}
}
