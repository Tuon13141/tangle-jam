using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Array2DGridElement : MonoBehaviour
{
    public Matrix<bool> map;

    public Vector2Int index;

    [Button]
    public void CheckVar()
    {
        var stack = new Stack<Vector2Int>();
        var result = Utils.CanMoveToBusStopCells(map.ToAray2D(), index, out stack);
        Debug.Log(result);
    }
}
