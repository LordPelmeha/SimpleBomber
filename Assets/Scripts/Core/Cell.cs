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

    public bool IsWalkable =>
        type == CellType.Empty ||
        type == CellType.PlayerSpawn ||
        type == CellType.EnemySpawn ||
        type == CellType.Exit;
}
