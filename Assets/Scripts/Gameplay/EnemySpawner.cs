using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 5f;

    private GridManager grid;
    private List<Cell> spawnCells = new List<Cell>();

    private void Start()
    {
        grid = GridManager.Instance;

        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.height; y++)
            {
                Cell cell = grid.Cells[x, y];
                if (cell != null && cell.type == CellType.EnemySpawn)
                {
                    spawnCells.Add(cell);
                }
            }
        }

        if (spawnCells.Count == 0)
        {
            Debug.LogWarning("EnemySpawner: Нет клеток EnemySpawn!");
        }
        else
        {
            StartCoroutine(SpawnLoop());
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
                yield break;

            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null || spawnCells.Count == 0) return;

        Cell spawnCell = spawnCells[Random.Range(0, spawnCells.Count)];

        Vector3 pos = grid.GridToWorld(spawnCell.x, spawnCell.y);
        GameObject enemyGo = Instantiate(enemyPrefab, pos, Quaternion.identity);

        EnemyController enemy = enemyGo.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.Init(spawnCell.x, spawnCell.y);
        }
    }
}