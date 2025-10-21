using System;
using System.Collections.Generic;
using UnityEngine;

public class MatrixConst
{
    public static readonly Vector2Int DEFAULT_GRID_SIZE = new Vector2Int(3, 3);
    public static readonly Vector2Int DEFAULT_CELL_SIZE = new Vector2Int(64, 20);
}

[System.Serializable]
public sealed class Matrix<T>
{
    [System.Serializable]
    public class MatrixRow<T1>
    {
        public T1[] row;

        public MatrixRow(int size)
        {
            row = new T1[size];
        }
    }

    public Matrix()
    {
        size = MatrixConst.DEFAULT_GRID_SIZE;
        cellSize = MatrixConst.DEFAULT_CELL_SIZE;
    }

    public Vector2Int size
    {
        get
        {
            return new Vector2Int(cells.Length, cells[0].row.Length);
        }

        set
        {
            cells = new MatrixRow<T>[value.x];
            for (int x = 0; x < value.x; x++)
            {
                var matrixRow = new MatrixRow<T>(value.y);
                for (int y = 0; y < value.y; y++)
                {
                    matrixRow.row[y] = default(T);
                }

                cells[x] = matrixRow;
            }
        }
    }

    public Vector2Int cellSize;

    public MatrixRow<T>[] cells;

    public T this[int x, int y]
    {
        get
        {
            if (x < 0 || x >= cells.Length || y < 0 || y >= cells[x].row.Length)
            {
                //Debug.LogError("Index was outside the bounds of the matrix.");
                return default;
            }

            return cells[x].row[y];
        }
        set
        {
            if (x < 0 || x >= cells.Length || y < 0 || y >= cells[x].row.Length)
            {
                //Debug.LogError("Index was outside the bounds of the matrix.");
                return;
            }

            cells[x].row[y] = value;
        }
    }

    public T[,] ToAray2D()
    {
        var array = new T[size.x, size.y];
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                array[x, y] = cells[x].row[y];
            }
        }

        return array;
    }
}
