using System.IO;
using UnityEditor;
using UnityEngine;

namespace LevelBase.Editor
{
    public class ScriptableObjectCreatTool : EditorWindow
    {
        [MenuItem("Tools/Scriptable Object Creat Tool")]
        private static void Init() => GetWindow<ScriptableObjectCreatTool>();
        //private Vector2 windowScrollPos;
        //private string _selectedFilePath = "";
        private string _content = "";
        private string _text = "";
        private string _levelName = "";
        private const string FolderPath = "Assets/Resources/GenerateSO";
        public LevelAsset levelData;
        private void OnGUI()
        {
            EditorGUIUtility.wideMode = true;
            //GUILayout.Label("Select file .txt");
            //if (GUILayout.Button("Load"))
            //{
            //    var path = EditorUtility.OpenFilePanel("Load File", Application.persistentDataPath, "txt");

            //    if (!string.IsNullOrEmpty(path))
            //    {
            //        _selectedFilePath = path;
            //        var fileContent = File.ReadAllText(path);
            //        _content = Static.DecompressString(fileContent);
            //        var levelNameFull = Path.GetFileNameWithoutExtension(path);
            //        _levelName = levelNameFull.StartsWith("Level_") ? levelNameFull.Substring("Level_".Length) : levelNameFull;
            //        Debug.Log($"Load file: {Path.GetFileName(path)}");
            //        Debug.Log($"Content:\n{_content}");
            //    }
            //}

            EditorGUILayout.LabelField("input string encode");
            _text = EditorGUILayout.TextArea(_text, GUILayout.Height(100));

            if (GUILayout.Button("Create Scriptable Object"))
            {
                _content = Static.DecompressString(_text);
                if (string.IsNullOrEmpty(_content))
                {
                    Debug.LogError("JSON is empty!");
                    return;
                }
                var level = ScriptableObject.CreateInstance<LevelAsset>();
                JsonUtility.FromJsonOverwrite(_content, level);
                levelData = level;
                var path = AssetDatabase.GenerateUniqueAssetPath($"{FolderPath}/Level_{_levelName}.asset");
                AssetDatabase.CreateAsset(levelData, path);
                EditorUtility.SetDirty(levelData);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = levelData;
                EditorGUIUtility.PingObject(levelData);
            }

            //if (!string.IsNullOrEmpty(_selectedFilePath))
            //{
            //    GUILayout.Label("Loaded file:");
            //    GUILayout.Label(_selectedFilePath, EditorStyles.wordWrappedLabel);
            //    windowScrollPos = EditorGUILayout.BeginScrollView(windowScrollPos, GUILayout.MaxHeight(300));
            //    GUILayout.Label("String in file:");
            //    GUILayout.Label(_content, EditorStyles.wordWrappedLabel);
            //    EditorGUILayout.EndScrollView();
            //    GUILayout.Label("Level name:" + _levelName);
            //}


        }
    }
}