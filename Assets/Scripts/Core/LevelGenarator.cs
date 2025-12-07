using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject floorPrefab;
    public GameObject solidWallPrefab;
    public GameObject destructibleWallPrefab;
    public GameObject exitPrefab;

    [Range(0f, 1f)]
    public float destructibleChance = 0.3f;

    private GridManager grid;


    private void Start()
    {
        grid = GridManager.Instance;
        Generate();
    }

    private void Generate()
    {
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

        grid.Cells[1, 1].type = CellType.PlayerSpawn;

        int exitX = grid.width - 2;
        int exitY = grid.height - 2;
        grid.Cells[exitX, exitY].type = CellType.Exit;

        Instantiate(exitPrefab,
            grid.GridToWorld(exitX, exitY),
            Quaternion.identity,
            transform);

        int enemySpawnY = grid.height - 2;
        for (int x = 1; x < grid.width - 1; x++)
        {
            Cell cell = grid.Cells[x, enemySpawnY];
            if (cell != null && cell.IsWalkable)
            {
                cell.type = CellType.EnemySpawn;
            }
        }
    }
}