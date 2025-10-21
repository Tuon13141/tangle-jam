using NaughtyAttributes;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class RevealByProgress : MonoBehaviour
{
    Renderer _objectRenderer;
    public Renderer objectRenderer
    {
        get
        {
            if (_objectRenderer == null) _objectRenderer = GetComponent<Renderer>();
            return _objectRenderer;
        }
    }

    float _progress;
    public float progress
    {
        get
        {
            return _progress;
        }

        set
        {
            _progress = Mathf.Clamp01(value);

            if (mpb == null) mpb = new MaterialPropertyBlock();
            mpb.SetFloat("_RevealProgress", _progress);
        }
    }

    private void Reset()
    {
        RefreshPosition();
    }

    private void Start()
    {
        RefreshPosition();
    }

    private void Update()
    {
        RefreshPosition();
    }

    MaterialPropertyBlock mpb;
    private void RefreshPosition()
    {
        if (mpb == null) mpb = new MaterialPropertyBlock();

        Bounds bounds = objectRenderer.bounds;
        mpb.SetFloat("_BoundsMinY", bounds.min.y);
        mpb.SetFloat("_BoundsMaxY", bounds.max.y);

        objectRenderer.SetPropertyBlock(mpb, 0);
    }

    [Button]
    void FixPivot()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Renderer objectRenderer = GetComponent<Renderer>();

        if (meshFilter == null || objectRenderer == null)
        {
            Debug.LogError("Missing MeshFilter or Renderer component!");
            return;
        }

        Mesh originalMesh = meshFilter.sharedMesh;
        if (originalMesh == null)
        {
            Debug.LogError("No mesh found on the MeshFilter!");
            return;
        }

        Vector3 center = objectRenderer.bounds.center - Vector3.up * objectRenderer.bounds.size.y / 2f;
        Vector3 pivotOffset = transform.InverseTransformPoint(center);

        // Create a new mesh
        Mesh newMesh = Instantiate(originalMesh);
        newMesh.name = originalMesh.name + "_PivotFixed";
        Vector3[] vertices = newMesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] -= pivotOffset;
        }

        newMesh.vertices = vertices;
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();

        // Assign the new mesh to the MeshFilter
        meshFilter.mesh = newMesh;

        // Move the GameObject to compensate for the pivot change
        transform.position += transform.TransformVector(pivotOffset);

        // Save the new mesh
        SaveMesh(newMesh, gameObject.name);
    }

    void SaveMesh(Mesh mesh, string name)
    {
#if UNITY_EDITOR
        string path = "Assets/SavedMeshes/";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        string assetPath = path + name + ".asset";

        UnityEditor.AssetDatabase.CreateAsset(mesh, assetPath);
        UnityEditor.AssetDatabase.SaveAssets();

        Debug.Log("Mesh saved to: " + assetPath);
#endif
    }
}