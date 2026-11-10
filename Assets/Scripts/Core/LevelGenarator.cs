using System.Collections.Generic; // 🆕 Нужно для List
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

    // 🆕 Блок для настройки бонусов
    [Header("Бонусы на карте")]
    public GameObject speedPowerUpPrefab; // Сюда перетащим префаб
    public int minPowerUps = 1;
    public int maxPowerUps = 2;
    public int minDistanceFromPlayer = 3; // Чтобы бонус не появлялся прямо под ногами

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

        // === 3. Точки спавна врагов ===
        for (int x = 1; x < grid.width - 1; x++)
        {
            for (int y = 1; y < grid.height - 1; y++)
            {
                Cell cell = grid.Cells[x, y];

                if (cell == null) continue;
                if (!cell.IsWalkable) continue;
                if (cell.type != CellType.Empty) continue;

                int distFromPlayer = Mathf.Max(
                    Mathf.Abs(x - 1),
                    Mathf.Abs(y - 1)
                );
                if (distFromPlayer < minSpawnDistanceFromPlayer) continue;

                int distFromExit = Mathf.Max(
                    Mathf.Abs(x - exitX),
                    Mathf.Abs(y - exitY)
                );
                if (distFromExit < exitSafeRadius) continue;

                cell.type = CellType.EnemySpawn;
            }
        }

        // === 4. 🆕 Спавн бонусов на карте ===
        SpawnPowerUps();
    }

    // 🆕 Новый метод для спавна бонусов
    // 🆕 Новый метод для спавна бонусов
    // 🆕 Метод для спавна бонусов
    private void SpawnPowerUps()
    {
        if (speedPowerUpPrefab == null) return;

        // Координаты выхода
        int exitX = grid.width - 2;
        int exitY = grid.height - 2;

        // 1. Собираем все пустые клетки (куда можно наступить)
        List<Cell> emptyCells = new List<Cell>();

        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.height; y++)
            {
                Cell cell = grid.Cells[x, y];

                // 🆕 РАЗРЕШАЕМ спавн как на обычном полу, так и на точках врагов
                if (cell != null && (cell.type == CellType.Empty || cell.type == CellType.EnemySpawn))
                {
                    // Проверяем расстояние до игрока
                    int distToPlayer = Mathf.Max(Mathf.Abs(1 - x), Mathf.Abs(1 - y));
                    if (distToPlayer < minDistanceFromPlayer) continue;

                    // Проверяем расстояние до выхода
                    int distToExit = Mathf.Max(Mathf.Abs(exitX - x), Mathf.Abs(exitY - y));
                    if (distToExit < exitSafeRadius) continue;

                    emptyCells.Add(cell);
                }
            }
        }

        // 2. Определяем, сколько бонусов создадим
        int powerUpsToSpawn = Random.Range(minPowerUps, maxPowerUps + 1);

        for (int i = 0; i < powerUpsToSpawn; i++)
        {
            if (emptyCells.Count == 0) break; // Если места не осталось

            int randomIndex = Random.Range(0, emptyCells.Count);
            Cell chosenCell = emptyCells[randomIndex];

            Vector3 pos = grid.GridToWorld(chosenCell.x, chosenCell.y);
            Instantiate(speedPowerUpPrefab, pos, Quaternion.identity, transform);

            emptyCells.RemoveAt(randomIndex);
        }
    }
}