namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;
#if UNITY_EDITOR
	using UnityEditor;
	using UnityEditor.SceneManagement;
	using UnityEngine.SceneManagement;
#endif

	public abstract class LanguageText : MonoBehaviour
	{
		protected Text _text;
		protected TextMeshProUGUI _textMeshPro;

		protected virtual void Awake()
		{
			_text = GetComponent<Text>();
			_textMeshPro = GetComponent<TextMeshProUGUI>();
			UpdateText();
		}

		protected virtual void UpdateText()
		{
			if (_text != null)
			{
				_text.text = GetString();
			}
			else if (_textMeshPro != null)
			{
				_textMeshPro.text = GetString();
			}
		}

		public abstract string GetString();
	}

	public abstract class LanguageText<T> : LanguageText where T : System.Enum
	{
		[SerializeField]
		protected T _key;
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(LanguageText<>), true)] // 継承クラスにも適用
	public class LanguageTextEditor : UnityEditor.Editor
	{
		private SerializedProperty _keyProperty;

		private void OnEnable()
		{
			_keyProperty = serializedObject.FindProperty("_key");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(_keyProperty);
			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				ApplyTextUpdate();
			}
		}

		private void ApplyTextUpdate()
		{
			LanguageText targetScript = (LanguageText)target;
			Undo.RecordObject(targetScript, "Change Language Key");

			// TextまたはTextMeshProUGUIを直接更新
			Text textComponent = targetScript.GetComponent<Text>();
			TextMeshProUGUI textMeshProComponent = targetScript.GetComponent<TextMeshProUGUI>();

			if (textComponent != null)
			{
				textComponent.text = targetScript.GetString();
				EditorUtility.SetDirty(textComponent);
			}
			else if (textMeshProComponent != null)
			{
				textMeshProComponent.text = targetScript.GetString();
				EditorUtility.SetDirty(textMeshProComponent);
			}

			PrefabUtility.RecordPrefabInstancePropertyModifications(targetScript);
			if (!Application.isPlaying)
			{
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}
	}
#endif
}
