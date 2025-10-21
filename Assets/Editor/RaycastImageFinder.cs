using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class RaycastImageCleaner : MonoBehaviour
{
    [MenuItem("Tools/Cleanup Raycast Targets")]
    static void CleanupRaycastTargets()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.LogWarning("Select a GameObject or prefab in the hierarchy!");
            return;
        }

        int modifiedCount = 0;
        Image[] images = selected.GetComponentsInChildren<Image>(true);
        foreach (var img in images)
        {
            if (img == null) continue;

            // Skip if part of a Button
            if (img.GetComponent<Button>() != null || img.GetComponentInParent<Button>() != null)
                continue;

            // Disable raycastTarget if it's enabled
            if (img.raycastTarget)
            {
                img.raycastTarget = false;
                EditorUtility.SetDirty(img); // Mark the object dirty for saving
                Debug.Log($"[Disabled raycastTarget] on {img.gameObject.name}", img.gameObject);
                modifiedCount++;
            }
        }

        Debug.Log($"✅ Cleanup complete. Modified {modifiedCount} Image(s).");
    }
}
