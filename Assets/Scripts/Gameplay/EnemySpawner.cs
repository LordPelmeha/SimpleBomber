using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    [Header("Спавн")]
    [Tooltip("Минимальное расстояние от игрока (в клетках) до точки появления врага")]
    public int minSpawnDistanceFromPlayer = 5;

    private GridManager grid;
    private GameManager gm;
    private List<Cell> spawnCells = new List<Cell>();
    private HashSet<Cell> occupiedCells = new HashSet<Cell>();

    private void Start()
    {
        gm = GameManager.Instance;
        grid = GridManager.Instance;

        if (grid == null)
        {
            Debug.LogError("EnemySpawner: GridManager не найден!");
            return;
        }

        for (int x = 0; x < grid.width; x++)
            for (int y = 0; y < grid.height; y++)
            {
                Cell cell = grid.Cells[x, y];
                if (cell != null && cell.type == CellType.EnemySpawn)
                    spawnCells.Add(cell);
            }

        if (spawnCells.Count == 0)
        {
            Debug.LogWarning("EnemySpawner: Нет клеток EnemySpawn!");
            return;
        }

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (gm != null && gm.IsGameOver) yield break;

            if (gm != null && gm.IsPaused)
            {
                yield return null;
                continue;
            }

            SpawnEnemy();

            float interval = Mathf.Max(0.1f, GameSettings.EnemySpawnInterval);
            yield return new WaitForSeconds(interval);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null || spawnCells.Count == 0) return;

   
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null) return;

        int px = Mathf.RoundToInt(player.transform.position.x / grid.cellSize);
        int py = Mathf.RoundToInt(player.transform.position.z / grid.cellSize);


        Cell spawnCell = null;
        for (int attempt = 0; attempt < 30; attempt++)
        {
            Cell candidate = spawnCells[Random.Range(0, spawnCells.Count)];

   
            if (occupiedCells.Contains(candidate)) continue;

   
            if (candidate.enemy != null) continue;

            // расстояние по Чебышёву
            int dist = Mathf.Max(
                Mathf.Abs(px - candidate.x),
                Mathf.Abs(py - candidate.y)
            );

            if (dist < minSpawnDistanceFromPlayer) continue;

            spawnCell = candidate;
            break;
        }

        if (spawnCell == null)
        {
            // все точки заняты или слишком близко 
            return;
        }

        //Спавн
        Vector3 pos = grid.GridToWorld(spawnCell.x, spawnCell.y);
        GameObject enemyGo = Instantiate(enemyPrefab, pos, Quaternion.identity);

        EnemyController enemy = enemyGo.GetComponent<EnemyController>();
        if (enemy != null)
            enemy.Init(spawnCell.x, spawnCell.y);

        occupiedCells.Add(spawnCell);
        Cell captured = spawnCell;

        var tracker = enemyGo.AddComponent<SpawnCellTracker>();
        tracker.Setup(() => occupiedCells.Remove(captured));
    }
}