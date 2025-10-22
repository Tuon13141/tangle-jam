using UnityEditor;
using UnityEngine;

namespace Tuon
{
    public class EditorMenuDraw : EditorWindow
    {
        public delegate void OnApply(StageData.CellData data);

        private static StageData.CellData currentData;
        private static OnApply onApply;

        public static void ShowWindow(StageData.CellData data, OnApply onApplyCallback)
        {
            currentData = data;
            onApply = onApplyCallback;


            // Get existing open window or if none, make a new one
            var window = GetWindow<EditorMenuDraw>();
            window.titleContent = new GUIContent("Edit Cell");
            window.maxSize = new Vector2(250, 105);
            window.minSize = new Vector2(250, 105);

            window.ShowPopup();
        }

        void OnGUI()
        {
            if (currentData == null) return;

            currentData.Type = (StageData.CellType)EditorGUILayout.EnumPopup("Type", currentData.Type);
            currentData.Value = EditorGUILayout.IntField("Value", currentData.Value);
            currentData.Direction = (StageData.Direction)EditorGUILayout.EnumPopup("Direction", currentData.Direction);
            currentData.String = EditorGUILayout.TextField("String", currentData.String);

            if (GUILayout.Button("Apply"))
            {
                Apply();
            }

            GUI.enabled = true;

            // We're doing this in OnGUI() since the Update() function doesn't seem to get called when we show the window with ShowModalUtility().
            var ev = Event.current;
            if (ev.type == EventType.KeyDown || ev.type == EventType.KeyUp)
            {
                switch (ev.keyCode)
                {
                    case KeyCode.Return:
                    case KeyCode.KeypadEnter:
                        Apply();
                        break;
                    case KeyCode.Escape:
                        Close();
                        break;
                }
            }
        }

        private void Apply()
        {
            onApply.Invoke(currentData);
            Close();
        }
    }
}

