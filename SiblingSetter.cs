namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	
	[ExecuteInEditMode]
	public class SiblingSetter : MonoBehaviour
	{
		[SerializeField, Header("Hierarchy内での順番を固定するコンポーネント。負数を設定すると最後になる。")]
		private int _index;

		[SerializeField]
		private bool _runInEditMode;

		private void LateUpdate()
		{
			if (_runInEditMode || Application.isPlaying)
			{
				UpdateIndex();
			}
				
		}

		private void UpdateIndex()
		{
			if (_index < 0)
			{
				transform.SetAsLastSibling();
			}
			else
			{
				transform.SetSiblingIndex(_index);
			}
		}
	}
}