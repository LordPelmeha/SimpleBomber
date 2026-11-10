using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Параметры типа врага")]
    [Tooltip("Сколько взрывов нужно, чтобы убить. 1 — обычный, 2 — танк.")]
    public int hitsRequired = 1;

    [Tooltip("Множитель скорости. 1 — обычный, 0.5 — в 2 раза медленнее.")]
    public float speedMultiplier = 1f;

    [Tooltip("Время неуязвимости после получения урона (в секундах)")]
    public float invulnerabilityTime = 0.2f;

    [Header("Движение")]
    public float moveSpeedCellsPerSecond = 2f;

    private int currentHits = 0;
    private float lastHitTime = -999f;
    private bool isDead = false;

    private int x;
    private int y;
    private GridManager grid;
    private float moveTimer;
    private bool initialized = false;

    public void Init(int startX, int startY)
    {
        grid = GridManager.Instance;
        x = startX;
        y = startY;

        Cell cell = grid.GetCell(x, y);
        if (cell != null)
            cell.enemy = this;

        transform.position = grid.GridToWorld(x, y);

        moveTimer = 0f;
        initialized = true;
    }

    private void Update()
    {
        if (!initialized || grid == null || isDead) return;

        moveTimer += Time.deltaTime;
        float stepInterval = 1f / Mathf.Max(0.01f, moveSpeedCellsPerSecond * speedMultiplier);

        if (moveTimer >= stepInterval)
        {
            moveTimer = 0f;
            TryStep();
        }
    }

    private void TryStep()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null) return;

        int px = Mathf.RoundToInt(player.transform.position.x / grid.cellSize);
        int py = Mathf.RoundToInt(player.transform.position.z / grid.cellSize);

        // Если мы уже на клетке игрока — убиваем его
        if (px == x && py == y)
        {
            var health = player.GetComponent<PlayerHealth>();
            if (health != null) health.Die();
            return;
        }

        Cell myCell = grid.GetCell(x, y);
        Cell playerCell = grid.GetCell(px, py);

        if (myCell != null && playerCell != null)
        {
            List<Cell> path = Pathfinding.FindPath(myCell, playerCell, grid);

            if (path != null && path.Count > 1)
            {
                Cell nextCell = path[1];
                TryMoveTo(nextCell.x, nextCell.y);
            }
        }
    }

    private void TryMoveTo(int nx, int ny)
    {
        Cell target = grid.GetCell(nx, ny);
        if (target == null) return;

        if (target.IsOccupiedByEnemy) return;

        // 🆕 ПРОВЕРКА НА ИГРОКА: если шагаем в клетку игрока, наносим урон
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            int px = Mathf.RoundToInt(player.transform.position.x / grid.cellSize);
            int py = Mathf.RoundToInt(player.transform.position.z / grid.cellSize);

            if (px == nx && py == ny)
            {
                var health = player.GetComponent<PlayerHealth>();
                if (health != null) health.Die();
                return; // Враг не делает шаг, игрок умирает
            }
        }

        Cell oldCell = grid.GetCell(x, y);
        if (oldCell != null && oldCell.enemy == this)
            oldCell.enemy = null;

        x = nx;
        y = ny;
        target.enemy = this;

        transform.position = grid.GridToWorld(x, y);
    }

    public void Hit()
    {
        if (isDead) return;

        if (Time.time - lastHitTime < invulnerabilityTime)
            return;

        lastHitTime = Time.time;
        currentHits++;

        Debug.Log($"[{gameObject.name}] Получен урон! {currentHits} / {hitsRequired}");

        if (currentHits >= hitsRequired)
        {
            isDead = true;
            Die();
        }
        else
        {
            StartCoroutine(HitFlash());
        }
    }

    private IEnumerator HitFlash()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Color[] originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_BaseColor"))
            {
                originalColors[i] = renderers[i].material.GetColor("_BaseColor");
                renderers[i].material.SetColor("_BaseColor", Color.red);
            }
            else if (renderers[i].material.HasProperty("_Color"))
            {
                originalColors[i] = renderers[i].material.color;
                renderers[i].material.color = Color.red;
            }
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_BaseColor"))
                renderers[i].material.SetColor("_BaseColor", originalColors[i]);
            else if (renderers[i].material.HasProperty("_Color"))
                renderers[i].material.color = originalColors[i];
        }
    }

    public void Die()
    {
        if (grid != null)
        {
            Cell c = grid.GetCell(x, y);
            if (c != null && c.enemy == this)
                c.enemy = null;
        }

        // 🆕 Уведомляем GameManager, что враг убит
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnEnemyKilled();
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (grid != null)
        {
            Cell c = grid.GetCell(x, y);
            if (c != null && c.enemy == this)
                c.enemy = null;
        }
    }
}