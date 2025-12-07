using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private float worldSpeed;

    [Header("Bombs")]
    public GameObject bombPrefab;
    public float bombFuseTime = 2f;
    public int bombRange = 4;

    private GridManager grid;
    private int gridX;
    private int gridY;
    private bool isMoving;
    private Vector3 targetWorldPos;

    private Keyboard kb = Keyboard.current;
    private GameManager gm;
    private void Start()
    {
        grid = GridManager.Instance;
        gm = GameManager.Instance;
        bool spawnFound = false;
        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.height; y++)
            {
                Cell cell = grid.Cells[x, y];
                if (cell != null && cell.type == CellType.PlayerSpawn)
                {
                    gridX = x;
                    gridY = y;
                    spawnFound = true;
                    break;
                }
            }
            if (spawnFound) break;
        }

        if (!spawnFound)
        {
            gridX = 1;
            gridY = 1;
        }

        transform.position = grid.GridToWorld(gridX, gridY);
        targetWorldPos = transform.position;

        moveSpeed = GameSettings.PlayerCellsPerSecond;
        bombRange = GameSettings.BombRange;

        worldSpeed = moveSpeed * grid.cellSize;
    }

    private void Update()
    {
        if (gm.IsGameOver || gm.IsPaused)
            return;

        HandleMovementInput();
        HandleBombInput();
        MoveToTarget();
    }

    private void HandleMovementInput()
    {
        if (isMoving) return;

        int dx = 0;
        int dy = 0;

        if (kb.wKey.isPressed)
        {
            dy = 1;
        }
        else if (kb.sKey.isPressed)
        {
            dy = -1;
        }
        else if (kb.aKey.isPressed)
        {
            dx = -1;
        }
        else if (kb.dKey.isPressed)
        {
            dx = 1;
        }

        if (dx == 0 && dy == 0)
            return;

        int newX = gridX + dx;
        int newY = gridY + dy;

        if (!grid.IsWalkable(newX, newY))
            return;

        gridX = newX;
        gridY = newY;
        targetWorldPos = grid.GridToWorld(gridX, gridY);
        isMoving = true;

        Vector3 dir = new Vector3(dx, 0f, dy);
        if (dir != Vector3.zero)
        {
            transform.Rotate(0, 0, 0);
            transform.Rotate(90 * dx, 0, 90 * dy);
            transform.forward = dir;

        }
    }

    private void HandleBombInput()
    {

        if (kb.spaceKey.wasPressedThisFrame)
        {
            PlaceBomb();
        }
    }

    private void MoveToTarget()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWorldPos,
            worldSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetWorldPos) < 0.01f)
        {
            transform.position = targetWorldPos;
            isMoving = false;
        }
    }

    private void PlaceBomb()
    {
        Vector3 bombPos = grid.GridToWorld(gridX, gridY);
        GameObject bombGo = Instantiate(bombPrefab, bombPos, Quaternion.identity);
        Bomb bomb = bombGo.GetComponent<Bomb>();
        if (bomb != null)
        {
            bomb.Init(gridX, gridY, bombRange, bombFuseTime);
        }
    }

    public (int x, int y) GetGridPosition()
    {
        return (gridX, gridY);
    }

}