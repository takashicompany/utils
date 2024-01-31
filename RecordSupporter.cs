namespace takashicompany.Unity
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.Events;

#if UNITY_EDITOR
	using UnityEditor;
	using UnityEditor.Animations;
#endif

	public class RecordSupporter : MonoBehaviour
	{
		// Clip生成時に最初のキーフレームより前と最後のキーフレームより後を削除する
		[SerializeField]
		private bool _isTrimStartAndEnd = true;

		// Clip生成後の保存先。空やnullの場合はAssets以下に保存する
		[SerializeField]
		private string _savePath;

		[SerializeField]
		private UnityEvent<RecordSupporter> _onRecordStart;

		[SerializeField]
		private UnityEvent<RecordSupporter, AnimationClip> _onRecordEnd;

		public bool isRecording { get; private set; }

		private AnimationClip _lastAnimationClip;

		private Animation _animation;

		[ContextMenu(nameof(StartRecord))]
		public virtual void StartRecord()
		{
#if UNITY_EDITOR
			StartRecordInternal();
#endif
		}

		[ContextMenu(nameof(StopRecord))]
		public virtual void StopRecord()
		{
#if UNITY_EDITOR
			StopRecordInternal();
#endif
		}

		[ContextMenu(nameof(PlayLastClip))]
		public virtual void PlayLastClip()
		{
			if (_lastAnimationClip == null)
			{
				Debug.LogError("Last Animation Clip is null");
				return;
			}

			if (_animation == null)
			{
				if (!TryGetComponent(out _animation))
				{
					_animation = gameObject.AddComponent<Animation>();
					_animation.playAutomatically = false;
				}
			}

			// _animationの全てのClipを削除
			foreach (AnimationState state in _animation)
			{
				_animation.RemoveClip(state.clip);
			}

			_animation.AddClip(_lastAnimationClip, _lastAnimationClip.name);

			// クリップが追加されたかの確認
			if (_animation.GetClip(_lastAnimationClip.name) == null)
			{
				Debug.LogError("Failed to add clip: " + _lastAnimationClip.name);
				return;
			}

			// ここでデフォルトのクリップを設定
			_animation.clip = _lastAnimationClip;

			// アニメーションを再生
			if (!_animation.Play(_lastAnimationClip.name))
			{
				Debug.LogError("Failed to play clip: " + _lastAnimationClip.name);
			}
		}



#if UNITY_EDITOR
		private GameObjectRecorder _recoder;

		private void NewRecorder()
		{
			_recoder = new GameObjectRecorder(gameObject);
			_recoder.BindComponentsOfType<Transform>(gameObject, false);
		}

		private void StartRecordInternal()
		{
			if (isRecording)
			{
				return;
			}

			NewRecorder();

			isRecording = true;

			_onRecordStart?.Invoke(this);
		}

		private void StopRecordInternal()
		{
			if (!isRecording)
			{
				return;
			}

			isRecording = false;

			var clip = new AnimationClip();

			_recoder.SaveToClip(clip);
			clip.legacy = true;

			_onRecordEnd?.Invoke(this, clip);

			// _savePathが空の時はAssets以下に保存する。ファイル名はこのGameObjectの名前 + _yyyy-MM-dd_HH-mm-ss.拡張子

			if (string.IsNullOrEmpty(_savePath))
			{
				_savePath = "Assets/";
			}

			// if (!_savePath.StartsWith("Assets/"))
			// {
			// 	_savePath = "Assets/" + _savePath;
			// }

			if (!_savePath.EndsWith("/"))
			{
				_savePath += "/";
			}

			if (!System.IO.Directory.Exists(_savePath))
			{
				System.IO.Directory.CreateDirectory(_savePath);
			}

			var fileName = $"{gameObject.name}_{System.DateTime.Now:yyyy-MM-dd-HH-mm-ss}.anim";

			if (_isTrimStartAndEnd)
			{
				AnimationUtility.SetAnimationClipSettings(clip, new AnimationClipSettings
				{
					startTime = 0,
					stopTime = clip.length,
					loopTime = false,
					loopBlend = false,
					keepOriginalOrientation = true,
					keepOriginalPositionXZ = true,
					keepOriginalPositionY = true,
					heightFromFeet = false,
					mirror = false
				});
			}

			AssetDatabase.CreateAsset(clip, _savePath + fileName);

			AssetDatabase.SaveAssets();

			_lastAnimationClip = clip;
		}

		protected virtual void LateUpdate()
		{
			if (isRecording)
			{
				_recoder.TakeSnapshot(Time.deltaTime);
			}
		}
#endif
	}

#if UNITY_EDITOR


	public class RecordSupporterEditor<T> : Editor where T : RecordSupporter
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			T instance = (T)target;

			GUILayout.BeginHorizontal();

			EditorGUI.BeginDisabledGroup(!Application.isPlaying);

			if (instance.isRecording)
			{
				if (GUILayout.Button("Stop Recording"))
				{
					instance.StopRecord();
				}
			}
			else
			{
				if (GUILayout.Button("Start Recording"))
				{
					instance.StartRecord();
				}
			}

			EditorGUI.EndDisabledGroup();

			if (instance.isRecording) EditorGUI.BeginDisabledGroup(!Application.isPlaying);

			if (GUILayout.Button("Play Last Clip"))
			{
				instance.PlayLastClip();
			}

			if (instance.isRecording) EditorGUI.EndDisabledGroup();

			GUILayout.EndHorizontal();
		}
	}

	[CustomEditor(typeof(RecordSupporter))]
	public class RecordSupporterEditor : RecordSupporterEditor<RecordSupporter>
	{
		
	}
#endif

}