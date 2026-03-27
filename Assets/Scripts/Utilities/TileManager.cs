using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;

    [SerializeField] private int gridSize = 3;
    [SerializeField] private float tileSpacing = 0.11f;

    private Tile[,] grid;
    private Vector2Int emptyPos;

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        grid = new Tile[gridSize, gridSize];

        // Iterate over the grid
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                // Skip the bottom-right slot (empty)
                if (x == gridSize - 1 && y == gridSize - 1)
                {
                    emptyPos = new Vector2Int(x, y);
                    continue;
                }

                // Calculate position relative to parent rotation
                Vector3 worldPos = transform.position
                                 + transform.right * (-x * tileSpacing)   // left along parent's X axis
                                 + transform.up * (-y * tileSpacing);    // down along parent's Y axis

                GameObject tileObj = Instantiate(tilePrefab, worldPos, transform.rotation, transform);

                Tile tile = tileObj.GetComponent<Tile>();
                if (tile == null)
                {
                    Debug.LogError("Tile script missing on prefab!");
                    continue;
                }

                tile.correctPosition = new Vector2Int(x, y);
                tile.currentPosition = new Vector2Int(x, y);

                grid[x, y] = tile;
            }
        }
    }

    private bool IsAdjacent(Vector2Int tilePos, Vector2Int emptyPos)
    {
        return Mathf.Abs(tilePos.x - emptyPos.x) + Mathf.Abs(tilePos.y - emptyPos.y) == 1;
    }

    public void TryMoveTile(Tile tile)
    {
        if (IsAdjacent(tile.currentPosition, emptyPos))
        {
            MoveTile(tile);
        }
    }

    private void MoveTile(Tile tile)
    {
        Vector2Int oldPos = tile.currentPosition;

        // Update grid
        grid[emptyPos.x, emptyPos.y] = tile;
        grid[oldPos.x, oldPos.y] = null;

        // Update tile data
        tile.currentPosition = emptyPos;

        // Move visually
        tile.transform.position = GridToWorld(emptyPos);

        // Update empty space
        emptyPos = oldPos;
    }

    private Vector3 GridToWorld(Vector2Int pos)
    {
        return transform.position + new Vector3(pos.x * tileSpacing, 0f, pos.y * tileSpacing);
    }
}
