using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    //check fdf
    public static bool CanMoveToBusStopCells(bool[,] levelMap, Vector2Int humanIndex, out Stack<Vector2Int> validIndexes)
    {
        if (humanIndex.y == levelMap.GetLength(1) - 1 && humanIndex.x >= 0 && humanIndex.x < levelMap.GetLength(0))// Nếu ở đầu hàng
        {
            validIndexes = new Stack<Vector2Int>();
            return true;
        }

        List<Vector2Int> checkedNonValidCells = new List<Vector2Int>();
        validIndexes = new Stack<Vector2Int>();
        return CheckCells(levelMap, humanIndex, ref checkedNonValidCells, ref validIndexes, ref humanIndex);
    }

    public static bool CanMoveToBusStopCellsAStar(bool[,] levelMap, Vector2Int humanIndex, out Stack<Vector2Int> validIndexes)
    {
        if (humanIndex.y == levelMap.GetLength(1) - 1 && humanIndex.x >= 0 && humanIndex.x < levelMap.GetLength(0))// Nếu ở đầu hàng
        {
            validIndexes = new Stack<Vector2Int>();
            return true;
        }

        //var path = AStarPathfinding.GeneratePathSync(currentX, currentY, randomX, randomY, walkableMap);

        //if (path.Length != 0)
        //{
        //    path_index = 0;
        //    target = DemoGrid.Instance.cordinateToWorldSpace(path[path_index].Item1, path[path_index].Item2);
        //    break;
        //}

        validIndexes = validIndexes = new Stack<Vector2Int>();
        return false;
    }

    static bool CheckCells(bool[,] levelMap, Vector2Int index,
                           ref List<Vector2Int> checkedNonValidCells,
                           ref Stack<Vector2Int> validIndexes,
                           ref Vector2Int startIndex)
    {
        if (validIndexes.Contains(index)) //điều kiện dừng khi đệ quy
        {
            return false;
        }
        if (index != startIndex)
        {
            if (checkedNonValidCells.Contains(index)) return false;

            if (index.y < 0 || index.y >= levelMap.GetLength(1) ||
                   index.x < 0 || index.x >= levelMap.GetLength(0) ||
                   !levelMap[index.x, index.y])
            {
                if (!checkedNonValidCells.Contains(index))
                    checkedNonValidCells.Add(index);
                return false;
            }
            validIndexes.Push(index);
        }

        if (index.y == levelMap.GetLength(1) - 1 && index.x >= 0 && index.x < levelMap.GetLength(0))
        {
            if (levelMap[index.x, index.y]) return true;
        }

        if (CheckCells(levelMap, index + Vector2Int.up, ref checkedNonValidCells,
                ref validIndexes, ref startIndex) ||
            CheckCells(levelMap, index + Vector2Int.left, ref checkedNonValidCells,
                ref validIndexes, ref startIndex) ||
            CheckCells(levelMap, index + Vector2Int.right, ref checkedNonValidCells,
                ref validIndexes, ref startIndex) ||
            CheckCells(levelMap, index + Vector2Int.down, ref checkedNonValidCells,
                ref validIndexes, ref startIndex))
        {
            return true;
        }
        else
        {
            if (validIndexes.Contains(index))
                checkedNonValidCells.Add(validIndexes.Pop());
            return false;
        }
    }


    private static readonly Vector2Int[] Directions = {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0)
    };

    private class Node
    {
        public Vector2Int Pos;
        public int Cost;
        public Node Parent;

        public Node(Vector2Int pos, int cost, Node parent)
        {
            Pos = pos;
            Cost = cost;
            Parent = parent;
        }
    }

    public static bool FindPath(int[,] map, Vector2Int start, out Stack<Vector2Int> solver, out int totalCost)
    {
        solver = new Stack<Vector2Int>();
        totalCost = -1;

        int width = map.GetLength(0);
        int height = map.GetLength(1);
        int targetRow = height - 1;

        bool[,] visited = new bool[width, height];
        int[,] costMap = new int[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                costMap[x, y] = int.MaxValue;

        List<Node> openList = new List<Node>();
        Node startNode = new Node(start, 0, null);
        openList.Add(startNode);
        costMap[start.x, start.y] = 0;

        while (openList.Count > 0)
        {
            // Lấy node có cost nhỏ nhất
            Node current = openList[0];
            foreach (var node in openList)
                if (node.Cost < current.Cost)
                    current = node;

            openList.Remove(current);
            Vector2Int pos = current.Pos;

            if (visited[pos.x, pos.y]) continue;
            visited[pos.x, pos.y] = true;

            int currentValue = map[pos.x, pos.y];

            // Nếu đến hàng cuối và là ô đi được
            if (pos.y == targetRow && map[pos.x, pos.y] != -1)
            {
                totalCost = current.Cost;

                while (current != null)
                {
                    solver.Push(current.Pos);
                    current = current.Parent;
                }
                return true;
            }

            foreach (var dir in Directions)
            {
                Vector2Int next = pos + dir;
                if (next.x < 0 || next.y < 0 || next.x >= width || next.y >= height)
                    continue;

                int cost = map[next.x, next.y];
                int nextValue = map[next.x, next.y];

                if (cost < 0 || visited[next.x, next.y]) continue;

                if (currentValue == 2 && nextValue == 0) continue;

                int newCost = current.Cost + cost;
                if (newCost < costMap[next.x, next.y])
                {
                    costMap[next.x, next.y] = newCost;
                    Node nextNode = new Node(next, newCost, current);
                    openList.Add(nextNode);
                }
            }
        }

        return false;
    }
}