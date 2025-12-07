using System.Collections.Generic;

public static class Pathfinding
{
    public static List<Cell> FindPath(Cell start, Cell goal, GridManager grid)
    {
        if (start == null || goal == null) return null;

        var queue = new Queue<Cell>();
        var cameFrom = new Dictionary<Cell, Cell>();

        queue.Enqueue(start);
        cameFrom[start] = null;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current == goal)
                break;

            foreach (var neighbor in GetNeighbors(current, grid))
            {
                if (!neighbor.IsWalkable) continue;
                if (cameFrom.ContainsKey(neighbor)) continue;

                cameFrom[neighbor] = current;
                queue.Enqueue(neighbor);
            }
        }

        if (!cameFrom.ContainsKey(goal))
        {
            return null;
        }

        var path = new List<Cell>();
        var c = goal;
        while (c != null)
        {
            path.Add(c);
            c = cameFrom[c];
        }
        path.Reverse();
        return path;
    }

    private static IEnumerable<Cell> GetNeighbors(Cell cell, GridManager grid)
    {
        int x = cell.x;
        int y = cell.y;

        int[,] dirs = { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } };

        for (int i = 0; i < 4; i++)
        {
            int nx = x + dirs[i, 0];
            int ny = y + dirs[i, 1];

            if (!grid.IsInside(nx, ny)) continue;

            yield return grid.Cells[nx, ny];
        }
    }
}