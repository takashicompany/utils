#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace takashicompany.Unity.Editor
{
    public class VoxelMapGeneratorWindow : EditorWindow
    {
        private VoxelMapGenerator _generator;
        private Vector2 _scrollPos;
        private string _saveFileName = "VoxelMap";
        private string _savePath = "";
        
        [MenuItem("TC Utils/2Dマップ生成")]
        public static void ShowWindow()
        {
            GetWindow<VoxelMapGeneratorWindow>("2Dマップ生成ツール");
        }
        
        private void OnEnable()
        {
            if (_generator == null)
            {
                _generator = new VoxelMapGenerator();
            }
            
            // デフォルトの保存パスを設定
            _savePath = Path.Combine(Application.dataPath, "Generated");
        }
        
        private void OnGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            
            GUILayout.Label("2Dマップ生成ツール", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // マップ設定
            EditorGUILayout.LabelField("マップ設定", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            _generator.width = EditorGUILayout.IntField("幅", _generator.width);
            _generator.height = EditorGUILayout.IntField("高さ", _generator.height);
            _generator.seaLevel = EditorGUILayout.Slider("海面レベル", _generator.seaLevel, 0f, 1f);
            _generator.frequency = EditorGUILayout.Slider("周波数", _generator.frequency, 0.5f, 10f);
            _generator.octaves = EditorGUILayout.IntSlider("オクターブ数", _generator.octaves, 1, 8);
            _generator.persistence = EditorGUILayout.Slider("永続性", _generator.persistence, 0f, 1f);
            _generator.seed = EditorGUILayout.IntField("シード値", _generator.seed);
            _generator.blurPasses = EditorGUILayout.IntSlider("ブラーパス", _generator.blurPasses, 0, 3);
            
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            
            // 色設定
            EditorGUILayout.LabelField("色設定", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            _generator.seaColor = EditorGUILayout.ColorField("海の色", _generator.seaColor);
            _generator.landLowColor = EditorGUILayout.ColorField("低地の色", _generator.landLowColor);
            _generator.landHighColor = EditorGUILayout.ColorField("高地の色", _generator.landHighColor);
            
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            
            
            // プレビュー領域
            if (_generator.tex != null)
            {
                EditorGUILayout.LabelField("プレビュー", EditorStyles.boldLabel);
                var rect = GUILayoutUtility.GetRect(256, 256, GUILayout.ExpandWidth(false));
                GUI.DrawTexture(rect, _generator.tex, ScaleMode.ScaleToFit);
                EditorGUILayout.Space();
            }
            
            // 生成ボタン
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("マップを生成", GUILayout.Height(30)))
            {
                GenerateMap();
            }
            
            if (GUILayout.Button("ランダムシード", GUILayout.Height(30)))
            {
                _generator.seed = Random.Range(0, 10000);
                GenerateMap();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // 保存設定
            EditorGUILayout.LabelField("保存設定", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("保存パス:", GUILayout.Width(60));
            _savePath = EditorGUILayout.TextField(_savePath);
            if (GUILayout.Button("参照", GUILayout.Width(50)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("保存先フォルダを選択", _savePath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    _savePath = selectedPath;
                }
            }
            EditorGUILayout.EndHorizontal();
            
            _saveFileName = EditorGUILayout.TextField("ファイル名", _saveFileName);
            
            EditorGUI.indentLevel--;
            
            // 保存ボタン
            GUI.enabled = _generator.tex != null;
            if (GUILayout.Button("PNGとして保存", GUILayout.Height(30)))
            {
                SaveMap();
            }
            GUI.enabled = true;
            
            EditorGUILayout.Space();
            
            // 情報表示
            if (_generator.tex != null)
            {
                EditorGUILayout.LabelField($"生成済みマップサイズ: {_generator.tex.width} x {_generator.tex.height}");
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        private void GenerateMap()
        {
            _generator.Generate();
            Repaint();
        }
        
        private void SaveMap()
        {
            if (_generator.tex == null)
            {
                EditorUtility.DisplayDialog("エラー", "保存するマップが生成されていません。\n先にマップを生成してください。", "OK");
                return;
            }
            
            // ディレクトリが存在しない場合は作成
            if (!Directory.Exists(_savePath))
            {
                Directory.CreateDirectory(_savePath);
            }
            
            string fileName = _saveFileName;
            if (!fileName.EndsWith(".png"))
            {
                fileName += ".png";
            }
            
            string fullPath = Path.Combine(_savePath, fileName);
            
            try
            {
                byte[] bytes = _generator.tex.EncodeToPNG();
                File.WriteAllBytes(fullPath, bytes);
                
                // Assetsフォルダ内の場合はAssetDatabaseを更新
                if (fullPath.StartsWith(Application.dataPath))
                {
                    AssetDatabase.Refresh();
                }
                
                EditorUtility.DisplayDialog("保存完了", $"マップを保存しました:\n{fullPath}", "OK");
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("保存エラー", $"保存に失敗しました:\n{e.Message}", "OK");
            }
        }
    }
}
#endif