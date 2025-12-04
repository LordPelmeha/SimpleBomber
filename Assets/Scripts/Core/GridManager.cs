using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] public int width;
    [SerializeField] public int height;
    [SerializeField] public float cellSize;

    public Cell[,] Cells { get; private set; }

    public static GridManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Cells = new Cell[width, height];
    }

    public Vector3 GridToWorld(int x, int y)
    {
        return new Vector3(x * cellSize, 0f, y * cellSize);
    }

    public bool IsInside(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public Cell GetCell(int x, int y)
    {
        if (!IsInside(x, y)) return null;
        return Cells[x, y];
    }

    public bool IsWalkable(int x, int y)
    {
        if (!IsInside(x, y)) return false;
        Cell cell = Cells[x, y];
        if (cell == null) return false;
        return cell.IsWalkable;
    }
}