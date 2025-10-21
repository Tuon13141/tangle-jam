using UnityEngine;
using UnityEditor;

public class ScriptFinder : MonoBehaviour
{
    [MenuItem("Tools/Find Scripts on Prefab")]
    static void FindScripts()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.LogWarning("Select a prefab or GameObject in the hierarchy!");
            return;
        }

        Component[] components = selected.GetComponentsInChildren<Component>(true);
        foreach (var comp in components)
        {
            if (comp == null) continue; // skip missing scripts
            var type = comp.GetType();
            if (type.Namespace == null || type.Namespace.StartsWith("UnityEngine") == false)
            {
                Debug.Log($"[Script Attached] {type.Name} on {comp.gameObject.name}");
            }
        }
    }
}
