using UnityEngine;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine.U2D;
using System.Collections.Generic;

[CustomEditor(typeof(PolygonCollider2D))]
public class PolygonCollider2DEditor : Editor
{
    private int MaxPointsPerPath = 20;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PolygonCollider2D collider = (PolygonCollider2D)target;

        MaxPointsPerPath = EditorGUILayout.IntField("Limit Point", MaxPointsPerPath);
        if (GUILayout.Button("Auto Fill From Sprite Renderer"))
        {
            AutoFillCollider(collider);
        }
    }

    private void AutoFillCollider(PolygonCollider2D collider)
    {
        // Tìm SpriteRenderer ở chính object hoặc các con
        SpriteRenderer sr = collider.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr = collider.GetComponentInChildren<SpriteRenderer>();
        }

        if (sr == null || sr.sprite == null)
        {
            Debug.LogWarning("SpriteRenderer or Sprite is missing.");
            return;
        }

        Sprite sprite = sr.sprite;

        var physicsShapes = new List<Vector2>();
        int shapeCount = sprite.GetPhysicsShapeCount();

        if (shapeCount == 0)
        {
            Debug.LogWarning("No physics shape in sprite. You may need to set a Custom Physics Shape in Sprite Editor.");
            return;
        }

        collider.pathCount = shapeCount;

        for (int i = 0; i < shapeCount; i++)
        {
            physicsShapes.Clear();
            sprite.GetPhysicsShape(i, physicsShapes);

            List<Vector2> localPoints = new List<Vector2>();
            foreach (var pt in physicsShapes)
            {
                localPoints.Add(pt);
            }

            if (localPoints.Count > MaxPointsPerPath)
            {
                localPoints = SimplifyPath(localPoints, MaxPointsPerPath);
            }

            collider.SetPath(i, localPoints.ToArray());
        }

        Debug.Log("PolygonCollider2D auto-filled from Sprite Renderer (including children).");
        EditorUtility.SetDirty(collider);
    }


    private List<Vector2> SimplifyPath(List<Vector2> path, int maxPoints)
    {
        List<Vector2> simplified = new List<Vector2>();

        int count = path.Count;
        float step = (float)count / maxPoints;

        for (int i = 0; i < maxPoints; i++)
        {
            int index = Mathf.FloorToInt(i * step);
            simplified.Add(path[index % count]);
        }

        return simplified;
    }
}
