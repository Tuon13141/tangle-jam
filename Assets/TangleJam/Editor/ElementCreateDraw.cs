using UnityEditor;
using UnityEngine;

namespace Tuon
{
    [CustomEditor(typeof(ElementCreate))]
    [CanEditMultipleObjects]
    public class ElementCreateDraw : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();
            //base.OnInspectorGUI();

            GUILayout.Space(6);
            Rect rect = EditorGUILayout.GetControlRect(false, 10);
            rect.height = 2;
            EditorGUI.DrawRect(rect, new Color32(0, 135, 189, 255));

            var buttonSize = new Vector2Int(85, 20);

            GUILayout.EndVertical();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Empty", GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y)))
            {
                foreach (var obj in targets)
                {
                    ElementCreate comp = (ElementCreate)obj;
                    comp.EmptyElement();
                    EditorUtility.SetDirty(comp);
                }
            }

            if (GUILayout.Button("Stack", GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y)))
            {
                foreach (var obj in targets)
                {
                    ElementCreate comp = (ElementCreate)obj;
                    comp.StackCoilElement();
                    EditorUtility.SetDirty(comp);
                }
            }

            if (GUILayout.Button("Coil", GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y)))
            {
                foreach (var obj in targets)
                {
                    ElementCreate comp = (ElementCreate)obj;
                    comp.CoilElement();
                    EditorUtility.SetDirty(comp);
                }

            }

            if (GUILayout.Button("Coil Mystery", GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y)))
            {
                foreach (var obj in targets)
                {
                    ElementCreate comp = (ElementCreate)obj;
                    comp.CoilMyteryElement();
                    EditorUtility.SetDirty(comp);
                }

            }

            if (GUILayout.Button("Pin Control", GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y)))
            {
                foreach (var obj in targets)
                {
                    ElementCreate comp = (ElementCreate)obj;
                    comp.PinControlElement();
                    EditorUtility.SetDirty(comp);
                }

            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Wall", GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y)))
            {
                foreach (var obj in targets)
                {
                    ElementCreate comp = (ElementCreate)obj;
                    comp.WallElement();
                    EditorUtility.SetDirty(comp);
                }

            }

            if (GUILayout.Button("Button Stack", GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y)))
            {
                foreach (var obj in targets)
                {
                    ElementCreate comp = (ElementCreate)obj;
                    comp.ButtonStack();
                    EditorUtility.SetDirty(comp);
                }
            }

            if (GUILayout.Button("", GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y)))
            {
                foreach (var obj in targets)
                {
                    ElementCreate comp = (ElementCreate)obj;
                    comp.EmptyElement();
                    EditorUtility.SetDirty(comp);
                }
            }
            if (GUILayout.Button("Coil Double", GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y)))
            {
                foreach (var obj in targets)
                {
                    ElementCreate comp = (ElementCreate)obj;
                    comp.CoilDouble();
                    EditorUtility.SetDirty(comp);
                }

            }

            if (GUILayout.Button("Pin Wall", GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y)))
            {
                foreach (var obj in targets)
                {
                    ElementCreate comp = (ElementCreate)obj;
                    comp.PinWallElement();
                    EditorUtility.SetDirty(comp);
                }

            }



            GUILayout.EndHorizontal();
            GUILayout.BeginVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}

