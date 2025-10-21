using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundFitter : MonoBehaviour
{
    
    public Vector2Int size;

    public void Awake()
    {
        var rt = GetComponent<RectTransform>();
        var ratio = size.x * 1f / size.y;
        Setup();
        
    }
    [UnityEngine.ContextMenu("Setup")]
    public void Setup()
    {
        var rt = GetComponent<RectTransform>();
        var fitter = GetComponent<AspectRatioFitter>();
        var ratio = size.x * 1f / size.y;
        
        var aspect = Camera.main.pixelWidth * 1f / Camera.main.pixelHeight;

        if (ratio <= aspect)
        {
            fitter.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
            rt.anchorMax = new Vector2(1,0.5f);
            rt.anchorMin = new Vector2(0,0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = Vector2.zero;
        }
        else
        {
            fitter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
            rt.anchorMax = new Vector2(.5f,1f);
            rt.anchorMin = new Vector2(.5f,0);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = Vector2.zero;
        }
        Debug.Log(ratio + "/"+aspect);
    }
    
}
