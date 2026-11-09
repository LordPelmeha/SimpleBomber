public enum CellType
{
    Empty,
    SolidWall,
    DestructibleWall,
    PlayerSpawn,
    EnemySpawn,
    Exit
}

public class Cell
{
    public int x;
    public int y;
    public CellType type;


    public EnemyController enemy;

    public bool IsWalkable =>
        type == CellType.Empty ||
        type == CellType.PlayerSpawn ||
        type == CellType.EnemySpawn ||
        type == CellType.Exit;


    public bool IsOccupiedByEnemy => enemy != null;
}
