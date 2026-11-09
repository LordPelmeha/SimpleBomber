using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject floorPrefab;
    public GameObject solidWallPrefab;
    public GameObject destructibleWallPrefab;
    public GameObject exitPrefab;

    [Range(0f, 1f)]
    public float destructibleChance = 0.3f;

    [Tooltip("Минимальное расстояние (в клетках) от игрока до точки спавна врага")]
    public int minSpawnDistanceFromPlayer = 3;

    [Tooltip("Радиус (в клетках) вокруг выхода, где враги не спавнятся")]
    public int exitSafeRadius = 2;

    private GridManager grid;

    private void Start()
    {
        grid = GridManager.Instance;
        Generate();
    }

    private void Generate()
    {
        // === 1. Генерация пола и стен ===
        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.height; y++)
            {
                var cell = new Cell
                {
                    x = x,
                    y = y,
                    type = CellType.Empty
                };
                grid.Cells[x, y] = cell;

                Vector3 pos = grid.GridToWorld(x, y);

                Instantiate(floorPrefab, pos, Quaternion.Euler(90f, 0f, 0f), transform);

                bool isBorder =
                    (x == 0 || y == 0 ||
                     x == grid.width - 1 ||
                     y == grid.height - 1);

                if (isBorder)
                {
                    cell.type = CellType.SolidWall;
                    Instantiate(solidWallPrefab, pos, Quaternion.identity, transform);
                    continue;
                }

                if (x % 2 == 0 && y % 2 == 0)
                {
                    cell.type = CellType.SolidWall;
                    Instantiate(solidWallPrefab, pos, Quaternion.identity, transform);
                    continue;
                }

                bool playerSafeZone = (x <= 2 && y <= 2);
                bool exitZone = (x >= grid.width - 3 && y >= grid.height - 3);

                if (!playerSafeZone && !exitZone)
                {
                    if (Random.value < destructibleChance)
                    {
                        cell.type = CellType.DestructibleWall;
                        Instantiate(destructibleWallPrefab, pos, Quaternion.identity, transform);
                    }
                }
            }
        }

        // === 2. Игрок и выход ===
        grid.Cells[1, 1].type = CellType.PlayerSpawn;

        int exitX = grid.width - 2;
        int exitY = grid.height - 2;
        grid.Cells[exitX, exitY].type = CellType.Exit;

        Instantiate(exitPrefab,
            grid.GridToWorld(exitX, exitY),
            Quaternion.identity,
            transform);


        for (int x = 1; x < grid.width - 1; x++)
        {
            for (int y = 1; y < grid.height - 1; y++)
            {
                Cell cell = grid.Cells[x, y];

                // Только пустые проходимые клетки
                if (cell == null) continue;
                if (!cell.IsWalkable) continue;
                if (cell.type != CellType.Empty) continue;

                // Не спавним слишком близко к игроку
                int distFromPlayer = Mathf.Max(
                    Mathf.Abs(x - 1),
                    Mathf.Abs(y - 1)
                );
                if (distFromPlayer < minSpawnDistanceFromPlayer) continue;

                // Не спавним рядом с выходом
                int distFromExit = Mathf.Max(
                    Mathf.Abs(x - exitX),
                    Mathf.Abs(y - exitY)
                );
                if (distFromExit < exitSafeRadius) continue;

                cell.type = CellType.EnemySpawn;
            }
        }
    }
}