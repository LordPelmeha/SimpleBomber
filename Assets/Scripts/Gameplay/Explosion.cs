using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float lifeTime = 0.3f;

    private int gridX;
    private int gridY;
    private GridManager grid;

    public void Init(int x, int y)
    {
        gridX = x;
        gridY = y;
    }

    private void Start()
    {
        grid = GridManager.Instance;

        HandleDestructibleWall(); 

        Destroy(gameObject, lifeTime);
    }

    private void HandleDestructibleWall()
    {
        if (grid == null) return;

        Cell cell = grid.GetCell(gridX, gridY);
        if (cell == null) return;

        if (cell.type != CellType.DestructibleWall)
            return;

        Vector3 worldPos = grid.GridToWorld(gridX, gridY);
        Vector3 center = worldPos + new Vector3(0f, 0.5f, 0f);
        float half = grid.cellSize * 0.45f;
        Vector3 halfExtents = new Vector3(half, 0.6f, half);

        Collider[] hits = Physics.OverlapBox(center, halfExtents);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("DestructibleWall"))
            {
                Destroy(hit.gameObject);
            }
        }

        cell.type = CellType.Empty;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.Die();
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.Die();
            }
        }
    }
}