using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private int gridX;
    private int gridY;
    private int range;
    private float fuseTime;

    public GameObject explosionPrefab;

    private GridManager grid;

    public void Init(int x, int y, int range, float fuseTime)
    {
        this.gridX = x;
        this.gridY = y;
        this.range = range;
        this.fuseTime = fuseTime;
    }

    private void Start()
    {
        grid = GridManager.Instance;
        StartCoroutine(FuseCoroutine());
    }

    private IEnumerator FuseCoroutine()
    {
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    private void Explode()
    {
        CreateExplosion(gridX, gridY);

        ExplodeDirection(1, 0); 
        ExplodeDirection(-1, 0);
        ExplodeDirection(0, 1); 
        ExplodeDirection(0, -1);

        Destroy(gameObject);
    }

    private void ExplodeDirection(int dx, int dy)
    {
        int x = gridX;
        int y = gridY;

        for (int i = 1; i <= range; i++)
        {
            x += dx;
            y += dy;

            if (!grid.IsInside(x, y))
                break;

            Cell cell = grid.GetCell(x, y);

            if (cell.type == CellType.SolidWall)
                break;

            CreateExplosion(x, y);

            if (cell.type == CellType.DestructibleWall)
                break;
        }
    }

    private void CreateExplosion(int x, int y)
    {
        Vector3 pos = grid.GridToWorld(x, y);
        GameObject go = Instantiate(explosionPrefab, pos, Quaternion.identity);
        Explosion exp = go.GetComponent<Explosion>();
        if (exp != null)
        {
            exp.Init(x, y);
        }
    }
}