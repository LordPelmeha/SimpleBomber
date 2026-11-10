using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Бомбы")]
    public GameObject bombPrefab;
    public float bombFuseTime = 2f;

    // 🆕 Множитель скорости (изначально 1)
    private float speedMultiplier = 1f;

    private int x;
    private int y;
    private int targetX;
    private int targetY;

    private GridManager grid;
    private bool initialized = false;

    private bool isMoving = false;
    private float moveProgress = 0f;
    private Vector3 startPosition;
    private Vector3 endPosition;

    private void Start()
    {
        grid = GridManager.Instance;
        x = 1;
        y = 1;
        targetX = x;
        targetY = y;
        transform.position = grid.GridToWorld(x, y);
        initialized = true;
    }

    private void Update()
    {
        if (!initialized || grid == null) return;
        if (GameManager.Instance != null && (GameManager.Instance.IsPaused || GameManager.Instance.IsGameOver)) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlaceBomb();
        }

        if (isMoving)
        {
            // 🆕 Применяем множитель к скорости
            float currentSpeed = GameSettings.PlayerCellsPerSecond * speedMultiplier;
            float stepInterval = 1f / Mathf.Max(0.01f, currentSpeed);

            moveProgress += Time.deltaTime / stepInterval;

            if (moveProgress >= 1f)
            {
                transform.position = endPosition;
                x = targetX;
                y = targetY;
                isMoving = false;
                moveProgress = 0f;
            }
            else
            {
                transform.position = Vector3.Lerp(startPosition, endPosition, moveProgress);
            }
        }
        else
        {
            int dx = 0, dy = 0;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) dy = 1;
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) dy = -1;
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) dx = -1;
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) dx = 1;

            if (dx != 0 || dy != 0)
            {
                TryStartMove(dx, dy);
            }
        }
    }

    private void TryStartMove(int dx, int dy)
    {
        int nx = x + dx;
        int ny = y + dy;

        if (!grid.IsWalkable(nx, ny)) return;

        Cell targetCell = grid.GetCell(nx, ny);
        if (targetCell == null) return;

        if (targetCell.IsOccupiedByEnemy)
        {
            PlayerHealth health = GetComponent<PlayerHealth>();
            if (health != null) health.Die();
            return;
        }

        targetX = nx;
        targetY = ny;
        startPosition = transform.position;
        endPosition = grid.GridToWorld(targetX, targetY);
        moveProgress = 0f;
        isMoving = true;
    }

    // 🆕 Метод для увеличения скорости
    public void AddSpeed(float amount)
    {
        speedMultiplier += amount;
        // Ограничитель, чтобы не бегал слишком быстро (например, максимум x3)
        speedMultiplier = Mathf.Clamp(speedMultiplier, 1f, 3f);
        Debug.Log("Скорость увеличена! Текущий множитель: " + speedMultiplier);
    }

    private void PlaceBomb()
    {
        if (bombPrefab == null) return;
        Vector3 pos = grid.GridToWorld(x, y);
        GameObject bombGo = Instantiate(bombPrefab, pos, Quaternion.identity);
        Bomb bomb = bombGo.GetComponent<Bomb>();
        if (bomb != null) bomb.Init(x, y, GameSettings.BombRange, bombFuseTime);
    }
}