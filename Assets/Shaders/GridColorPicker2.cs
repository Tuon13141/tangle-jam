using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class GridColorPicker2 : MonoBehaviour
{
    public Material material;
    public Color[] gridColors = new Color[1024];

    public Color[] currentGridColor;

#if UNITY_EDITOR
    void Awake()
    {
        currentGridColor = Enumerable.Repeat(Color.white, 1024).ToArray();
        ApplyGridColors();
    }

    void OnValidate()
    {
        // Ensure there are exactly 64 colors in the array
        if (gridColors.Length != 1024)
        {
            Debug.LogWarning("Resizing gridColors array to 64.");
            Color[] resizedColors = new Color[1024];
            for (int i = 0; i < 1024; i++)
            {
                resizedColors[i] = i < gridColors.Length ? gridColors[i] : Color.white;
            }
            currentGridColor = gridColors = resizedColors;
        }

        ApplyGridColors();
    }
#endif

    ComputeBuffer colorBuffer;
    public void ApplyGridColors()
    {
        // Convert Unity's Color to Vector4
        Vector4[] colorVectors = new Vector4[1024];
        for (int i = 0; i < 1024; i++)
        {
            colorVectors[i] = currentGridColor[i];
        }

        // Initialize and set the StructuredBuffer
        if (colorBuffer != null)
        {
            colorBuffer.Release();
        }

        colorBuffer = new ComputeBuffer(1024, sizeof(float) * 4);
        colorBuffer.SetData(colorVectors);
        material.SetBuffer("_Colors", colorBuffer);
    }

    private void ReleaseBuffer()
    {
        if (colorBuffer != null)
        {
            colorBuffer.Release();
            colorBuffer = null;
        }
    }

    void OnDisable()
    {
        ReleaseBuffer();
    }

    void OnDestroy()
    {
        ReleaseBuffer();
    }
}
