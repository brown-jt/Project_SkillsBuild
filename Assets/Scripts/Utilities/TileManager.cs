using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TileManager : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;

    [SerializeField] private int gridSize = 3;
    [SerializeField] private float tileSpacing = 0.11f;

    [SerializeField] private Texture2D fullImage;

    [SerializeField] private int scrambleMoves = 30;

    private Tile[,] grid;
    private Texture2D[,] tileTextures;

    private Vector2Int emptyPos;

    private Camera mainCamera;

    private bool isSolved = false;
    public bool IsSolved => isSolved;

    public System.Action OnSolved;
    public System.Action OnUnsolved;

    private void Start()
    {
        GenerateGrid();
        if (fullImage != null)
        {
            SplitImage();
            AssignTexturesToTiles();
            ScrambleTiles(scrambleMoves);

            // In the unlikely event that the scramble results in a solved puzzle, keep scrambling until it's not solved
            while (CheckIfComplete())
            {
                ScrambleTiles(scrambleMoves);
            }
        }
    }

    private void Update()
    {
        if (mainCamera == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~LayerMask.GetMask("ItemInteractable")))
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                if (tile != null && tile.owner == this) TryMoveTile(tile);
            }
        }
    }

    private void GenerateGrid()
    {
        grid = new Tile[gridSize, gridSize];

        // Iterate over the grid
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                // Skip the bottom-right slot (empty) but ensure we assign the emptyPos to track
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
                tileObj.transform.localRotation = Quaternion.Euler(0f, 180f, 0f); // Flip the tile to face upwards

                Tile tile = tileObj.GetComponent<Tile>();
                if (tile == null)
                {
                    Debug.LogError("Tile script missing on prefab!");
                    continue;
                }

                tile.owner = this;
                tile.correctPosition = new Vector2Int(x, y);
                tile.currentPosition = new Vector2Int(x, y);

                grid[x, y] = tile;
            }
        }
    }

    private void SplitImage()
    {
        if (fullImage == null)
        {
            Debug.LogError("No puzzle image assigned!");
            return;
        }

        tileTextures = new Texture2D[gridSize, gridSize];

        int tileWidth = fullImage.width / gridSize;
        int tileHeight = fullImage.height / gridSize;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                // Skip the bottom-right slot (empty)
                if (x == gridSize - 1 && y == gridSize - 1) continue;

                // Create a new Texture2D for this tile
                Texture2D tileTexture = new Texture2D(tileWidth, tileHeight, TextureFormat.RGBA32, false);

                // Copy pixels from full image
                // Flip Y because Unity textures start at bottom-left
                Color[] pixels = fullImage.GetPixels(
                    x * tileWidth,
                    fullImage.height - (y + 1) * tileHeight,
                    tileWidth,
                    tileHeight
                );

                tileTexture.SetPixels(pixels);
                tileTexture.Apply();

                tileTextures[x, y] = tileTexture;
            }
        }
    }

    private void AssignTexturesToTiles()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Tile tile = grid[x, y];
                if (tile == null) continue; // empty tile

                MeshRenderer rend = tile.GetComponent<MeshRenderer>();
                if (rend != null)
                {
                    // Create a new material with URP shader
                    Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                    mat.SetTexture("_BaseMap", tileTextures[x, y]);

                    // Assign to renderer
                    rend.material = mat;
                }
            }
        }
    }

    private void ScrambleTiles(int moves)
    {
        for (int i = 0; i < moves; i++)
        {
            List<Tile> neighbourTiles = GetAdjacentTilesFromEmpty();

            if (neighbourTiles.Count > 0)
            {
                Tile randomTile = neighbourTiles[Random.Range(0, neighbourTiles.Count)];
                MoveTileInstantly(randomTile);
            }
        }
    }

    public void SetCamera(Camera cam)
    {
        mainCamera = cam;
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

            int randomSFX = Random.Range(0, 2);
            if (randomSFX == 0)
            {
                AudioManager.Instance.PlaySFX("Tile");
            }
            else
            {
                AudioManager.Instance.PlaySFX("Tile2");
            }
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
        StartCoroutine(MoveTileSmoothly(tile, GridToWorld(emptyPos), 0.2f));

        // Update empty space
        emptyPos = oldPos;

        // Check solved state after the move
        bool wasSolved = isSolved;
        isSolved = CheckIfComplete();

        // If was solved but now isn't, trigger unsolved event
        if (wasSolved && !isSolved)
        {
            OnUnsolved?.Invoke();
        }
    }

    private void MoveTileInstantly(Tile tile)
    {
        Vector2Int oldPos = tile.currentPosition;

        // Update grid
        grid[emptyPos.x, emptyPos.y] = tile;
        grid[oldPos.x, oldPos.y] = null;

        // Update tile data
        tile.currentPosition = emptyPos;

        // Move instantly instead of using coroutine
        tile.transform.position = GridToWorld(emptyPos);

        // Update empty space
        emptyPos = oldPos;
    }

    private Vector3 GridToWorld(Vector2Int gridPos)
    {
        return transform.position
             + transform.right * (-gridPos.x * tileSpacing)
             + transform.up * (-gridPos.y * tileSpacing);
    }

    private bool CheckIfComplete()
    {
        foreach (Tile tile in grid)
        {
            if (tile != null && tile.currentPosition != tile.correctPosition) return false;
        }

        OnSolved?.Invoke();
        return true;
    }

    private List<Tile> GetAdjacentTilesFromEmpty()
    {
        List<Tile> neighbours = new List<Tile>();

        Vector2Int[] directions =
        {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

        foreach (var dir in directions)
        {
            Vector2Int checkPos = emptyPos + dir;

            if (checkPos.x >= 0 && checkPos.x < gridSize &&
                checkPos.y >= 0 && checkPos.y < gridSize)
            {
                Tile tile = grid[checkPos.x, checkPos.y];
                if (tile != null) neighbours.Add(tile);
            }
        }

        return neighbours;
    }

    public void ResetPuzzle()
    {
        // We may not have spawned the grid yet if Reset is called before Start, so generate if needed
        // This is due to hiding the terminals if not X answers, which can cause Reset to be called before Start
        if (grid == null)
        {
            GenerateGrid();
            if (fullImage != null)
            {
                SplitImage();
                AssignTexturesToTiles();
            }
        }
        ScrambleTiles(scrambleMoves);

        // In the unlikely event that the scramble results in a solved puzzle, keep scrambling until it's not solved
        while (CheckIfComplete())
        {
            ScrambleTiles(scrambleMoves);
        }

        // Reset to a non-solved state after scrambling
        isSolved = false;
    }

    private IEnumerator MoveTileSmoothly(Tile tile, Vector3 targetPos, float duration)
    {
        Vector3 startPos = tile.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            tile.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        tile.transform.position = targetPos;
    }
}
