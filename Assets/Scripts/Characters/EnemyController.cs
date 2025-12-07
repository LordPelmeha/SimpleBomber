using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private GridManager grid;
    private PlayerController player;

    private int gridX;
    private int gridY;
    private Vector3 targetWorldPos;
    private bool isMoving;

    private List<Cell> path;
    private int pathIndex;

    public void Init(int startX, int startY)
    {
        gridX = startX;
        gridY = startY;

        transform.position = GridManager.Instance.GridToWorld(gridX, gridY);
        targetWorldPos = transform.position;
    }

    private void Start()
    {
        grid = GridManager.Instance;
        player = FindObjectOfType<PlayerController>();

        RecalculatePath();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        if (player == null) return;

        if (!isMoving)
        {
            RecalculatePath();
            StepAlongPath();
        }

        MoveToTarget();
    }

    private void RecalculatePath()
    {
        if (player == null) return;

        Cell startCell = grid.GetCell(gridX, gridY);
        var (px, py) = player.GetGridPosition();
        Cell goalCell = grid.GetCell(px, py);

        path = Pathfinding.FindPath(startCell, goalCell, grid);
        pathIndex = 0;
    }

    private void StepAlongPath()
    {
        if (path == null || path.Count == 0) return;

        if (pathIndex >= path.Count)
        {
            return;
        }

        Cell nextCell = path[pathIndex];

        if (nextCell.x == gridX && nextCell.y == gridY)
        {
            pathIndex++;
            if (pathIndex >= path.Count) return;
            nextCell = path[pathIndex];
        }

        gridX = nextCell.x;
        gridY = nextCell.y;
        targetWorldPos = grid.GridToWorld(gridX, gridY);
        isMoving = true;

        Vector3 dir = targetWorldPos - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            transform.forward = dir;
        }

        pathIndex++;
    }

    private void MoveToTarget()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWorldPos,
            moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetWorldPos) < 0.01f)
        {
            transform.position = targetWorldPos;
            isMoving = false;

            TryAttackPlayer();

        }
    }

    private void TryAttackPlayer()
    {
        if (player == null) return;

        var (px, py) = player.GetGridPosition();
        if (px == gridX && py == gridY)
        {
            var health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.Die();
            }
        }
    }

    public void Die()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameOver)
        {
            GameManager.Instance.OnEnemyKilled();
        }

        Destroy(gameObject);
    }
}