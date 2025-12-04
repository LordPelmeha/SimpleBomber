using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    private GridManager grid;
    private int gridX;
    private int gridY;
    private bool isMoving;
    private Vector3 targetWorldPos;

    private void Start()
    {
        grid = GridManager.Instance;

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
    }

    private void Update()
    {
        HandleMovementInput();
        MoveToTarget();
    }

    private void HandleMovementInput()
    {
        if (isMoving) return;

        Keyboard kb = Keyboard.current;

        int dx = 0;
        int dy = 0;

        if (kb.wKey.wasPressedThisFrame)
        {
            dy = 1;
        }
        else if (kb.sKey.wasPressedThisFrame)
        {
            dy = -1;
        }
        else if (kb.aKey.wasPressedThisFrame)
        {
            dx = -1;
        }
        else if (kb.dKey.wasPressedThisFrame)
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
            transform.forward = dir;
        }
    }

    private void MoveToTarget()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWorldPos,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetWorldPos) < 0.01f)
        {
            transform.position = targetWorldPos;
            isMoving = false;
        }
    }

    public (int x, int y) GetGridPosition()
    {
        return (gridX, gridY);
    }
}